#region using

using System;
using System.Globalization;
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
        internal const string SALT = "rabe-backend-salt";

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
		public IActionResult Login(LoginRequest request)
		{
            if(request == null)
            {
                return BadRequest();
            }

            var lehrer = _context.Lehrer.FirstOrDefault(l => l.Email.ToLower() == request.email.ToLower());

            if (lehrer == null)
			{
				return NotFound();
			}

            if (HttpContext.Session.GetInt32("fails") >= 3)
            {
                lehrer.Blocked = true;
                _context.Lehrer.Update(lehrer);

                return BadRequest(lehrer);
            }

            using (var sha = SHA256.Create())
			{
				var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(request.password + SALT)));

				if (lehrer.Password == hash)
				{
					if (lehrer.Blocked == true)
					{
						return BadRequest(lehrer);
					}

					lehrer.Token = TokenProvider.GetToken(lehrer);

					_context.Lehrer.Update(lehrer);
					HttpContext.Session.SetString("JWToken", "12");

                    return Ok(lehrer);
				}

                HttpContext.Session.SetInt32("fails", HttpContext.Session.GetInt32("fails") ?? 1);

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

            lehrer.Token = null;
            _context.Lehrer.Update(lehrer);

            HttpContext.Session.Clear();

			return Ok();
		}

		[HttpPost("[action]")]
		[ProducesResponseType(200)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		public IActionResult ChangePassword(PasswordChangeRequest request)
		{
            if(request == null)
            {
                return BadRequest();
            }

            var token = HttpContext.Session.GetString("JWToken");

            return Ok(new { token = token, session = HttpContext.Request.HttpContext.Session.Keys });

            if(token == null)
            {
                return Unauthorized();
            }

            var lehrer = _context.Lehrer.FirstOrDefault(l => l.Token == token);

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

					return Ok();
				}

				return Unauthorized();
			}
		}

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult IsLoggedIn()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if(token == null)
            {
                return Unauthorized();
            }

            return Ok();
        }
	}
}