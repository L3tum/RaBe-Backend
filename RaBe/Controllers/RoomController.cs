#region using

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaBe.Model;

#endregion

namespace RaBe.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RoomController : ControllerBase
	{
		private readonly RaBeContext context;

		public RoomController(RaBeContext context)
		{
			this.context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Raum>>> GetAllRooms()
		{
			return Ok(await context.Raum.ToListAsync().ConfigureAwait(false));
		}

		[HttpGet("[action]/{teacherId}")]
		[ProducesResponseType(typeof(IList<Raum>), 200)]
		public IActionResult GetAllRoomsOfTeacher(int teacherId)
		{
			return Ok(context.Raum.Where(r => r.LehrerRaum.Any(lr => lr.LehrerId == teacherId)));
		}

		[HttpGet("[action]/{raumId}")]
		[ProducesResponseType(typeof(IList<Arbeitsplatz>), 200)]
		public IActionResult GetAllWorkplacesOfRoom(int raumId)
		{
			return Ok(context.Arbeitsplatz.Where(a => a.RaumId == raumId));
		}

		[HttpPut]
		[ProducesResponseType(200)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		public IActionResult ModifyRoom(Raum raum)
		{
			if (raum == null)
			{
				return BadRequest();
			}

			var dbRaum = context.Raum.FirstOrDefault(r => r.Id == raum.Id);

			if (dbRaum == null)
			{
				return NotFound();
			}

			context.Raum.Update(raum);

			return Ok();
		}

		[HttpPost]
		[ProducesResponseType(200)]
		[ProducesResponseType(401)]
		public IActionResult AddRoom(Raum raum)
		{
			if (raum == null)
			{
				return BadRequest();
			}

			context.Raum.Add(raum);

			return Ok();
		}

		[HttpDelete("{raumId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public IActionResult DeleteRoom(int raumId)
		{
			var raum = context.Raum.FirstOrDefault(r => r.Id == raumId);

			if (raum == null)
			{
				return NotFound();
			}

			context.Raum.Remove(raum);

			return Ok();
		}
	}
}