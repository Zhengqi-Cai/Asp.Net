
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using EBookReader.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using EBookReader.Models;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.StaticFiles;

namespace EBookReader
{
  public class Startup
  {
        public Startup(IConfiguration configuration,IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        private IHostingEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
    {
            services.AddDbContext<EBookReaderDbContext>(options =>
          options.UseSqlServer(
              Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc();
            services.AddIdentity<User, IdentityRole>().AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<EBookReaderDbContext>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = false;
                options.Cookie.IsEssential = true;
            });
            

        }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
    {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            var provider = new FileExtensionContentTypeProvider();
            // Add new mappings to support
            provider.Mappings[".epub"] = "application/epub+zip";

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
           Path.Combine(_env.WebRootPath, "FileStorage")), //Please note that I have replaced Directory.GetCurrentDirectory() as the first argument. This is changed in .Net core 2.2, the reason is yet to be discovered for me.
                RequestPath = "/Library",//Notice this request path prepended in test.html while providing the file path
                ContentTypeProvider = provider
            });
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
           Path.Combine(_env.WebRootPath, "FileStorage")),
                RequestPath = "/Library"
            });

            app.UseSession();
            app.UseAuthentication();
            CreateRoles(serviceProvider).Wait();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "default",
                  template: "{controller=Book}/{action=Index}/{id?}"
                );
            });
            

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Server Error!");
            });
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();
            string[] roleNames = { "Admin", "User" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            User user = await UserManager.FindByEmailAsync("zhengqi_cai@outlook.com");

            if (user == null)
            {
                user = new User()
                {
                    UserName = "zhengqi_cai@outlook.com",
                    Email = "zhengqi_cai@outlook.com",
                };
                await UserManager.CreateAsync(user, "Test@123");
            }
            await UserManager.AddToRoleAsync(user, "Admin");


            User user1 = await UserManager.FindByEmailAsync("testUser@syr.edu");

            if (user1 == null)
            {
                user1 = new User()
                {
                    UserName = "testUser@syr.edu",
                    Email = "testUser@syr.edu",
                };
                await UserManager.CreateAsync(user1, "Test@123");
            }
            await UserManager.AddToRoleAsync(user1, "User");



        }
    }
}
