using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Todos.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Controller : ControllerBase
    {
        [HttpGet("Check/{id}")]
        public IActionResult Check(int id)
        {
            Task.Check(id);
            return Ok();
        }

        [HttpPost("Schedule")]
        public IActionResult Schedule([FromBody] TaskToSchedule task)
        {
            Task.Schedule(task.Id, task.Date, task.Meeting);
            return Ok();
        }

        [HttpGet("Postpone/{id}")]
        public IActionResult Postpone(int id)
        {
            Task.Postpone(id);
            return Ok();
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            Task.Delete(id);
            return Ok();
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody] TaskToCreate task)
        {
            using TaskContext ctx = new();
            Task task2 = Task.Create(ctx.Users.FirstOrDefault(x => x.Id == task.UserId), task.Name, task.LastDate, task.Interval);
            return Ok(task2);
        }

        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            using TaskContext ctx = new();
            return Ok(ctx.Users.ToList().ConvertToDTOList());
        }

        [HttpGet("GetUser/{userId}")]
        public IActionResult GetUser(int userId)
        {
            using TaskContext ctx = new();
            if (userId > 0)
                return Ok(ctx.Users.Where(x => x.Id == userId).ToList().ConvertToDTOList()[0]);
            else
                return Ok(ctx.Users.ToList().ConvertToDTOList()[0]);
        }

        [HttpGet("GetTasks/{userId}")]
        public IActionResult GetTasks(int? userId)
        {
            using TaskContext ctx = new();
            if (!userId.HasValue)
                userId = ctx.Users.FirstOrDefault().Id;
            List<Task> tasks = ctx.Tasks.Where(x => x.UserId == userId.Value).Include(i => i.User).Include(i => i.Checks).ToList();
            //try
            //{
            //    tasks.ForEach(x => x.Validate());
            //    ctx.SaveChanges();
            //}
            //catch (Exception) { }
            return Ok(tasks.ConvertToDTOList());
        }
    }

    public class TaskToCreate
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int Interval { get; set; }
        public DateTime? LastDate { get; set; }
    }

    public class TaskToSchedule
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime? Meeting { get; set; }
    }
}
