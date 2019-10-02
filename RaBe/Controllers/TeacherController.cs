#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaBe.Model;
using RaBe.RequestModel;

#endregion

namespace RaBe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly RaBeContext context;

        public TeacherController(RaBeContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<Lehrer>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetAllTeachers()
        {
            if (!TokenProvider.IsAdmin(User))
            {
                return Unauthorized();
            }

            return Ok(await context.Lehrer.ToListAsync().ConfigureAwait(false));
        }

        [HttpGet("{teacherId}")]
        [ProducesResponseType(typeof(Lehrer), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Lehrer>> GetTeacher(long teacherId)
        {
            if (!TokenProvider.IsAdmin(User))
            {
                return Unauthorized();
            }

            var lehrer = await context.Lehrer.FindAsync(teacherId);

            if(lehrer == null)
            {
                return NotFound();
            }

            return Ok(lehrer);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult AddTeacher(AddTeacherRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            if (!TokenProvider.IsAdmin(User))
            {
                return Unauthorized();
            }

            var lehrer = new Lehrer()
            {
                Name = request.name,
                Email = request.email,
                Administrator = request.admin
            };

            using (var sha = SHA256.Create())
            {
                lehrer.Password =
                    Convert.ToBase64String(
                        sha.ComputeHash(Encoding.UTF8.GetBytes(request.password + LoginController.SALT)));
            }

            context.Lehrer.Add(lehrer);

            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult ModifyTeacher(Lehrer lehrer)
        {
            if (lehrer == null)
            {
                return BadRequest();
            }

            if (!TokenProvider.IsAdmin(User))
            {
                return Unauthorized();
            }

            var dbLehrer = context.Lehrer.FirstOrDefault(r => r.Id == lehrer.Id);

            if (dbLehrer == null)
            {
                return NotFound();
            }

            dbLehrer.Name = lehrer.Name;
            dbLehrer.Email = lehrer.Email;
            dbLehrer.Blocked = lehrer.Blocked;
            dbLehrer.Administrator = lehrer.Administrator;

            context.Lehrer.Update(dbLehrer);

            return Ok();
        }

        [HttpDelete("[action]/{teacherId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public IActionResult DeleteTeacher(int teacherId)
        {
            if (!TokenProvider.IsAdmin(User))
            {
                return Unauthorized();
            }

            var lehrer = context.Lehrer.FirstOrDefault(r => r.Id == teacherId);

            if (lehrer == null)
            {
                return NotFound();
            }

            context.Lehrer.Remove(lehrer);

            return Ok();
        }

        [HttpPut("[action]/{teacherId}/{roomId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public IActionResult MarkAsAuthority(int teacherId, int roomId)
        {
            if (!TokenProvider.IsAdmin(User))
            {
                return Unauthorized();
            }

            var lehrer = context.Lehrer.FirstOrDefault(r => r.Id == teacherId);

            if (lehrer == null)
            {
                return NotFound();
            }

            var room = context.Raum.FirstOrDefault(r => r.Id == roomId);

            if (room == null)
            {
                return NotFound();
            }

            var teacherRoom = room.LehrerRaum.FirstOrDefault(l => l.Id == teacherId);

            if (teacherRoom == null)
            {
                teacherRoom = new LehrerRaum();
                teacherRoom.LehrerId = teacherId;
                teacherRoom.RaumId = roomId;
                teacherRoom.Betreuer = true;
                context.LehrerRaum.Add(teacherRoom);
            }
            else
            {
                teacherRoom.Betreuer = true;
                context.LehrerRaum.Update(teacherRoom);
            }

            return Ok();
        }

        [HttpDelete("[action]/{teacherId}/{roomId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public IActionResult RemoveFromAuthority(int teacherId, int roomId)
        {
            if (!TokenProvider.IsAdmin(User))
            {
                return Unauthorized();
            }

            var lehrer = context.Lehrer.FirstOrDefault(r => r.Id == teacherId);

            if (lehrer == null)
            {
                return NotFound();
            }

            var room = context.Raum.FirstOrDefault(r => r.Id == roomId);

            if (room == null)
            {
                return NotFound();
            }

            var teacherRoom = room.LehrerRaum.FirstOrDefault(l => l.Id == teacherId);

            if (teacherRoom == null)
            {
                teacherRoom = new LehrerRaum { LehrerId = teacherId, RaumId = roomId, Betreuer = false };
                context.LehrerRaum.Add(teacherRoom);
            }
            else
            {
                teacherRoom.Betreuer = false;
                context.LehrerRaum.Update(teacherRoom);
            }

            return Ok();
        }
    }
}