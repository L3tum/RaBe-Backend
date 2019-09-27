#region using

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;

#endregion

namespace RaBe
{
	public class Startup
	{
		internal static readonly byte[] SecretKey = Encoding.ASCII.GetBytes
			("RaBe-2374-OFFKDI940NG7:56753253-gso-5769-0921-kfirox29zoxv");

		private readonly IHostingEnvironment env;

		public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			Configuration = configuration;
			this.env = env;
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

			services.AddMvc(o =>
			{
				o.Filters.Add(policy);
				o.Filters.Add(typeof(DBSaveChangesFilter));
				o.EnableEndpointRouting = false;
				o.ReturnHttpNotAcceptable = true;
			}).AddNewtonsoftJson().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

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
				.AddSmtpSender(Environment.GetEnvironmentVariable("EMAIL_SERVER") ?? "test.test",
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
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, RaBeContext dbContext)
		{
			app.UseSession();
			app.UseMvc();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
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
	}
}