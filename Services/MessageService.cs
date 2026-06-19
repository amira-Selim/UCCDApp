using UCCD_App.Dto;
using UCCD_App.Models;
using UCCD_App.Repo;

namespace UCCD_App.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repo;
        private readonly IEmailService _emailService;

        public MessageService(IMessageRepository repo, IEmailService emailService)
        {
            _repo = repo;
            _emailService = emailService;
        }

        public async Task<MessageResponseDto> CreateAsync(CreateMessageDto dto)
        {
            var message = new Message
            {
                Name = dto.Name,
                Email = dto.Email,
                IssueType = dto.IssueType,
                Content = dto.Content
            };

            await _repo.AddAsync(message);

            // Email Body
            var body = $@"
                <h3>New Message Received</h3>
                <p><strong>Name:</strong> {dto.Name}</p>
                <p><strong>Email:</strong> {dto.Email}</p>
                <p><strong>Type:</strong> {dto.IssueType}</p>
                <p><strong>Message:</strong><br/>{dto.Content}</p>
            ";

            await _emailService.SendEmailAsync(
                $"New Message - {dto.IssueType}",
                body
            );

            return new MessageResponseDto
            {
                Id = message.Id,
                Name = message.Name,
                Email = message.Email,
                IssueType = message.IssueType,
                Content = message.Content,
                CreatedAt = message.CreatedAt,
                IsRead = message.IsRead,
                IsArchived = message.IsArchived
            };
        }

        public async Task<List<MessageResponseDto>> GetAllAsync()
        {
            var messages = await _repo.GetAllAsync();

            return messages.Select(m => new MessageResponseDto
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                IssueType = m.IssueType,
                Content = m.Content,
                CreatedAt = m.CreatedAt,
                IsRead = m.IsRead,
                IsArchived = m.IsArchived
            }).ToList();
        }

        public async Task<MessageResponseDto?> GetByIdAsync(int id)
        {
            var m = await _repo.GetByIdAsync(id);
            if (m == null) return null;

            return new MessageResponseDto
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                IssueType = m.IssueType,
                Content = m.Content,
                CreatedAt = m.CreatedAt,
                IsRead = m.IsRead,
                IsArchived = m.IsArchived
            };
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            var m = await _repo.GetByIdAsync(id);
            if (m == null) return false;

            m.IsRead = true;
            return await _repo.UpdateAsync(m);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var m = await _repo.GetByIdAsync(id);
            if (m == null) return false;

            return await _repo.DeleteAsync(m);
        }
    }
}