using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Todos
{
    public class Check
    {
        [Key]
        public int CheckId { get; set; }
        public DateTime Date { get; set; }
        public int TaskId { get; set; }
        [ForeignKey("TaskId")]
        public Task Task { get; set; }

        public CheckDTO CreateDTO()
        {
            return new CheckDTO()
            {
                CheckId = CheckId,
                Date = Date,
            };
        }
    }

    public class CheckDTO
    {
        public int CheckId { get; set; }
        public DateTime Date { get; set; }
    }
}
