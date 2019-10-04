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
	public class TeacherRoomController : ControllerBase
	{
		private readonly RaBeContext _context;

		public TeacherRoomController(RaBeContext context)
		{
			_context = context;
		}

		[HttpGet("{roomId}/{teacherId}")]
		[ProducesResponseType(typeof(LehrerRaum), 200)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		public ActionResult<LehrerRaum> GetLehrerRaum(int roomId, int teacherId)
		{
			if (!TokenProvider.IsAdmin(User))
			{
				return Unauthorized();
			}

			var teacherRoom = _context.LehrerRaum.FirstOrDefault(lr => lr.RaumId == roomId && lr.LehrerId == teacherId);

			if (teacherRoom == null)
			{
				return NotFound();
			}

			return Ok(teacherRoom);
		}

		[HttpGet("[action]/{roomId}")]
		[ProducesResponseType(typeof(List<LehrerRaum>), 200)]
		[ProducesResponseType(401)]
		public ActionResult<List<LehrerRaum>> GetAuthorities(int roomId)
		{
			if (!TokenProvider.IsAdmin(User))
			{
				return Unauthorized();
			}

			var teacherRooms = _context.LehrerRaum.Where(lr => lr.RaumId == roomId && lr.Betreuer);

			return Ok(teacherRooms);
		}

		[HttpPost("[action]/{roomId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public IActionResult UpdateAuthorities(int roomId, List<LehrerRaum> authorities)
		{
			if (!TokenProvider.IsAdmin(User))
			{
				return Unauthorized();
			}

			if (authorities == null)
			{
				return BadRequest();
			}

			if (authorities.Any(a => a.RaumId != roomId))
			{
				return BadRequest();
			}

			var dbAuthorities = _context.LehrerRaum.Where(lr => lr.RaumId == roomId);

			foreach (var authority in authorities)
			{
				var dbAuthority = dbAuthorities.FirstOrDefault(lr =>
					lr.LehrerId == authority.LehrerId && lr.RaumId == authority.RaumId);

				if (dbAuthority != null)
				{
					if (!dbAuthority.Betreuer)
					{
						dbAuthority.Betreuer = true;
						_context.LehrerRaum.Update(dbAuthority);
					}
				}
				else
				{
					dbAuthority = new LehrerRaum
					{
						RaumId = authority.RaumId,
						Betreuer = true,
						LehrerId = authority.LehrerId
					};

					_context.LehrerRaum.Add(dbAuthority);
				}
			}

			foreach (var dbAuthority in dbAuthorities)
			{
				if (!authorities.Any(a => a.LehrerId == dbAuthority.LehrerId && a.RaumId == dbAuthority.RaumId))
				{
					_context.LehrerRaum.Remove(dbAuthority);
				}
			}

			return Ok();
		}

		// GET: api/LehrerRaum
		[HttpGet]
		[ProducesResponseType(typeof(LehrerRaum), 200)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<IEnumerable<LehrerRaum>>> GetLehrerRaum()
		{
			if (!TokenProvider.IsAdmin(User))
			{
				return Unauthorized();
			}

			return Ok(await _context.LehrerRaum.ToListAsync().ConfigureAwait(false));
		}

		// GET: api/LehrerRaum/5
		[HttpGet("{id}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<LehrerRaum>> GetLehrerRaum(long id)
		{
			if (!TokenProvider.IsAdmin(User))
			{
				return Unauthorized();
			}

			var lehrerRaum = await _context.LehrerRaum.FindAsync(id);

			if (lehrerRaum == null)
			{
				return NotFound();
			}

			return Ok(lehrerRaum);
		}

		// DELETE: api/LehrerRaum/5
		[HttpDelete("{id}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		public async Task<ActionResult> DeleteLehrerRaum(long id)
		{
			if (!TokenProvider.IsAdmin(User))
			{
				return Unauthorized();
			}

			var lehrerRaum = await _context.LehrerRaum.FindAsync(id);

			if (lehrerRaum == null)
			{
				return NotFound();
			}

			_context.LehrerRaum.Remove(lehrerRaum);
			await _context.SaveChangesAsync();

			return Ok();
		}

		private bool LehrerRaumExists(long id)
		{
			return _context.LehrerRaum.Any(e => e.Id == id);
		}
	}
}