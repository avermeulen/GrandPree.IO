using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace grandpree.io.Models
{
    public class Race
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public string Location { get; set; }
    }
}