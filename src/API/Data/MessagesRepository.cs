using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Mappers;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessagesRepository(AppDbContext context) : IMessagesRepository
{
    public void Add(Message message) => context.Messages.Add(message);

    public void AddGroup(Group group) => context.Groups.Add(group);

    public void Delete(Message message) => context.Messages.Remove(message);

    public async Task<Message?> GetAsync(string messageId) => await context.Messages.FindAsync(messageId);

    public async Task<Connection?> GetConnectionAsync(string connectionId) => await context.Connections.FindAsync(connectionId);

    public async Task<PaginationResult<MessageResponse>> GetForMemberAsync(MessageParams messageParams)
    {
        var query = context.Messages
            .OrderByDescending(m => m.MessageSent)
            .AsQueryable();

        query = messageParams.Container switch
        {
            ContainerTypes.Outbox => query.Where(m => m.SenderId == messageParams.MemberId && !m.SenderDeleted),
            _ => query.Where(m => m.RecipientId == messageParams.MemberId && !m.RecipientDeleted)
        };

        var messageQuery = query.Select(MessageMapper.ToResponseProjection());

        return await Pagination.CreateAsync(messageQuery, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<Group?> GetGroupForConnectionAsync(string connectionId)
        => await context.Groups
            .Include(g => g.Connections)
            .Where(g => g.Connections.Any(c => c.ConnectionId == connectionId))
            .FirstOrDefaultAsync();

    public async Task<Group?> GetMessageGroupAsync(string groupName)
        => await context.Groups
            .Include(g => g.Connections)
            .FirstOrDefaultAsync(g => g.Name == groupName);

    public async Task<IReadOnlyList<MessageResponse>> GetThreadAsync(string currentMemberId, string recipientId)
    {
        await context.Messages
            .Where(m => m.RecipientId == currentMemberId
                && m.SenderId == recipientId
                && m.DateRead == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.DateRead, DateTime.UtcNow));

        return await context.Messages
            .Where(m => (m.RecipientId == currentMemberId && !m.RecipientDeleted && m.SenderId == recipientId)
                || (m.RecipientId == recipientId && !m.SenderDeleted && m.SenderId == currentMemberId))
            .OrderBy(m => m.MessageSent)
            .Select(MessageMapper.ToResponseProjection())
            .ToListAsync();
    }

    public async Task RemoveConnectionAsync(string connectionId)
        => await context.Connections
            .Where(c => c.ConnectionId == connectionId)
            .ExecuteDeleteAsync();

    public async Task<bool> SaveAllAsync() => await context.SaveChangesAsync() > 0;
}