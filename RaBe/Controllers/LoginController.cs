#region using

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using RaBe.Model;
using RaBe.RequestModel;

#endregion

namespace RaBe.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
		private readonly RaBeContext _context;

		public LoginController(RaBeContext context)
		{
			_context = context;
		}

		[AllowAnonymous]
		[HttpPost]
		[ProducesResponseType(typeof(Lehrer), 200)]
		[ProducesResponseType(typeof(Lehrer), 400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		public ActionResult<Lehrer> Login(LoginRequest request)
		{
			var lehrer = _context.Lehrer.FirstOrDefault(l => l.Email.ToLower() == request.email.ToLower());
			var salt = Environment.GetEnvironmentVariable("RABE_SALT") ?? "rabe-backend-salt";

			if (lehrer == null)
			{
				return NotFound();
			}

			using (var sha = SHA256.Create())
			{
				var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(request.password + salt)));

				if (lehrer.Password == hash)
				{
					if (lehrer.Blocked == 1)
					{
						return BadRequest(lehrer);
					}

					lehrer.Token = TokenProvider.GetToken(lehrer).ToString();

					_context.Lehrer.Update(lehrer);
					HttpContext.Session.SetString("JWToken", lehrer.Token);

					return Ok(lehrer);
				}

				return Unauthorized();
			}
		}

		[HttpPost("[action]")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public IActionResult Logout()
		{
			var lehrer = _context.Lehrer.FirstOrDefault(l => l.Token == HttpContext.Session.GetString("JWToken"));

			if (lehrer == null)
			{
				return NotFound();
			}

			HttpContext.Session.Clear();
			lehrer.Token = null;
			_context.Lehrer.Update(lehrer);

			return Ok();
		}

		[HttpPost("[action]")]
		[ProducesResponseType(200)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		public IActionResult ChangePassword(PasswordChangeRequest request)
		{
			var lehrer = _context.Lehrer.First(l => l.Token == HttpContext.Session.GetString("JWToken"));

			if (lehrer == null)
			{
				return NotFound();
			}

			using (var sha = SHA256.Create())
			{
				var hash = Encoding.UTF8.GetString(sha.ComputeHash(Encoding.UTF8.GetBytes(request.oldPassword)));

				if (lehrer.Password == hash)
				{
					lehrer.Password =
						Encoding.UTF8.GetString(sha.ComputeHash(Encoding.UTF8.GetBytes(request.newPassword)));
					lehrer.PasswordGeaendert = 1;

					_context.Lehrer.Update(lehrer);

					return Ok();
				}

				return Unauthorized();
			}
		}
	}
}