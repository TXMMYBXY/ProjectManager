using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.Features.Account.Requests;
using ProjectManager.Api.Features.Account.Responses;
using ProjectManager.Application.Account;
using ProjectManager.Application.Account.Dto;

namespace ProjectManager.Api.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;
    
    public AccountController(IMapper mapper, IAccountService accountService)
    {
        _mapper = mapper;
        _accountService = accountService;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        var requestDto = _mapper.Map<RegisterDto>(request);
        
        var response = await _accountService.RegisterAsync(requestDto);
        
        return Ok(response.Token);
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var requestDto = _mapper.Map<LoginDto>(request);
        
        var response = await _accountService.LoginAsync(requestDto);
        
        return Ok(response.Token);
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await _accountService.LogoutAsync();
        
        return Ok();
    }
}