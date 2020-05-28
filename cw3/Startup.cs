
using cw3.DAL;
using cw3.Middlewares;
using cw3.Models;
using cw3.ModelsNew;
using cw3.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace cw3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<s8346Context>(options =>
            {
                options.UseSqlServer("Data Source=10.1.1.36; Initial Catalog=s8346;User ID=apbds8346;Password=admin'");
            });

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //       .AddJwtBearer(options =>
            //       {
            //           options.TokenValidationParameters = new TokenValidationParameters
            //           {
            //               ValidateIssuer = true,
            //               ValidateAudience = true,
            //               ValidateLifetime = true,
            //               ValidIssuer = "Gakko",
            //               ValidAudience = "Students",
            //               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]))
            //           };
            //       });
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IStudentsDbService, SqlServerDbService>();
            services.AddSingleton<IDbService, MockDbService>();
            services.AddControllersWithViews();

            //services.AddAuthentication("AuthenticationBasic")
            //      .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("AuthenticationBasic", null);

            services.AddControllers()
                    .AddXmlSerializerFormatters();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbService idb)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMiddleware<LoggingMiddleware>();
            app.Use(async (context, next) => {
                if (!context.Request.Headers.ContainsKey("Index"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Nie podano indeksu w nagłówku");
                    return;
                }
                var index = context.Request.Headers["Index"].ToString();

                IStudentsDbService dbService = new SqlServerDbService();
                if (!dbService.CheckIndex(index))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Nie ma takiego studenta w bazie");
                    return;
                }
                await next();
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
