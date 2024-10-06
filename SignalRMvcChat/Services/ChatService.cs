using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SignalRMvcChat.Data;
using SignalRMvcChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRMvcChat.Services
{
    public class ChatService : IChatService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ChatService(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Conversation? GetConversation(int conversationId)
        {
            var conversation = _context.Conversations
                .AsNoTracking()
                .Include(c => c.Messages)
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == conversationId);
            return conversation.Result;
        }

        public Conversation? GetConversation(AppUser currentUser, AppUser receiver)
        {
            return _context.Conversations.AsNoTracking()
                .Include(c => c.Users).Include(c => c.Messages)
                .Where(c => c.Users.Contains(currentUser) && c.Users.Contains(receiver) && c.Type == ConversationType.Private)
                .FirstOrDefaultAsync().Result;
        }

        public async Task<Conversation> InitiateConversation(AppUser currentUser, AppUser receiver)
        {
            var conversation = new Conversation
            {
                Name = $"{currentUser.UserName} {receiver.UserName}",
                Users = new List<AppUser> { currentUser, receiver },
                Messages = new List<Message>(),
                Type = ConversationType.Private,
            };

            var result = await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<Message> AddMessage(AppUser currentUser, int conversationId, string text)
        {
            var message = new Message
            {
                Text = text,
                TimeStamp = DateTime.Now,
                ConversationId = conversationId,
                SenderId = currentUser.Id,
                SenderName = currentUser.UserName,
            };

            var result = await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<List<Message>> GetConversationMessages(int conversationId)
        {
            return await _context.Conversations
                .AsNoTracking()
                .Where(c => c.Id == conversationId)
                .SelectMany(c => c.Messages)
                .OrderBy(m => m.TimeStamp)
                .ToListAsync();
        }

        public async Task<Conversation> InitiateGroupConversation(string name, List<AppUser> members)
        {
            Conversation conversation = new Conversation
            {
                Name = name,
                Users = members,
                Messages = new List<Message>(),
                Type = ConversationType.Group,
            };

            var result = await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<List<Conversation>> GetUserGroups(AppUser currentUser)
        {
            return await _context.Conversations.Where(c => c.Users.Contains(currentUser) && c.Users.Count > 2).ToListAsync();
        }
    }
}
