#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaBe.Model;
using RaBe.ResponseModel;

#endregion

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
		public async Task<ActionResult<AllErrorsResponse>> GetFehler()
		{
			var response = new AllErrorsResponse();

			var errors = await _context.Fehler.ToListAsync().ConfigureAwait(false);

			foreach (var error in errors)
			{
				if (!response.rooms.ContainsKey(error.Arbeitsplatz.RaumId))
				{
					response.rooms.Add(error.Arbeitsplatz.RaumId,
						new ErrorsResponse
							{roomId = error.Arbeitsplatz.RaumId, roomName = error.Arbeitsplatz.Raum.Name});
				}

				response.rooms[error.Arbeitsplatz.RaumId].errors.Add(error);
			}

			return Ok(response);
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
		[ProducesResponseType(typeof(Fehler), 200)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<Fehler>> GetFehler(long id)
		{
			var fehler = await _context.Fehler.FindAsync(id);

			if (fehler == null)
			{
				return NotFound();
			}

			return Ok(fehler);
		}

		// PUT: api/Errors/5/2
		[HttpPut("{id}/{status}")]
		[ProducesResponseType(typeof(Fehler), 200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<Fehler>> ResolveError(long id, int status)
		{
			if (status > 2)
			{
				return BadRequest();
			}

			var fehler = await _context.Fehler.FindAsync(id);

			if (fehler == null)
			{
				return NotFound();
			}

			fehler.Status = status;

			_context.Fehler.Update(fehler);

			return Ok(fehler);
		}

		// DELETE: api/Errors/5
		[HttpDelete("{id}")]
		[ProducesResponseType(typeof(Fehler), 200)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<Fehler>> ResolveError(long id)
		{
			var fehler = await _context.Fehler.FindAsync(id);

			if (fehler == null)
			{
				return NotFound();
			}

			fehler.Status = 0;

			_context.Fehler.Update(fehler);

			return Ok(fehler);
		}

		// POST: api/Errors
		[HttpPost]
		[ProducesResponseType(typeof(Fehler), 201)]
		[ProducesResponseType(409)]
		[ProducesResponseType(422)]
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

				throw;
			}

			var workplace = _context.Arbeitsplatz.Find(fehler.ArbeitsplatzId);
			var category = _context.Kategorie.Find(fehler.KategorieId);
			var lehrer = await _context.LehrerRaum.Where(lr => lr.RaumId == workplace.RaumId).ToListAsync();

			var ok = await SendErrorReportEmail(lehrer.Select(l => new Address(l.Lehrer.Email, l.Lehrer.Name)).ToList(),
				workplace.Raum.Name, fehler.Titel, category.Name, workplace.Name, fehler.Status);

			if (!ok)
			{
				return UnprocessableEntity("Email sending not ok");
			}

			return CreatedAtAction("GetFehler", new {id = fehler.Id}, fehler);
		}

		[HttpPut("[action]/{errorId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public async Task<ActionResult> MarkAsStandard(int errorId)
		{
			var error = await _context.Fehler.FindAsync(errorId);

			if (error == null)
			{
				return NotFound();
			}

			var standard = new StandardFehler
			{
				Beschreibung = error.Beschreibung,
				KategorieId = error.KategorieId,
				Status = error.Status > 0 ? error.Status : 1,
				Titel = error.Titel
			};

			_context.StandardFehler.Add(standard);

			return Ok();
		}

		[HttpDelete("[action]/{standardId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public async Task<ActionResult> DeleteStandard(int standardId)
		{
			var standard = await _context.StandardFehler.FindAsync(standardId);

			if (standard == null)
			{
				return NotFound();
			}

			_context.StandardFehler.Remove(standard);

			return Ok();
		}

		private bool FehlerExists(long id)
		{
			return _context.Fehler.Any(e => e.Id == id);
		}

		private async Task<bool> SendErrorReportEmail(List<Address> toAdresses, string roomName, string title,
			string category, string workplace, long status)
		{
			var email = Email.From(Environment.GetEnvironmentVariable("EMAIL_SENDER"))
				.To(toAdresses)
				.Subject($"Problem in Raum {roomName}")
				.UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Templates/ErrorReport.cshtml",
					new
					{
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