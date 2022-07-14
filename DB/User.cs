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
