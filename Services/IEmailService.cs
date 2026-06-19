namespace UCCD_App.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string subject, string body);
    }
}