using HealthCare.Claims.Monolith.Services;
using HealthCare.Claims.Monolith.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Claims.Monolith.Controllers;

[ApiController]
[Route("api/members")]
public sealed class MembersController(MemberService members) : ControllerBase
{
    [HttpGet]
    public IActionResult GetMembers() => Ok(members.GetMembers());

    [HttpPost]
    public IActionResult CreateMember(CreateMemberRequest request)
    {
        try
        {
            return CreatedAtAction(nameof(GetMembers), members.CreateMember(request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{memberId}")]
    public IActionResult UpdateMember(string memberId, UpdateMemberRequest request)
    {
        try
        {
            return Ok(members.UpdateMember(memberId, request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{memberId}/deactivate")]
    public IActionResult DeactivateMember(string memberId)
    {
        try
        {
            return Ok(members.DeactivateMember(memberId));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
