using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity;

namespace Todos
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }
        public string Name { get; set; }
        public List<Check> Checks { get; set; }
        public int Interval { get; set; }
        public int ExtraTime { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public State State { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? Scheduled { get; set; }
        public TimeSpan? Meeting { get; set; }

        internal static void Check(int id)
        {
            using TaskContext ctx = new();
            Task task = ctx.Tasks
                .Where(x => x.TaskId == id)
                .Include(i => i.User)
                .Include(i => i.Checks)
                .First();
            task.Checks.Add(new Check() 
            { 
                Date = DateTime.Now 
            });
            task.ExtraTime = 0;
            task.Scheduled = null;
            task.Meeting = null;
            ctx.SaveChanges();
        }

        internal static Task Schedule(int id, DateTime? date, DateTime? meeting)
        {
            using TaskContext ctx = new();
            Task task = ctx.Tasks
                .Where(x => x.TaskId == id)
                .Include(i => i.User)
                .Include(i => i.Checks)
                .First();
            if(date.HasValue)
            {
                task.Meeting = new TimeSpan(1, 0, 0);
                if (meeting.HasValue)
                {
                    task.Meeting = new TimeSpan((meeting.Value.Hour + 2) % 24, meeting.Value.Minute, 0);
                }
                task.Scheduled = date;
                task.State = State.Scheduled;
            }
            else
            {
                task.Meeting = null;
                task.Scheduled = null;
                task.State = State.Safe;
            }
            ctx.SaveChanges();
            UpdateState(task.TaskId);
            return task;
        }

        public static void UpdateState(int taskId)
        {
            using TaskContext ctx = new();
            Task task = ctx.Tasks
                .Where(x => x.TaskId == taskId)
                .Include(i => i.User)
                .Include(i => i.Checks)
                .First();
            State start = task.State;
            if (!task.Scheduled.HasValue)
            {
                DateTime compareDate = task.AddedDate;
                if (task.Checks is not null)
                {
                    compareDate = task.Checks.OrderBy(x => x.Date).Last().Date;
                }
                if (DateTime.Now > compareDate + new TimeSpan(task.Interval + task.ExtraTime, 0,0,0))
                {
                    task.State = State.Urgent;
                }
                else if (task.Interval >= 92)
                {
                    if (DateTime.Now > compareDate + new TimeSpan(task.ExtraTime + task.Interval, 0, 0, 0) - new TimeSpan(7, 0, 0, 0))
                    {
                        task.State = State.VeryClose;
                    }
                    else if (DateTime.Now > compareDate + new TimeSpan(task.ExtraTime + task.Interval, 0, 0, 0) - new TimeSpan(30, 0, 0, 0))
                    {
                        task.State = State.Close;
                    }
                    else
                    {
                        task.State = State.Safe;
                    }
                }
                else
                {
                    if (DateTime.Now > compareDate + new TimeSpan(task.ExtraTime + task.Interval, 0, 0, 0) - new TimeSpan(task.Interval, 0, 0, 0) / 6)
                    {
                        task.State = State.VeryClose;
                    }
                    else if (DateTime.Now > compareDate + new TimeSpan(task.ExtraTime + task.Interval, 0, 0, 0) - new TimeSpan(task.Interval, 0, 0, 0) / 3)
                    {
                        task.State = State.Close;
                    }
                    else
                    {
                        task.State = State.Safe;
                    }
                }
            }
            else if (DateTime.Now > task.Scheduled)
            {
                task.Checks.Add(new Check() { Date = task.Scheduled.Value });
                task.Scheduled = null;
            }
            ctx.SaveChanges();
        }

        internal static void Delete(int id)
        {
            using TaskContext ctx = new();
            Task task = ctx.Tasks
                .Where(x => x.TaskId == id)
                .Include(i => i.User)
                .Include(i => i.Checks)
                .First();
            ctx.Checks.RemoveRange(task.Checks);
            ctx.Tasks.Remove(task);
            ctx.SaveChanges();
        }

        internal static void Postpone(int id)
        {
            using TaskContext ctx = new();
            Task task = ctx.Tasks
                .Where(x => x.TaskId == id)
                .First();
            task.ExtraTime += 7;
            ctx.SaveChanges();
        }

        public static Task Create(User user, string Name, DateTime? lastDone, int interval)
        {
            List<Check> list = new();
            if (lastDone.HasValue)
            {
                list.Add(new Check() 
                { 
                    Date = lastDone.Value 
                });
            }

            Task task = new()
            {
                Name = Name,
                AddedDate = DateTime.Now,
                State = State.Safe,
                Interval = interval,
                ExtraTime = 0,
                UserId = user.Id,
                Checks = list,
                Scheduled = null,
            };


            using TaskContext ctx = new();
            task = ctx.Tasks.Add(task);
            ctx.SaveChanges();
            UpdateState(task.TaskId);

            return task;
        }

        public TaskDTO CreateDTO()
        {
            UpdateState(TaskId);
            using TaskContext ctx = new();
            Task task = ctx.Tasks
               .Where(x => x.TaskId == TaskId)
               .Include(i => i.User)
               .Include(i => i.Checks)
               .First();
            return new TaskDTO()
            {
                TaskId = TaskId,
                Name = task.Name,
                Interval = task.Interval,
                ExtraTime = task.ExtraTime,
                User = task.User.CreateDTO(),
                State = task.State,
                AddedDate = task.AddedDate,
                Scheduled = task.Scheduled,
                Checks = task.Checks.ConvertToDTOList() 
            };
        }
    }

    public class TaskDTO
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public List<CheckDTO> Checks { get; set; }
        public int Interval { get; set; }
        public int ExtraTime { get; set; }
        public UserDTO User { get; set; }
        public State State { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? Scheduled { get; set; }
    }

    public enum State
    {
        Safe = 1,
        Scheduled = 2,
        Close = 5,
        VeryClose = 6,
        Urgent = 7,
        Invalid = 10,
    }
}
