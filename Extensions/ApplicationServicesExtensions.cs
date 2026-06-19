using Microsoft.EntityFrameworkCore;
using UCCD_App.Context;
using UCCD_App.Repo;
using UCCD_App.Services;

namespace UCCD_App.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // نقلنا هنا الـ DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("db")));

            // نقلنا هنا الـ Repos و الـ Services
            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            services.AddScoped<IWishlistRepo, WishlistRepo>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IStudentCourseService, StudentCourseService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IJobBoardService, JobBoardService>();
            
            // ====== هنا السطر الجديد لربط خدمة المتطوعين ======
            services.AddScoped<IVolunteerService, VolunteerService>();

            // نقلنا هنا الـ CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(_ => true);
                });
            });

            return services;
        }
    }
}