using UCCD_App.Dto;

namespace UCCD_App.Services
{
    public interface IMessageService
    {
        Task<MessageResponseDto> CreateAsync(CreateMessageDto dto);
        Task<List<MessageResponseDto>> GetAllAsync();
        Task<MessageResponseDto?> GetByIdAsync(int id);
        Task<bool> MarkAsReadAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}