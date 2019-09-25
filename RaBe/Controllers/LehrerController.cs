﻿using Microsoft.AspNetCore.Mvc;
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
    public class LehrerController : ControllerBase
    {
        private RaBeContext context;

        public LehrerController(RaBeContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<Lehrer>), 200)]
        public IActionResult GetAllTeachers()
        {
            return Ok(context.Lehrer);
        }

        [HttpPost]
        public IActionResult AddTeacher()
        {
            Lehrer lehrer = null;

            using (var reader = new StreamReader(Request.Body))
            {
                var body = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(body))
                {
                    return BadRequest();
                }

                lehrer = JsonConvert.DeserializeObject<Lehrer>(body);
            }

            if (lehrer == null)
            {
                return this.BadRequest();
            }

            return Ok(context.Lehrer.Add(lehrer));
        }

        [HttpPut]
        public IActionResult ModifyTeacher()
        {
            Lehrer lehrer = null;

            using (var reader = new StreamReader(Request.Body))
            {
                var body = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(body))
                {
                    return BadRequest();
                }

                lehrer = JsonConvert.DeserializeObject<Lehrer>(body);
            }

            if (lehrer == null)
            {
                return this.BadRequest();
            }

            var dbLehrer = context.Lehrer.FirstOrDefault(r => r.Id == lehrer.Id);

            if (dbLehrer == null)
            {
                return NotFound();
            }

            return Ok(context.Lehrer.Update(lehrer));
        }

        [HttpDelete("[method]/{teacherId}")]
        public IActionResult DeleteTeacher(int teacherId)
        {
            var lehrer = context.Lehrer.FirstOrDefault(r => r.Id == teacherId);

            if (lehrer == null)
            {
                return NotFound();
            }

            return Ok(context.Lehrer.Remove(lehrer));
        }
    }
}
