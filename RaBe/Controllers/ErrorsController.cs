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
    public class ErrorsController : ControllerBase
    {
        private readonly RaBeContext _context;

        public ErrorsController(RaBeContext context)
        {
            _context = context;
        }

        // GET: api/Errors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fehler>>> GetFehler()
        {
            return await _context.Fehler.ToListAsync();
        }

        // GET
        [HttpGet("[method]/{roomId}")]
        public async Task<ActionResult<IEnumerable<Fehler>>> GetAllErrorsOfRoom(int roomId)
        {
            return await _context.Fehler.Where(f => f.Arbeitsplatz.RaumId == roomId).ToListAsync();
        }

        [HttpGet("[method]/{workplaceId}")]
        public async Task<ActionResult<IEnumerable<Fehler>>> GetAllErrorsOfWorkplace(int workplaceId)
        {
            return await _context.Fehler.Where(f => f.ArbeitsplatzId == workplaceId).ToListAsync();
        }

        // GET: api/Errors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Fehler>> GetFehler(long id)
        {
            var fehler = await _context.Fehler.FindAsync(id);

            if (fehler == null)
            {
                return NotFound();
            }

            return fehler;
        }

        // PUT: api/Errors/5/2
        [HttpPut("{id}/{status}")]
        public async Task<ActionResult<Fehler>> ResolveError(long id, int status)
        {
            var fehler = await _context.Fehler.FindAsync(id);

            if (fehler == null)
            {
                return NotFound();
            }

            if(status > 2)
            {
                return BadRequest();
            }

            fehler.Status = status;

            _context.Fehler.Update(fehler);

            return fehler;
        }

        // DELETE: api/Errors/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Fehler>> ResolveError(long id)
        {
            var fehler = await _context.Fehler.FindAsync(id);

            if (fehler == null)
            {
                return NotFound();
            }

            fehler.Status = 0;

            _context.Fehler.Update(fehler);

            return fehler;
        }

        // POST: api/Errors
        [HttpPost]
        public async Task<ActionResult<Fehler>> ReportError(Fehler fehler)
        {
            _context.Fehler.Add(fehler);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FehlerExists(fehler.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFehler", new { id = fehler.Id }, fehler);
        }

        private bool FehlerExists(long id)
        {
            return _context.Fehler.Any(e => e.Id == id);
        }
    }
}
