using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaBe.Model;
using RaBe.RequestModel;
using RaBe.ResponseModel;

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
        [ProducesResponseType(typeof(Lehrer), 401)]
        [ProducesResponseType(typeof(Lehrer), 404)]
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
                    if(lehrer.Blocked == 1)
                    {
                        return this.BadRequest(lehrer);
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
        public IActionResult Logout()
        {
            var lehrer = _context.Lehrer.First(l => l.Token == HttpContext.Session.GetString("JWToken"));

            HttpContext.Session.Clear();
            lehrer.Token = null;
            _context.Lehrer.Update(lehrer);

            return Ok();
        }

        [HttpPost("[action]")]
        public IActionResult ChangePassword(PasswordChangeRequest request)
        {
            var lehrer = _context.Lehrer.First(l => l.Token == HttpContext.Session.GetString("JWToken"));

            if (lehrer == null)
            {
                return Unauthorized();
            }

            using (var sha = SHA256.Create())
            {
                var hash = Encoding.UTF8.GetString(sha.ComputeHash(Encoding.UTF8.GetBytes(request.OldPassword)));

                if (lehrer.Password == hash)
                {
                    lehrer.Password = Encoding.UTF8.GetString(sha.ComputeHash(Encoding.UTF8.GetBytes(request.NewPassword)));
                    lehrer.PasswordGeaendert = 1;

                    _context.Lehrer.Update(lehrer);

                    return Ok();
                }

                return Unauthorized();
            }
        }
    }
}