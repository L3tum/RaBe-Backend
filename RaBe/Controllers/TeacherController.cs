﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RaBe.Model;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;

namespace RaBe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private RaBeContext context;

        public TeacherController(RaBeContext context)
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
        public IActionResult AddTeacher(Lehrer lehrer)
        {
            if (lehrer == null)
            {
                return this.BadRequest();
            }

            using (var sha = SHA256.Create())
            {
                lehrer.Password = Encoding.UTF8.GetString(sha.ComputeHash(Encoding.UTF8.GetBytes(lehrer.Password)));
            }

            return Ok(context.Lehrer.Add(lehrer));
        }

        [HttpPut]
        public IActionResult ModifyTeacher(Lehrer lehrer)
        {
            if (lehrer == null)
            {
                return this.BadRequest();
            }

            var dbLehrer = context.Lehrer.FirstOrDefault(r => r.Id == lehrer.Id);

            if (dbLehrer == null)
            {
                return NotFound();
            }

            if(dbLehrer.Password != lehrer.Password)
            {
                return BadRequest();
            }

            return Ok(context.Lehrer.Update(lehrer));
        }

        [Authorize]
        [HttpDelete("[action]/{teacherId}")]
        public IActionResult DeleteTeacher(int teacherId)
        {
            var lehrer = context.Lehrer.FirstOrDefault(r => r.Id == teacherId);

            if (lehrer == null)
            {
                return NotFound();
            }

            return Ok(context.Lehrer.Remove(lehrer));
        }

        [HttpPut("[action]/{teacherId}/{roomId}")]
        public IActionResult MarkAsAuthority(int teacherId, int roomId)
        {
            var lehrer = context.Lehrer.FirstOrDefault(r => r.Id == teacherId);

            if (lehrer == null)
            {
                return NotFound();
            }

            var room = context.Raum.FirstOrDefault(r => r.Id == roomId);

            if(room == null)
            {
                return NotFound();
            }

            var teacherRoom = room.LehrerRaum.FirstOrDefault(l => l.Id == teacherId);

            if(teacherRoom == null)
            {
                teacherRoom = new LehrerRaum();
                teacherRoom.LehrerId = teacherId;
                teacherRoom.RaumId = roomId;
                teacherRoom.Betreuer = 1;
                context.LehrerRaum.Add(teacherRoom);
            }
            else
            {
                teacherRoom.Betreuer = 1;
                context.LehrerRaum.Update(teacherRoom);
            }

            return Ok();
        }

        [HttpDelete("[action]/{teacherId}/{roomId}")]
        public IActionResult RemoveFromAuthority(int teacherId, int roomId)
        {
            var lehrer = context.Lehrer.FirstOrDefault(r => r.Id == teacherId);

            if (lehrer == null)
            {
                return NotFound();
            }

            var room = context.Raum.FirstOrDefault(r => r.Id == roomId);

            if (room == null)
            {
                return NotFound();
            }

            var teacherRoom = room.LehrerRaum.FirstOrDefault(l => l.Id == teacherId);

            if (teacherRoom == null)
            {
                teacherRoom = new LehrerRaum();
                teacherRoom.LehrerId = teacherId;
                teacherRoom.RaumId = roomId;
                teacherRoom.Betreuer = 0;
                context.LehrerRaum.Add(teacherRoom);
            }
            else
            {
                teacherRoom.Betreuer = 0;
                context.LehrerRaum.Update(teacherRoom);
            }

            return Ok();
        }
    }
}
