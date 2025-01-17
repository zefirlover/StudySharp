﻿using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using StudySharp.ApplicationServices.JwtAuthService;
using StudySharp.ApplicationServices.JwtAuthService.ResultModels;
using StudySharp.Domain.Constants;
using StudySharp.Domain.General;
using StudySharp.DomainServices;

namespace StudySharp.ApplicationServices.Commands.Auth
{
    public sealed class LoginCommand : IRequest<OperationResult<LoginResult>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, OperationResult<LoginResult>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(UserManager<ApplicationUser> userManager, IJwtService jwtAuthManager)
        {
            _userManager = userManager;
            _jwtService = jwtAuthManager;
        }

        public async Task<OperationResult<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Email);
            if (user == null)
            {
                return OperationResult.Fail<LoginResult>(ErrorConstants.InvalidCredentials);
            }

            var isSucceeded = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isSucceeded)
            {
                return OperationResult.Fail<LoginResult>(ErrorConstants.InvalidCredentials);
            }

            var jwtResult = _jwtService.GenerateTokens(user.UserName, new[] { new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName) }, DateTime.Now);

            return OperationResult.Ok(new LoginResult
            {
                UserName = user.UserName,
                Role = "Test",
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString,
            });
        }
    }
}
