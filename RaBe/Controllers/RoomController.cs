using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpGet("[method]")]
        public ActionResult<IEnumerable<Raum>> GetAllRooms()
        {
            return context.Raum;
        }

        [HttpGet("[method]/{teacherId}")]
        [ProducesResponseType(typeof(IList<Raum>), 200)]
        public IActionResult GetAllRoomsOfTeacher(int teacherId)
        {
            return Ok(context.Raum.Where(r => r.LehrerRaum.Where(lr => lr.LehrerId == teacherId).Count() > 0));
        }

        [HttpGet("[method]/{raumId}")]
        [ProducesResponseType(typeof(IList<Arbeitsplatz>), 200)]
        public IActionResult GetAllWorkplacesOfRoom(int raumId)
        {
            return Ok(context.Arbeitsplatz.Where(a => a.RaumId == raumId));
        }

        [HttpPut("[method]")]
        public IActionResult ModifyRoom()
        {
            Raum raum = null;

            using (var reader = new StreamReader(Request.Body))
            {
                var body = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(body))
                {
                    return BadRequest();
                }

                raum = JsonConvert.DeserializeObject<Raum>(body);
            }

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
        public IActionResult AddRoom()
        {
            Raum raum = null;

            using (var reader = new StreamReader(Request.Body))
            {
                var body = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(body))
                {
                    return BadRequest();
                }

                raum = JsonConvert.DeserializeObject<Raum>(body);
            }

            if (raum == null)
            {
                return this.BadRequest();
            }

            return Ok(context.Raum.Add(raum));
        }

        [HttpDelete("[method]/{raumId}")]
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
