using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Authorization;
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
        [HttpGet("[action]/{roomId}")]
        public async Task<ActionResult<IEnumerable<Fehler>>> GetAllErrorsOfRoom(int roomId)
        {
            return await _context.Fehler.Where(f => f.Arbeitsplatz.RaumId == roomId).ToListAsync();
        }

        [HttpGet("[action]/{workplaceId}")]
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

            var workplace = _context.Arbeitsplatz.Find(fehler.ArbeitsplatzId);
            var category = _context.Kategorie.Find(fehler.KategorieId);
            var lehrer = await _context.LehrerRaum.Where(lr => lr.RaumId == workplace.RaumId).ToListAsync();

            var ok = await SendErrorReportEmail(lehrer.Select(l => new Address(l.Lehrer.Email, l.Lehrer.Name)).ToList(), workplace.Raum.Name, fehler.Titel, category.Name, workplace.Name, fehler.Status);

            if (!ok)
            {
                return this.StatusCode(500, "Email sending not ok");
            }

            return CreatedAtAction("GetFehler", new { id = fehler.Id }, fehler);
        }

        private bool FehlerExists(long id)
        {
            return _context.Fehler.Any(e => e.Id == id);
        }

        private async Task<bool> SendErrorReportEmail(List<Address> toAdresses, string roomName, string title, string category, string workplace, long status)
        {
            var email = Email.From(Environment.GetEnvironmentVariable("EMAIL_SENDER"))
                .To(toAdresses)
                .Subject($"Problem in Raum {roomName}")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Templates/ErrorReport.cshtml", 
                new { 
                    RaumName = roomName,
                    Titel = title,
                    Kategorie = category,
                    Arbeitsplatz = workplace,
                    Status = status
                });

            var response = await email.SendAsync();

            return response.Successful;
        }
    }
}
