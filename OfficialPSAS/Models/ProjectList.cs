using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficialPSAS.Models;

namespace OfficialPSAS.Models
{
    public class ProjectList
    {
        public int tid { get; set; }
        public string username { get; set; }
        public string title { get; set; }
        public int pid { get; set; }
        public string description { get; set; }
        public string projectName { get; set; }
        public int gid { get; set; }

    }
}