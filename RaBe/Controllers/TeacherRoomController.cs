using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaBe.Model;
using RaBe.RequestModel;

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

        // GET: api/LehrerRaum
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LehrerRaum>>> GetLehrerRaum()
        {
            return await _context.LehrerRaum.ToListAsync();
        }

        // GET: api/LehrerRaum/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LehrerRaum>> GetLehrerRaum(long id)
        {
            var lehrerRaum = await _context.LehrerRaum.FindAsync(id);

            if (lehrerRaum == null)
            {
                return NotFound();
            }

            return lehrerRaum;
        }

        // POST: api/LehrerRaum
        [HttpPost]
        public async Task<ActionResult<bool>> PostLehrerRaum(TeacherRoomRequest request)
        {
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
                    return Conflict(false);
                }
                else
                {
                    throw;
                }
            }

            return Ok(true);
        }

        // DELETE: api/LehrerRaum/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteLehrerRaum(long id)
        {
            var lehrerRaum = await _context.LehrerRaum.FindAsync(id);
            if (lehrerRaum == null)
            {
                return NotFound(false);
            }

            _context.LehrerRaum.Remove(lehrerRaum);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        private bool LehrerRaumExists(long id)
        {
            return _context.LehrerRaum.Any(e => e.Id == id);
        }
    }
}
