using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace RaBe
{
    public class Startup
    {
        private IHostingEnvironment env;
        internal static readonly byte[] SecretKey = Encoding.ASCII.GetBytes
                 ("RaBe-2374-OFFKDI940NG7:56753253-gso-5769-0921-kfirox29zoxv");

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
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

            if (env.IsDevelopment())
            {
                services.AddMvc(o =>
                {
                    o.Filters.Add(new AllowAnonymousFilter());
                }).AddNewtonsoftJson();
            }
            else
            {
                services.AddMvc(o =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    o.Filters.Add(new AuthorizeFilter(policy));
                }).AddNewtonsoftJson();
            }

            services.AddSwaggerDocument((g) =>
            {
                g.Title = "RaBe Backend";
            });

            services.AddFluentEmail(Environment.GetEnvironmentVariable("EMAIL_SENDER"))
                .AddRazorRenderer()
                .AddSmtpSender(Environment.GetEnvironmentVariable("EMAIL_SERVER"), int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, RaBeContext dbContext)
        {
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

            app.UseSession();
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

            // ===== Create tables ======
            dbContext.Database.EnsureCreated();
        }
    }
}
