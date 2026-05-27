using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// Account controller.
/// </summary>
public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService) : BaseApiController
{
    /// <summary>
    /// Creates an User.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>A newly created account.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST api/account/register
    ///     {
    ///        "displayName": "displayName",
    ///        "email": "test@test.com",
    ///        "password": "password"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
    {
        var user = new AppUser
        {
            DisplayName = request.DisplayName,
            Email = request.Email,
            UserName = request.Email,
            Member = new Member
            {
                DisplayName = request.DisplayName,
                Gender = request.Gender,
                City = request.City,
                Country = request.Country,
                BirthDay = request.BirthDay
            }
        };
        
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("identity", error.Description);
            }

            return ValidationProblem();
        }

        return await user.ToDto(tokenService);
    }

    /// <summary>
    /// Login as an User.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Valid user token.</returns>
    /// <response code="200">Returns the valid user token</response>
    /// <response code="401">If the username or password are incorrect</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null) return Unauthorized("Invalid email or password");
        
        var result = await userManager.CheckPasswordAsync(user, request.Password);

        if (!result) return Unauthorized("Invalid username or password");

        return await user.ToDto(tokenService);
    }
}
