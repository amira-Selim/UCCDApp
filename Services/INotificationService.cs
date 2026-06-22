using System.Threading.Tasks;

namespace UCCD_App.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string title, string message, string type = "Info", string? userId = null);
    }
}