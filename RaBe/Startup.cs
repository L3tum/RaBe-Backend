#region using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using RaBe.Healthchecks;

#endregion

namespace RaBe
{
	public class Startup
	{
		internal static readonly byte[] SecretKey = Encoding.ASCII.GetBytes
			("RaBe-2374-OFFKDI940NG7:56753253-gso-5769-0921-kfirox29zoxv");

		private readonly IHostingEnvironment env;
		private static DateTime lastTs;
		private static TimeSpan lastProcessorTime;

		public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			Configuration = configuration;
			this.env = env;
			lastTs = DateTime.UtcNow;
			lastProcessorTime = TimeSpan.Zero;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			IFilterMetadata policy = null;

			if (env.IsDevelopment())
			{
				policy = new AllowAnonymousFilter();
			}
			else
			{
				var built = new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.Build();
				policy = new AuthorizeFilter(built);
			}

			services.AddMvcCore(o =>
				{
					o.Filters.Add(policy);
					o.Filters.Add(typeof(DBSaveChangesFilter));
					o.EnableEndpointRouting = false;
					o.ReturnHttpNotAcceptable = true;
				}).AddNewtonsoftJson().AddApiExplorer().AddAuthorization().AddCors().AddCacheTagHelper()
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			services.AddDbContext<RaBeContext>();
			services.AddSession();

			//Configure JWT Token Authentication
			services.AddAuthentication(auth =>
				{
					auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(token =>
				{
					token.RequireHttpsMetadata = false;
					token.SaveToken = true;
					token.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						//Same Secret key will be used while creating the token
						IssuerSigningKey = new SymmetricSecurityKey(SecretKey),
						ValidateIssuer = true,
						//Usually, this is your application base URL
						ValidIssuer = "http://localhost:80/",
						ValidateAudience = true,
						//Here, we are creating and using JWT within the same application.
						//In this case, base URL is fine.
						//If the JWT is created using a web service, then this would be the consumer URL.
						ValidAudience = "GSO",
						RequireExpirationTime = true,
						ValidateLifetime = true,
						ClockSkew = TimeSpan.FromMinutes(5)
					};
				});

			services.AddOpenApiDocument(g =>
			{
				g.Title = "RaBe Backend";
				g.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
				g.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT", new OpenApiSecurityScheme
				{
					Type = OpenApiSecuritySchemeType.ApiKey,
					Name = "Authorization",
					In = OpenApiSecurityApiKeyLocation.Header,
					BearerFormat = "Bearer [Token]"
				}));

				g.SchemaGenerator = new OpenApiSchemaGenerator(new AspNetCoreOpenApiDocumentGeneratorSettings());

				g.PostProcess = document => document.Produces = new List<string>
				{
					"application/json",
					"text/json"
				};
			});

			services.AddFluentEmail(Environment.GetEnvironmentVariable("EMAIL_SENDER") ?? "test@test.test")
				.AddRazorRenderer()
				.AddSmtpSender(Environment.GetEnvironmentVariable("EMAIL_SERVER") ?? "smtp.google.com",
					int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT") ?? "25"));

			services.AddCors(o =>
			{
				o.AddDefaultPolicy(cp =>
				{
					cp.AllowAnyOrigin();
					cp.WithHeaders("authorization", "accept", "content-type", "origin");
					cp.AllowAnyMethod();
				});
			});

			services.AddHealthChecks()
				.AddDiskStorageHealthCheck(setup => { setup.AddDrive(new DriveInfo(env.ContentRootPath).Name); })
				.AddCheck<MemoryHealthCheck>("memory")
				.AddSmtpHealthCheck(setup =>
				{
					setup.Host = Environment.GetEnvironmentVariable("EMAIL_SERVER") ?? "smtp.google.com";
					setup.Port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT") ?? "25");
				})
				.AddSqlite("DataSource=./RaBe.db")
				.AddDbContextCheck<RaBeContext>();
			services.AddHealthChecksUI(setupSettings: settings =>
				settings.SetHealthCheckDatabaseConnectionString("DataSource=./RaBe.db")
					.AddHealthCheckEndpoint("RaBe Backend", GetHealthUri()));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, RaBeContext dbContext)
		{
			app.UseSession();
			app.UseMvc();
			app.UseHealthChecks("/health", new HealthCheckOptions
				{
					Predicate = _ => true
				})
				.UseHealthChecks("/healthz", new HealthCheckOptions
				{
					Predicate = _ => true,
					ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
				});
			app.UseHealthChecksUI();
			app.UseRouting().UseEndpoints(config => { config.MapHealthChecksUI(); });
			app.UseRewriter(new RewriteOptions().AddRedirect("^$", "/swagger"));

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
				app.UseResponseCompression();
			}

			if (!env.IsDevelopment())
			{
				app.UseHttpsRedirection();
			}

			//Add JWToken to all incoming HTTP Request Header
			app.Use(async (context, next) =>
			{
				var JWToken = context.Session.GetString("JWToken");

				if (!string.IsNullOrEmpty(JWToken))
				{
					context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
				}

				await next();
			});
			app.UseAuthentication();

			app.UseOpenApi();
			app.UseSwaggerUi3();
			app.UseCors();

			// ===== Create tables ======
			dbContext.Database.EnsureCreated();
		}

		private string GetHealthUri()
		{
			return (Environment.GetEnvironmentVariable("APPLICATION_URL") ?? (env.IsDevelopment()
				        ? "http://localhost:53125"
				        : "https://rabe-backend.herokuapp.com")) + "/healthz";
		}
	}
}