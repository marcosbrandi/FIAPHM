﻿using FIAPHM.Models;
using FIAPHM.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FIAPHM.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtSettings _jwtSettings;
    public AuthController(AuthService authService, IOptions<JwtSettings> jwtSettings)
    {
        _authService = authService;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var role = await _authService.Authenticate(request.Email, request.Password);
        if (role == null) return Unauthorized();

        var token = GenerateJwtToken(request.Email, role);
        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(string email, string role)
    {
        //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            //issuer: _configuration["Jwt:Issuer"],
            //audience: _configuration["Jwt:Audience"],
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: new[] { new Claim(ClaimTypes.Email, email), new Claim(ClaimTypes.Role, role) },
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}