using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaBe.Model;

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

        // PUT: api/LehrerRaum/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLehrerRaum(long id, LehrerRaum lehrerRaum)
        {
            if (id != lehrerRaum.Id)
            {
                return BadRequest();
            }

            _context.Entry(lehrerRaum).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LehrerRaumExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/LehrerRaum
        [HttpPost]
        public async Task<ActionResult<LehrerRaum>> PostLehrerRaum(LehrerRaum lehrerRaum)
        {
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
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLehrerRaum", new { id = lehrerRaum.Id }, lehrerRaum);
        }

        // DELETE: api/LehrerRaum/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<LehrerRaum>> DeleteLehrerRaum(long id)
        {
            var lehrerRaum = await _context.LehrerRaum.FindAsync(id);
            if (lehrerRaum == null)
            {
                return NotFound();
            }

            _context.LehrerRaum.Remove(lehrerRaum);
            await _context.SaveChangesAsync();

            return lehrerRaum;
        }

        private bool LehrerRaumExists(long id)
        {
            return _context.LehrerRaum.Any(e => e.Id == id);
        }
    }
}
