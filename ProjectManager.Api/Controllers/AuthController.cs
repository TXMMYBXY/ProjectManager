using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.Features.Account.Requests;
using ProjectManager.Api.Features.Account.Responses;
using ProjectManager.Application.Account;
using ProjectManager.Application.Account.Dto;

namespace ProjectManager.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;
    
    public AuthController(IMapper mapper, IAccountService accountService)
    {
        _mapper = mapper;
        _accountService = accountService;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        var requestDto = _mapper.Map<RegisterDto>(request);
        
        var responseDto = await _accountService.RegisterAsync(requestDto);

        var response = _mapper.Map<RegisterResponse>(responseDto);
        
        return Ok(response);
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var requestDto = _mapper.Map<LoginDto>(request);
        
        var responseDto = await _accountService.LoginAsync(requestDto);

        var response = _mapper.Map<LoginResponse>(responseDto);
        
        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        await _accountService.LogoutAsync();
        
        return Ok();
    }
}