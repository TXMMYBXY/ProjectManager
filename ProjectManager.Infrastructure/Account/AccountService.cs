using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Account;
using ProjectManager.Application.Account.Dto;
using ProjectManager.Application.Common.Exceptions;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Infrastructure.Account;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;
    private readonly UserManager<Entities.Models.Employee> _userManager;
    private readonly SignInManager<Entities.Models.Employee> _signInManager;

    public AccountService(
        ILogger<AccountService> logger, 
        IMapper mapper,
        IJwtService jwtService,
        UserManager<Entities.Models.Employee> userManager,
        SignInManager<Entities.Models.Employee> signInManager)
    {
        _logger = logger;
        _mapper = mapper;
        _jwtService = jwtService;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    
    public async Task<RegisterResultDto> RegisterAsync(RegisterDto dto)
    {
        ConflictException.ThrowIf(!dto.Password.Equals(dto.ConfirmPassword), "Passwords do not match");
        ConflictException.ThrowIf(await _userManager.FindByEmailAsync(dto.Email) != null, "Email already exists");
        
        var employee = _mapper.Map<Entities.Models.Employee>(dto);
        
        employee.UserName = dto.UserName;
        
        var result = await _userManager.CreateAsync(employee, dto.Password);    
        
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        
        await _userManager.AddToRoleAsync(employee, Enum.GetName(UserRole.Employee));
        
        _logger.LogInformation("New account created successfully");
        
        var token = _jwtService.GenerateToken(employee);

        return new RegisterResultDto
        {
            Id = employee.Id,
            Token = token
        };
    }

    public async Task<LoginResultDto> LoginAsync(LoginDto dto)
    {
        var employee = await _userManager.FindByEmailAsync(dto.Email);
        var passwordMatch = _signInManager.UserManager
            .PasswordHasher.VerifyHashedPassword(employee, employee.PasswordHash, dto.Password);

        ConflictException.ThrowIf(passwordMatch == PasswordVerificationResult.Failed, 
            "Invalid email or password");
        
        var token = _jwtService.GenerateToken(employee);

        return new LoginResultDto
        {
            Id = employee.Id,
            Token = token
        };
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}