using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MembersController(IMembersRepository membersRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
    {
        return Ok(await membersRepository.GetMembersAsync());
    }

    [HttpGet("{id}")] // https://localhost:5001/api/members/bob-id
    public async Task<ActionResult<Member>> GetMember(string id)
    {
        var member = await membersRepository.GetMemberAsync(id);

        if (member == null) return NotFound();

        return member.ToResponse();
    }

    [HttpGet("{id}/photos")]
    public async Task<ActionResult<IReadOnlyList<Photo>>> GetPhotos(string id)
    {
        return Ok(await membersRepository.GetPhotosAsync(id));
    }

    [HttpPut]
    public async Task<ActionResult> UpdateMember(MemberUpdateRequest request)
    {
        var memberId = User.GetMemberId();
        
        if (string.IsNullOrEmpty(memberId)) return BadRequest("User ID not found.");
        
        var member = await membersRepository.GetMemberForUpdateAsync(memberId);

        if (member == null) return BadRequest("Member not found.");

        member.DisplayName = request.DisplayName ?? member.DisplayName;
        member.Description = request.Description ?? member.Description;
        member.City = request.City ?? member.City;
        member.Country = request.Country ?? member.Country;

        member.User.DisplayName = request.DisplayName ?? member.User.DisplayName;

        membersRepository.Update(member);

        if (await membersRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update member.");
    }
        
}
