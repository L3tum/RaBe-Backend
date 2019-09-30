#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
		public async Task<IActionResult> GetAllTeachers()
		{
			return Ok(await context.Lehrer.ToListAsync());
		}

		[HttpPost]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public IActionResult AddTeacher(Lehrer lehrer)
		{
			if (lehrer == null)
			{
				return BadRequest();
			}

			using (var sha = SHA256.Create())
			{
				lehrer.Password = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(lehrer.Password + LoginController.SALT)));
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

			var dbLehrer = context.Lehrer.FirstOrDefault(r => r.Id == lehrer.Id);

			if (dbLehrer == null)
			{
				return NotFound();
			}

			lehrer.Password = dbLehrer.Password;

			context.Lehrer.Update(lehrer);

			return Ok();
		}

		[HttpDelete("[action]/{teacherId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public IActionResult DeleteTeacher(int teacherId)
		{
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
		[ProducesResponseType(404)]
		public IActionResult MarkAsAuthority(int teacherId, int roomId)
		{
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
				teacherRoom.Betreuer = 1;
				context.LehrerRaum.Add(teacherRoom);
			}
			else
			{
				teacherRoom.Betreuer = 1;
				context.LehrerRaum.Update(teacherRoom);
			}

			return Ok();
		}

		[HttpDelete("[action]/{teacherId}/{roomId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public IActionResult RemoveFromAuthority(int teacherId, int roomId)
		{
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
				teacherRoom.Betreuer = 0;
				context.LehrerRaum.Add(teacherRoom);
			}
			else
			{
				teacherRoom.Betreuer = 0;
				context.LehrerRaum.Update(teacherRoom);
			}

			return Ok();
		}

		[HttpPost("[action]")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public IActionResult BlockLock(BlockLockRequest request)
		{
			var lehrer = context.Lehrer.FirstOrDefault(l => l.Email == request.email);

			if (lehrer == null)
			{
				return NotFound();
			}

			lehrer.Blocked = 1;

			context.Lehrer.Update(lehrer);

			return Ok();
		}
	}
}