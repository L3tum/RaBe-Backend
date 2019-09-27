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
	public class CategoriesController : ControllerBase
	{
		private readonly RaBeContext _context;

		public CategoriesController(RaBeContext context)
		{
			_context = context;
		}

		// GET: api/Categories
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Kategorie>>> GetCategory()
		{
			return await _context.Kategorie.ToListAsync();
		}

		// GET: api/Categories/5
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(Kategorie), 200)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<Kategorie>> GetCategory(long id)
		{
			var kategorie = await _context.Kategorie.FindAsync(id);

			if (kategorie == null)
			{
				return NotFound();
			}

			return Ok(kategorie);
		}

		// PUT: api/Categories/5
		[HttpPut("{id}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> PutCategory(long id, Kategorie kategorie)
		{
			if (id != kategorie.Id)
			{
				return BadRequest();
			}

			_context.Entry(kategorie).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!KategorieExists(id))
				{
					return NotFound();
				}

				throw;
			}

			return Ok();
		}

		// POST: api/Categories
		[HttpPost]
		[ProducesResponseType(typeof(Kategorie), 201)]
		[ProducesResponseType(409)]
		public async Task<ActionResult<Kategorie>> PostCategory(Kategorie kategorie)
		{
			_context.Kategorie.Add(kategorie);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (KategorieExists(kategorie.Id))
				{
					return Conflict();
				}

				throw;
			}

			return CreatedAtAction("GetCategory", new {id = kategorie.Id}, kategorie);
		}

		// DELETE: api/Categories/5
		[HttpDelete("{id}")]
		[ProducesResponseType(typeof(Kategorie), 200)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<Kategorie>> DeleteCategory(long id)
		{
			var kategorie = await _context.Kategorie.FindAsync(id);
			if (kategorie == null)
			{
				return NotFound();
			}

			_context.Kategorie.Remove(kategorie);
			await _context.SaveChangesAsync();

			return Ok(kategorie);
		}

		private bool KategorieExists(long id)
		{
			return _context.Kategorie.Any(e => e.Id == id);
		}
	}
}