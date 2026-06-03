using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class MessageHub(IMessagesRepository messagesRepository,
    IMembersRepository membersRepository,
    IHubContext<PresenceHub> presenceHub) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["userId"].ToString() ?? throw new HubException("Other user not found");
        var groupName = GetGroupName(GetUserId(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToGroupAsync(groupName);

        var messages = await messagesRepository.GetThreadAsync(GetUserId(), otherUser);
        await Clients.Group(groupName).SendAsync("ReceivedMessageThread", messages);
    }

    public async Task SendMessage(MessageRequest request)
    {
        var sender = await membersRepository.GetMemberAsync(GetUserId());
        var recipient = await membersRepository.GetMemberAsync(request.RecipientId);

        if (recipient == null || sender == null || sender.Id == request.RecipientId)
        {
            throw new HubException("Unable to send the message");
        }

        var message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = request.Content
        };

        var groupName = GetGroupName(sender.Id, recipient.Id);
        var group = await messagesRepository.GetMessageGroupAsync(groupName);
        var userInGroup = group != null && group.Connections.Any(c => c.UserId == message.RecipientId);

        if (userInGroup)
        {
            message.DateRead = DateTime.UtcNow;
        }

        messagesRepository.Add(message);

        if (await messagesRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", message.ToResponse());
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.Id);
            if (connections != null && connections.Count > 0 && !userInGroup)
            {
                await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", message.ToResponse());
            }
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await messagesRepository.RemoveConnectionAsync(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    private async Task<bool> AddToGroupAsync(string groupName)
    {
        var group = await messagesRepository.GetMessageGroupAsync(groupName);
        var connection = new Connection(Context.ConnectionId, GetUserId());

        if (group == null)
        {
            group = new Group(groupName);
            messagesRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        return await messagesRepository.SaveAllAsync();
    }

    private static string GetGroupName(string? caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private string GetUserId()
    {
        return Context.User?.GetMemberId() ?? throw new HubException("Cannot get member id");
    }
}