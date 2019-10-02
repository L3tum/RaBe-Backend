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
		[ProducesResponseType(200)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<IEnumerable<Raum>>> GetAllRooms()
		{
			return Ok(await context.Raum.ToListAsync().ConfigureAwait(false));
		}

        [HttpGet("{roomId}")]
        [ProducesResponseType(typeof(Raum), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Raum>> GetRoom(long roomId)
        {
            var room = await context.Raum.FindAsync(roomId);

            if(room == null)
            {
                return NotFound();
            }

            return Ok(room);
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
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		public IActionResult ModifyRoom(Raum raum)
		{
			if (raum == null)
			{
				return BadRequest();
			}

			if (!TokenProvider.IsAdmin(User))
			{
				return Unauthorized();
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
		[ProducesResponseType(typeof(Raum), 200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public IActionResult AddRoom(Raum raum)
		{
			if (raum == null)
			{
				return BadRequest();
			}

			if (!TokenProvider.IsAdmin(User))
			{
				return Unauthorized();
			}

			context.Raum.Add(raum);

            context.SaveChanges();

            raum = context.Raum.FirstOrDefault(r => r.Name == raum.Name);

			return Ok(raum);
		}

		[HttpDelete("{raumId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		public IActionResult DeleteRoom(int raumId)
		{
			if (!TokenProvider.IsAdmin(User))
			{
				return Unauthorized();
			}

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