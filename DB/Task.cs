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
        public string CalendarEventId { get; set; }
        public State State { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? Scheduled { get; set; }
        public TimeSpan? Meeting { get; set; }

        internal static void Check(int id)
        {
            using TaskContext ctx = new();
            var task = ctx.Tasks.Where(x => x.TaskId == id).Include(i => i.User).Include(i => i.Checks).First();
            //task.Validate();
            //Event e = task.User.CalendarService.Events.Get(task.User.CalendarId, task.CalendarEventId).Execute();
            task.Checks.Add(new Check() { Date = DateTime.Now });
            task.ExtraTime = 0;
            //e.Start = new EventDateTime()
            //{
            //    Date = (DateTime.Now + new TimeSpan(task.Interval, 0, 0, 0)).ToString("yyyy-MM-dd")
            //};
            //e.End = new EventDateTime()
            //{
            //    Date = (DateTime.Now + new TimeSpan(task.Interval + 1, 0, 0, 0)).ToString("yyyy-MM-dd")
            //};
            //e.ColorId = "2";
            task.Scheduled = null;
            task.Meeting = null;
            //task.User.CalendarService.Events.Update(e, task.User.CalendarId, task.CalendarEventId).Execute();
            ctx.SaveChanges();
        }

        internal static void Schedule(int id, DateTime date, DateTime? meeting)
        {

            using TaskContext ctx = new();
            Task task = ctx.Tasks.Where(x => x.TaskId == id).Include(i => i.User).Include(i => i.Checks).First();
            //task.Validate();
            task.Meeting = new TimeSpan(1, 0, 0);
            if (meeting.HasValue)
                task.Meeting = new TimeSpan((meeting.Value.Hour + 2) % 24, meeting.Value.Minute, 0);
            //Event e = task.User.CalendarService.Events.Get(task.User.CalendarId, task.CalendarEventId).Execute();
            task.Scheduled = date;
            //e.Start = new EventDateTime()
            //{
            //    Date = date.ToString("yyyy-MM-dd")
            //};
            //e.End = new EventDateTime()
            //{
            //    Date = (date + task.Meeting.Value).ToString("yyyy-MM-dd"),
            //};
            //e.ColorId = "1";
            task.State = State.Scheduled;
            //task.User.CalendarService.Events.Update(e, task.User.CalendarId, task.CalendarEventId).Execute();
            ctx.SaveChanges();
        }

        public void UpdateState()
        {
            State start = State;
            if (!Scheduled.HasValue)
            {
                DateTime compareDate = AddedDate;
                if (Checks is not null)
                    compareDate = Checks.OrderBy(x => x.Date).Last().Date;
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
                if (start != State)
                {
                    //Event e = User.CalendarService.Events.Get(User.CalendarId, CalendarEventId).Execute();

                    //if (State == State.Urgent)
                    //{
                    //    e.ColorId = "4";
                    //}
                    //else if (State == State.VeryClose)
                    //{
                    //    e.ColorId = "6";
                    //}
                    //else if (State == State.Close)
                    //{
                    //    e.ColorId = "5";
                    //}
                    //else if (State == State.Safe)
                    //{
                    //    e.ColorId = "2";
                    //}
                    //User.CalendarService.Events.Update(e, User.CalendarId, CalendarEventId).Execute();
                }
            }
            else if (DateTime.Now > Scheduled)
            {
                //Event e = User.CalendarService.Events.Get(User.CalendarId, CalendarEventId).Execute();
                Checks.Add(new Check() { Date = Scheduled.Value });
                ExtraTime = 0;
                //e.Start = new EventDateTime()
                //{
                //    Date = (Scheduled.Value + new TimeSpan(Interval, 0, 0, 0)).ToString("yyyy-MM-dd")
                //};
                //e.End = new EventDateTime()
                //{
                //    Date = (Scheduled.Value + new TimeSpan(Interval + 1, 0, 0, 0)).ToString("yyyy-MM-dd")
                //};
                //e.ColorId = "2";
                Scheduled = null;
                //User.CalendarService.Events.Update(e, User.CalendarId, CalendarEventId).Execute();
            }
        }

        internal static void Delete(int id)
        {
            using TaskContext ctx = new();
            Task task = ctx.Tasks.Where(x => x.TaskId == id).Include(i => i.User).Include(i => i.Checks).First();
            //task.Validate();
            //task.User.CalendarService.Events.Delete(task.User.CalendarId, task.CalendarEventId).Execute();
            ctx.Checks.RemoveRange(task.Checks);
            ctx.Tasks.Remove(task);
            ctx.SaveChanges();
        }

        internal static void Postpone(int id)
        {
            using TaskContext ctx = new();
            Task task = ctx.Tasks.Where(x => x.TaskId == id).Include(i => i.User).Include(i => i.Checks).First();
            //task.Validate();
            task.ExtraTime += 7;
            //task.Validate();
            ctx.SaveChanges();
        }

        public static Task Create(User user, string Name, DateTime? lastDone, int interval)
        {
            List<Check> list = new();
            if (lastDone.HasValue)
                list.Add(new Check() { Date = lastDone.Value });

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

            //task.CalendarEventId = CreateEvent(task, user);
            using TaskContext ctx = new();
            ctx.Tasks.Add(task);
            ctx.SaveChanges();

            Task task2 = ctx.Tasks.Where(x => x.TaskId == task.TaskId).Include(i => i.User).Include(i => i.Checks).First();
            task2.UpdateState();
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

        //private static string CreateEvent(Task task, User user)
        //{
        //    DateTime initialDate = DateTime.Now + new TimeSpan(task.Interval + task.ExtraTime, 0, 0, 0);
        //    if (task.Checks.Count > 0)
        //        initialDate = task.Checks.Last().Date + new TimeSpan(task.Interval + task.ExtraTime, 0, 0, 0);

        //    Event e = user.CalendarService.Events.Insert(new Event()
        //    {
        //        Summary = task.Name,
        //        Start = new EventDateTime()
        //        {
        //            Date = initialDate.ToString("yyyy-MM-dd")
        //        },
        //        End = new EventDateTime()
        //        {
        //            Date = (initialDate + new TimeSpan(1, 0, 0, 0)).ToString("yyyy-MM-dd")
        //        },
        //    }, user.CalendarId).Execute();

        //    return e.Id;
        //}

        //public void Validate()
        //{
        //    using TaskContext ctx = new();
        //    Event e = User.CalendarService.Events.Get(User.CalendarId, CalendarEventId).Execute();
        //    if (e is null || e.Status == "cancelled")
        //    {
        //        CalendarEventId = CreateEvent(this, User);
        //    }
        //    else
        //    {
        //        DateTime initialDate = AddedDate + new TimeSpan(Interval + ExtraTime, 0, 0, 0) ;
        //        if (Checks.Count > 0)
        //            initialDate = Checks.Last().Date + new TimeSpan(Interval + ExtraTime, 0, 0, 0);

        //        if(State.Scheduled == State && Scheduled.HasValue && Meeting.HasValue)
        //        {
        //            if (e.Start.Date != Scheduled.Value.ToString("yyyy-MM-dd"))
        //            {
        //                e.Start.Date = Scheduled.Value.ToString("yyyy-MM-dd");
        //                e.Start.DateTime = null;
        //                e.End.Date = (Scheduled.Value + Meeting.Value).ToString("yyyy-MM-dd");
        //                e.End.DateTime = null;
        //                try
        //                {
        //                    User.CalendarService.Events.Update(e, User.CalendarId, CalendarEventId).Execute();
        //                }
        //                catch (Google.GoogleApiException) { }
        //            }
        //        }
        //        else if(e.End.Date != (initialDate + new TimeSpan(1, 0, 0, 0)).ToString("yyyy-MM-dd") || e.Start.Date != initialDate.ToString("yyyy-MM-dd"))
        //        {
        //            e.End.Date = (initialDate + new TimeSpan(1, 0, 0, 0)).ToString("yyyy-MM-dd");
        //            e.Start.Date = initialDate.ToString("yyyy-MM-dd");
        //            e.Start.DateTime = null;
        //            e.End.DateTime = null;
        //            try
        //            {
        //                User.CalendarService.Events.Update(e, User.CalendarId, CalendarEventId).Execute();
        //            }
        //            catch (Google.GoogleApiException) { }
        //        }
        //        State = State.Invalid;
        //        if (int.TryParse(e.ColorId, out int ColorId))
        //            if(Enum.IsDefined(typeof(State), ColorId))
        //                State = (State) ColorId;
        //    }
        //    UpdateState();
        //}
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
