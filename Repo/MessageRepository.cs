using Microsoft.EntityFrameworkCore;
using UCCD_App.Context;
using UCCD_App.Models;

namespace UCCD_App.Repo
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Message> AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<Message>> GetAllAsync()
        {
            return await _context.Messages
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<Message?> GetByIdAsync(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> UpdateAsync(Message message)
        {
            _context.Messages.Update(message);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Message message)
        {
            _context.Messages.Remove(message);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}   