using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Todos
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        //public string PreferedCalendar { get; set; }

        //private CalendarService calendarService; //todo coś lepszego
        //public CalendarService CalendarService
        //{
        //    get
        //    {
        //        if (calendarService is null)
        //        {
        //            UserCredential credential;
        //            using (var stream =
        //               new FileStream($"credentialsKrzys.json", FileMode.Open, FileAccess.Read))
        //            {
        //                // The file token.json stores the user's access and refresh tokens, and is created
        //                // automatically when the authorization flow completes for the first time.
        //                string credPath = $"tokenKrzys.json";
        //                string[] Scopes = { CalendarService.Scope.Calendar };
        //                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //                    GoogleClientSecrets.Load(stream).Secrets,
        //                    Scopes,
        //                    "user",
        //                    CancellationToken.None,
        //                    new FileDataStore(credPath, true)).Result;
        //            }

        //            calendarService = new CalendarService(new BaseClientService.Initializer()
        //            {
        //                HttpClientInitializer = credential,
        //                ApplicationName = "",
        //            });
        //        }
        //        else
        //        {
        //            if(calendarService.CalendarList.List().Execute().Items.Any(x => x.Summary == PreferedCalendar) == false)
        //            {
        //                calendarService.CalendarList.Insert(new Google.Apis.Calendar.v3.Data.CalendarListEntry()
        //                {
        //                    Summary = PreferedCalendar
        //                }).Execute();
        //            }
        //        }
        //        return calendarService;
        //    }
        //}

        //public string CalendarId => CalendarService.CalendarList.List().Execute().Items.FirstOrDefault(x => x.Summary == PreferedCalendar).Id;

        public UserDTO CreateDTO()
        {
            return new UserDTO
            {
                UserId = Id,
                UserName = UserName
            };
        }
    }

    public class UserDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
