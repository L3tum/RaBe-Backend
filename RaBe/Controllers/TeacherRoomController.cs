#region using

using System.Collections.Generic;
using System.Linq;
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
        public ActionResult<LehrerRaum> GetBetreuer(int roomId)
        {
            if (!TokenProvider.IsAdmin(User))
            {
                return Unauthorized();
            }

            var teacherRoom = _context.LehrerRaum.FirstOrDefault(lr => lr.RaumId == roomId && lr.Betreuer == true);

            if (teacherRoom == null)
            {
                return NotFound();
            }

            return Ok(teacherRoom);
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

        // POST: api/LehrerRaum
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult> PostLehrerRaum(TeacherRoomRequest request)
        {
            if (!TokenProvider.IsAdmin(User))
            {
                return Unauthorized();
            }

            if (await _context.Lehrer.FindAsync(request.teacherId) == null ||
                await _context.Raum.FindAsync(request.roomId) == null)
            {
                return NotFound();
            }

            if (await _context.LehrerRaum.FirstOrDefaultAsync(lr =>
                    lr.LehrerId == request.teacherId && lr.RaumId == request.roomId) != null)
            {
                return Conflict();
            }

            var lehrerRaum = new LehrerRaum();
            lehrerRaum.LehrerId = request.teacherId;
            lehrerRaum.RaumId = request.roomId;
            lehrerRaum.Betreuer = request.betreuer ? 1 : 0;

            _context.LehrerRaum.Add(lehrerRaum);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LehrerRaumExists(lehrerRaum.Id))
                {
                    return Conflict();
                }

                throw;
            }

            return Ok();
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