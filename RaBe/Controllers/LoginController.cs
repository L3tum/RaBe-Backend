#region using

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaBe.RequestModel;
using RaBe.ResponseModel;

#endregion

namespace RaBe.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
		internal const string SALT = "rabe-backend-salt";
		private readonly RaBeContext _context;

		public LoginController(RaBeContext context)
		{
			_context = context;
		}

		[AllowAnonymous]
		[HttpPost]
		[ProducesResponseType(typeof(LoginResponse), 200)]
		[ProducesResponseType(typeof(LoginResponse), 400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		public ActionResult<LoginResponse> Login(LoginRequest request)
		{
			if (request == null)
			{
				return BadRequest(new LoginResponse());
			}

			var lehrer = _context.Lehrer.FirstOrDefault(l => l.Email.ToLower() == request.email.ToLower());

			if (lehrer == null)
			{
				return NotFound();
			}

			if (HttpContext.Session.GetInt32("fails") >= 3)
			{
				lehrer.Blocked = true;
				lehrer.Token = null;
				_context.Lehrer.Update(lehrer);

				HttpContext.Session.Clear();

				return BadRequest(LoginResponse.FromTeacher(lehrer));
			}

			using (var sha = SHA256.Create())
			{
				var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(request.password + SALT)));

				if (lehrer.Password == hash)
				{
					if (lehrer.Blocked)
					{
						lehrer.Token = null;

						_context.Lehrer.Update(lehrer);

						return BadRequest(LoginResponse.FromTeacher(lehrer));
					}

					lehrer.Token = TokenProvider.GetToken(lehrer);

					_context.Lehrer.Update(lehrer);

					HttpContext.Session.Clear();

					return Ok(LoginResponse.FromTeacher(lehrer));
				}

				HttpContext.Session.SetInt32("fails", (HttpContext.Session.GetInt32("fails") ?? 0) + 1);

				return Unauthorized();
			}
		}

		[HttpPost("[action]")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public IActionResult Logout()
		{
			var id = long.Parse(User.HasClaim(c => c.Type == "id")
				? User.Claims.First(c => c.Type == "id").Value
				: "-1");

			var lehrer = _context.Lehrer.Find(id);

			if (lehrer == null)
			{
				return NotFound();
			}

			lehrer.Token = null;
			_context.Lehrer.Update(lehrer);

			HttpContext.Session.Clear();

			return Ok();
		}

		[HttpPost("[action]")]
		[ProducesResponseType(200)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		public ActionResult<LoginResponse> ChangePassword(PasswordChangeRequest request)
		{
			if (request == null)
			{
				return BadRequest();
			}

			var id = long.Parse(User.HasClaim(c => c.Type == "id")
				? User.Claims.First(c => c.Type == "id").Value
				: "-1");

			var lehrer = _context.Lehrer.Find(id);

			if (lehrer == null)
			{
				return NotFound();
			}

			using (var sha = SHA256.Create())
			{
				var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(request.oldPassword + SALT)));

				if (lehrer.Password == hash)
				{
					lehrer.Password =
						Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(request.newPassword + SALT)));
					lehrer.PasswordGeaendert = true;

					_context.Lehrer.Update(lehrer);

					return Ok(LoginResponse.FromTeacher(lehrer));
				}

				return Unauthorized();
			}
		}

		[HttpGet]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public IActionResult IsLoggedIn()
		{
			var id = long.Parse(User.HasClaim(c => c.Type == "id")
				? User.Claims.First(c => c.Type == "id").Value
				: "-1");

			var lehrer = _context.Lehrer.Find(id);

			if (lehrer == null)
			{
				return Unauthorized();
			}

			if (lehrer.Blocked)
			{
				return BadRequest();
			}

			return Ok();
		}
	}
}