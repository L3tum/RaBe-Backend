using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RaBe.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace RaBe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private RaBeContext context;

        public RoomController(RaBeContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Raum>>> GetAllRooms()
        {
            return Ok(await context.Raum.ToListAsync());
        }

        [HttpGet("[action]/{teacherId}")]
        [ProducesResponseType(typeof(IList<Raum>), 200)]
        public IActionResult GetAllRoomsOfTeacher(int teacherId)
        {
            return Ok(context.Raum.Where(r => r.LehrerRaum.Where(lr => lr.LehrerId == teacherId).Count() > 0));
        }

        [HttpGet("[action]/{raumId}")]
        [ProducesResponseType(typeof(IList<Arbeitsplatz>), 200)]
        public IActionResult GetAllWorkplacesOfRoom(int raumId)
        {
            return Ok(context.Arbeitsplatz.Where(a => a.RaumId == raumId));
        }

        [HttpPut]
        public IActionResult ModifyRoom(Raum raum)
        {
            if(raum == null)
            {
                return this.BadRequest();
            }

            var dbRaum = context.Raum.FirstOrDefault(r => r.Id == raum.Id);

            if(dbRaum == null)
            {
                return NotFound();
            }

            return Ok(context.Raum.Update(raum));
        }

        [HttpPost]
        public IActionResult AddRoom(Raum raum)
        {
            if (raum == null)
            {
                return this.BadRequest();
            }

            return Ok(context.Raum.Add(raum));
        }

        [HttpDelete("{raumId}")]
        public IActionResult DeleteRoom(int raumId)
        {
            var raum = context.Raum.FirstOrDefault(r => r.Id == raumId);

            if(raum == null)
            {
                return NotFound();
            }

            return Ok(context.Raum.Remove(raum));
        }
    }
}
