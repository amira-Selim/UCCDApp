using UCCD_App.Models;

namespace UCCD_App.Repo
{
    public interface IMessageRepository
    {
        Task<Message> AddAsync(Message message);
        Task<List<Message>> GetAllAsync();
        Task<Message?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Message message);
        Task<bool> DeleteAsync(Message message);
    }
}
