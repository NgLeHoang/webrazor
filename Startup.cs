using System.Net;
using Album.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MyApp.Security.Requirements;
using MyApp.Services;
using webrazorapp.models;

namespace webrazorapp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddOptions();
            var mailSetting = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailSetting);
            services.AddSingleton<IEmailSender, SendMailService>();

            services.AddRazorPages();
            services.AddDbContext<AppDbContext>(options =>
            {
                string? connectString = Configuration.GetConnectionString("MyBlogContext");
                options.UseSqlServer(connectString);
            });

            // Register Identity
            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // services.AddDefaultIdentity<AppUser>().AddEntityFrameworkStores<MyBlogContext>()
            // .AddDefaultTokenProviders();

            // IdentityOptions
            services.Configure<IdentityOptions>(options =>
            {
                // Set Password
                options.Password.RequireDigit = false; // Not must have number
                options.Password.RequireLowercase = false; // Not must lower case
                options.Password.RequireNonAlphanumeric = false; // No special characters required
                options.Password.RequireUppercase = false; // Capital letters are not required
                options.Password.RequiredLength = 3; // Minimum number of characters for password
                options.Password.RequiredUniqueChars = 1; // Number of distinct characters

                // Config Lockout - lock user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Lock 5 minuates
                options.Lockout.MaxFailedAccessAttempts = 5; // Fail 5 times is lock
                options.Lockout.AllowedForNewUsers = true;

                // Config User.
                options.User.AllowedUserNameCharacters = // User name characters
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email is the only one

                // Config sign in.
                options.SignIn.RequireConfirmedEmail = true;            // Configure email address validation (email must exist)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Verify phone number
                options.SignIn.RequireConfirmedAccount = true;

            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login/";
                options.LogoutPath = "/logout/";
                options.AccessDeniedPath = "/khongduoctruycap.html";
            });

            services.AddAuthentication()
            .AddGoogle(googleOptions =>
            {
                // Đọc thông tin Authentication:Google từ appsettings.json
                var googleAuthNSection = Configuration.GetSection("Authentication:Google");

                // Thiết lập ClientID và ClientSecret để truy cập API google
                googleOptions.ClientId = googleAuthNSection["ClientId"];
                googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
                // Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
                googleOptions.CallbackPath = "/dang-nhap-tu-google";
            })
            .AddFacebook(facebookOptions =>
            {
                // Đọc cấu hình
                IConfigurationSection facebookAuthNSection = Configuration.GetSection("Authentication:Facebook");
                facebookOptions.AppId = facebookAuthNSection["AppId"];
                facebookOptions.AppSecret = facebookAuthNSection["AppSecret"];
                // Thiết lập đường dẫn Facebook chuyển hướng đến
                facebookOptions.CallbackPath = "/dang-nhap-tu-facebook";
            });
            services.AddSingleton<IdentityErrorDescriber, AppIdentityErroDescriber>();

            services.AddAuthorization(options => {
                options.AddPolicy("AllowEditRole", policyBuilder => {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireRole("Admin");
                    policyBuilder.RequireClaim("canedit", "user");
                });

                options.AddPolicy("InGenZ", policyBuilder => {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.Requirements.Add(new GenZRequirement());
                });

                options.AddPolicy("ShowAdminMenu", policyBuilder => {
                    policyBuilder.RequireRole("Admin");
                });

                options.AddPolicy("CanUpdateArticle", policyBuilder => {
                    policyBuilder.Requirements.Add(new ArticleUpdateRequirement());
                });
            });

            services.AddTransient<IAuthorizationHandler, AppAuthorizationHandler>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

        }
    }
}