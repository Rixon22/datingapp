using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessagesRepository
{
   void Add(Message message);
   void Delete(Message message);
   Task<Message?> GetAsync(string messageId);
   Task<PaginationResult<MessageResponse>> GetForMemberAsync(MessageParams messageRequest);
   Task<IReadOnlyList<MessageResponse>> GetThreadAsync(string currentMemberId, string recipientId);
   Task<bool> SaveAllAsync();
   
   void AddGroup(Group group);
   Task RemoveConnectionAsync(string connectionId);
   Task<Connection?> GetConnectionAsync(string  connectionId);
   Task<Group?> GetMessageGroupAsync(string groupName);
   Task<Group?> GetGroupForConnectionAsync(string connectionId);

}