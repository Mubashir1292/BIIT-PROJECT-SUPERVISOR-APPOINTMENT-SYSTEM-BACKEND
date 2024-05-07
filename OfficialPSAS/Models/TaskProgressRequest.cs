using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficialPSAS.Models
{
    public class TaskProgressRequest
    {
        public string GroupMemberId { get; set; }
        public int Status { get; set; }
        public string Comments { get; set; }
    }
}