using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WebStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WebStore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();

            services.AddEntityFramework()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();
            app.UseIdentity();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        //private async Task CreateRoles(ApplicationDbContext context, IServiceProvider serviceProvider)
        //{
        //    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        //    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //    // First, Creating User role as each role in User Manager  
        //    List<IdentityRole> roles = new List<IdentityRole>();
        //    roles.Add(new IdentityRole { Name = "Manager", NormalizedName = "MANAGER" });

        //    //Then, the machine added Default User as the Admin user role
        //    foreach (var role in roles)
        //    {
        //        var roleExit = await roleManager.RoleExistsAsync(role.Name);
        //        if (!roleExit)
        //        {
        //            context.Roles.Add(role);
        //            context.SaveChanges();
        //        }
        //    }

        //    await CreateUser(context, userManager);
        //}

        //private async Task CreateUser(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        //{
        //    var managerUser = await userManager.FindByNameAsync("TestManager");
        //    if (managerUser != null)
        //    {
        //        if (!(await userManager.IsInRoleAsync(userManager.Id, "Manager")))
        //            userManager.AddToRoleAsync(userManager.UserName, "TestManager");
        //    }
        //    else  
        //    {
        //        var newManager = new ApplicationUser()
        //        {
        //            UserName = "TestManager"
        //        };
        //        await userManager.CreateAsync(newManager, "Alpha@Mega");
        //        await userManager.AddToRoleAsync(userManager.UserName, "Manager");
        //    }
        //}

        //app.Run(async(context) =>
        //{
        //    await context.Response.WriteAsync("<h1>Sorry, an error has occurred, please return <a href='/'>Home</a></h1>");
        //});
    }
}
