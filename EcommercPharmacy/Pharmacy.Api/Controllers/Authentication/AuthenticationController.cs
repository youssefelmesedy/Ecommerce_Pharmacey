using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.Application.Dtos.Authentication;
using Pharmacy.Application.Features.Authentication.EmailVerification;
using Pharmacy.Application.Features.Authentication.ForgotPassword;
using Pharmacy.Application.Features.Authentication.Login;
using Pharmacy.Application.Features.Authentication.Logout;
using Pharmacy.Application.Features.Authentication.RefreshToken;
using Pharmacy.Application.Features.Authentication.Registration;
using Pharmacy.Application.Features.Authentication.ResetPassword;
using Pharmacy.Application.Features.Authentication.SendEmailVerification;

namespace Pharmacy.Api.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto, CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand(dto, HttpContext.Connection.RemoteIpAddress?.ToString());

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Succeeded || result.Data is null)
                return BadRequest(result);

            if (result.Data.RefreshToken is not null)
                SetRefreshTokenInCookie(result.Data.RefreshToken, result.Data.ExpiresRefreshToken);

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(dto, HttpContext.Connection.RemoteIpAddress?.ToString());

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Succeeded || result.Data is null)
                return BadRequest(result);

            if (result.Data.RefreshToken is not null)
                SetRefreshTokenInCookie(result.Data.RefreshToken, result.Data.ExpiresRefreshToken);

            return Ok(result);
        }
        [Authorize]
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            var token = Request.Cookies["refreshToken"];
            if(token == null)
                return NotFound("Refresh token not found");

            var command = new RefreshTokenCommand(token, "192.168.1.1");

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Succeeded || result.Data is null)
                return BadRequest(result);

            if (result.Data.Token is not null)
                SetRefreshTokenInCookie(result.Data.Token, result.Data.ExpiresAtUtc);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var token = Request.Cookies["refreshToken"];
            if (token == null)
                return NotFound("Refresh token not found");

            var command = new LogoutCommand(token, "192.168.1.1");

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Succeeded || result.Data is null)
                return BadRequest(result);

           SetRefreshTokenInCookie(string.Empty, default);

            return Ok(result);
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto, CancellationToken cancellationToken)
        {
            var command = new ForgotPasswordCommand(dto);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(result);


            return Ok(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto, CancellationToken cancellationToken)
        {
            var command = new ResetPasswordCommand(dto);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(result);


            return Ok(result);
        }

        [HttpGet("SendEmailVerification")]
        public async Task<IActionResult> SendEmailVerification([FromBody] SendEmailVerificationDto dto, CancellationToken cancellationToken)
        {
            var command = new SendEmailVerificationCommand(dto);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(result);


            return Ok(result);
        }

        [HttpPost("EmailVerification")]
        public async Task<IActionResult> EmailVerification([FromBody] EmailVerificationDto dto, CancellationToken cancellationToken)
        {
            var command = new EmailVerificationcommand(dto);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(result);


            return Ok(result);
        }

        private void SetRefreshTokenInCookie(string token, DateTime expiresAt)
        {
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Expires = expiresAt.ToLocalTime(),
                SameSite = SameSiteMode.Strict,
                Secure = true
            };
            Response.Cookies.Append("refreshToken", token, cookieOption);
        }
    }
}
