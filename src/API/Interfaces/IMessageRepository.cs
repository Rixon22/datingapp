using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessagesRepository
{
   void Add(Message message);
   void Delete(Message message);
   Task<Message?> Get(string messageId);
   Task<PaginationResult<MessageResponse>> GetForMember();
   Task<IReadOnlyList<MessageResponse>> GetThread(string currentMemberId, string recipientId);
   Task<bool> SaveAllAsync(); 
}