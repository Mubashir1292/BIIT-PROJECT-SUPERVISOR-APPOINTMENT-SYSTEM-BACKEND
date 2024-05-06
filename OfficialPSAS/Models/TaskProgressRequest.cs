using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficialPSAS.Models
{
    public class TaskProgressRequest
    {
        public int Status { get; set; }
        public string Comments { get; set; }
    }
}