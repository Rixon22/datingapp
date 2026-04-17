using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MessagesController(
    IMessagesRepository messagesRepository,
    IMembersRepository membersRepository) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageResponse>> CreateMessage(MessageRequest request)
    {
        var sender = await membersRepository.GetMemberAsync(User.GetMemberId());
        var recipient = await membersRepository.GetMemberAsync(request.RecipientId);

        if (recipient == null || sender == null || sender.Id == request.RecipientId)
        {
            return BadRequest("Unable to send the message");
        }

        var message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = request.Content
        };

        messagesRepository.Add(message);

        if (await messagesRepository.SaveAllAsync())
        {
            return message.ToResponse();
        }

        return BadRequest("Failed to send the message");
    }


    [HttpGet]
    public async Task<ActionResult<PaginationResult<MessageResponse>>> GetMessagesByContainer(
        [FromBody] MessageParams messageParams)
    {
        messageParams.MemberId = User.GetMemberId();

        return await messagesRepository.GetForMember(messageParams);
    }

    [HttpGet("thread/{recipientId}")]
    public async Task<ActionResult<IReadOnlyList<MessageResponse>>> GetThread(string recipientId)
    {
        return Ok(await messagesRepository.GetThread(User.GetMemberId(), recipientId));
    }
}