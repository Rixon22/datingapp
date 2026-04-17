using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Mappers;

namespace API.Data;

public class MessagesRepository(AppDbContext context) : IMessagesRepository
{
    public void Add(Message message) => context.Messages.Add(message);

    public void Delete(Message message) => context.Messages.Remove(message);

    public async Task<Message?> Get(string messageId) => await context.Messages.FindAsync(messageId);

    public async Task<PaginationResult<MessageResponse>> GetForMember(MessageParams messageParams)    {
        
        var query = context.Messages
            .OrderByDescending(m => m.MessageSent)
            .AsQueryable();
        
        query = messageParams.Container switch
        {
            ContainerTypes.Outbox => query.Where(m => m.SenderId == messageParams.MemberId),
            _ => query.Where(m => m.RecipientId == messageParams.MemberId)
        };

        var messageQuery = query.Select(MessageMapper.ToResponseProjection());

        return await Pagination.CreateAsync(messageQuery, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IReadOnlyList<MessageResponse>> GetThread(string currentMemberId, string recipientId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveAllAsync() => await context.SaveChangesAsync() > 0;
}