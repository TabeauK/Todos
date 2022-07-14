using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
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

        internal static void Schedule(int id, DateTime date, DateTime? meeting)
        {
            using TaskContext ctx = new();
            Task task = ctx.Tasks
                .Where(x => x.TaskId == id)
                .Include(i => i.User)
                .Include(i => i.Checks)
                .First();
            task.Meeting = new TimeSpan(1, 0, 0);
            if (meeting.HasValue)
            {
                task.Meeting = new TimeSpan((meeting.Value.Hour + 2) % 24, meeting.Value.Minute, 0);
            }
            task.Scheduled = date;
            task.State = State.Scheduled;
            ctx.SaveChanges();
        }

        public void UpdateState()
        {
            State start = State;
            if (!Scheduled.HasValue)
            {
                DateTime compareDate = AddedDate;
                if (Checks is not null)
                {
                    compareDate = Checks.OrderBy(x => x.Date).Last().Date;
                }
                if (DateTime.Now > compareDate + new TimeSpan(Interval + ExtraTime, 0,0,0))
                {
                    State = State.Urgent;
                }
                else if (Interval >= 92)
                {
                    if (DateTime.Now > compareDate + new TimeSpan(ExtraTime + Interval, 0, 0, 0) - new TimeSpan(7, 0, 0, 0))
                    {
                        State = State.VeryClose;
                    }
                    else if (DateTime.Now > compareDate + new TimeSpan(ExtraTime + Interval, 0, 0, 0) - new TimeSpan(30, 0, 0, 0))
                    {
                        State = State.Close;
                    }
                    else
                    {
                        State = State.Safe;
                    }
                }
                else
                {
                    if (DateTime.Now > compareDate + new TimeSpan(ExtraTime + Interval, 0, 0, 0) - new TimeSpan(Interval, 0, 0, 0) / 6)
                    {
                        State = State.VeryClose;
                    }
                    else if (DateTime.Now > compareDate + new TimeSpan(ExtraTime + Interval, 0, 0, 0) - new TimeSpan(Interval, 0, 0, 0) / 3)
                    {
                        State = State.Close;
                    }
                    else
                    {
                        State = State.Safe;
                    }
                }
            }
            else if (DateTime.Now > Scheduled)
            {
                Checks.Add(new Check() { Date = Scheduled.Value });
                Scheduled = null;
            }
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
                .Include(i => i.User)
                .Include(i => i.Checks)
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

            task.UpdateState();

            using TaskContext ctx = new();
            ctx.Tasks.Add(task);
            ctx.SaveChanges();

            return task;
        }

        public TaskDTO CreateDTO()
        {
            return new TaskDTO()
            {
                TaskId = TaskId,
                Name = Name,
                Interval = Interval,
                ExtraTime = ExtraTime,
                User = User.CreateDTO(),
                State = State,
                AddedDate = AddedDate,
                Scheduled = Scheduled,
                Checks = Checks.ConvertToDTOList() 
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
        Safe = 2,
        Scheduled = 1,
        Close = 5,
        VeryClose = 6,
        Urgent = 4,
        Invalid = 10,
    }
}
