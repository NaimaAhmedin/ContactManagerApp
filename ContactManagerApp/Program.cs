using ContactManagerApp.Data;
using ContactManagerApp.Models;
using ContactManagerApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace ContactManagerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    Task.Run(async () =>
                    {
                        await ContextSeed.SeedRolesAsync(userManager, roleManager);
                        await ContextSeed.SeedSuperAdminAsync(userManager, roleManager);
                    }).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((context, services) =>
                    {
                        var configuration = context.Configuration;

                        services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                        services.AddIdentity<ApplicationUser, IdentityRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultUI()
                            .AddDefaultTokenProviders();

                        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie()
                        .AddLinkedIn(options =>
                        {
                            options.ClientId = configuration["Authentication:LinkedIn:ClientId"];
                            options.ClientSecret = configuration["Authentication:LinkedIn:ClientSecret"];
                            options.Scope.Add("email");
                            options.Scope.Add("profile");
                            options.SaveTokens = true;
                            options.UserInformationEndpoint = "https://api.linkedin.com/v2/userinfo";
                            options.Events = new OAuthEvents
                            {
                                OnCreatingTicket = async context =>
                                {
                                    var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                                    var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                                    response.EnsureSuccessStatusCode();

                                    var userJson = await response.Content.ReadAsStringAsync();
                                    using (var user = JsonDocument.Parse(userJson))
                                    {
                                        var FirstName = user.RootElement.GetProperty("given_name").GetString();
                                        var LastName = user.RootElement.GetProperty("family_name").GetString();
                                        var email = user.RootElement.GetProperty("email").GetString();

                                        // Ensure these values are not null
                                        if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
                                        {
                                            throw new InvalidOperationException("LinkedIn did not provide a first name or last name.");
                                        }

                                        context.Identity.AddClaim(new Claim("FirstName", FirstName));
                                        context.Identity.AddClaim(new Claim("LastName", LastName));
                                        context.Identity.AddClaim(new Claim(ClaimTypes.Email, email));
                                    }
                                }
                            };
                        });
                        services.AddTransient<IEmailSender, EmailSender>();
                        


                        services.AddControllersWithViews();
                        services.AddRazorPages();
                    });

                    webBuilder.Configure((context, app) =>
                    {
                        var env = context.HostingEnvironment;

                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                            app.UseDatabaseErrorPage();
                        }
                        else
                        {
                            app.UseExceptionHandler("/Home/Error");
                            app.UseHsts();
                        }
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
                            endpoints.MapRazorPages();
                        });
                    });
                });
    }
}
