using DAL;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wikimedia.Models
{
    public class Allocation : Record
    {
        public int TeacherId { get; set; }
        public int CourseId { get; set; }
        public int Year { get; set; }

        [JsonIgnore]
        public Teacher Teacher => DB.Teachers.Get(TeacherId);
        [JsonIgnore]
        public Course Course => DB.Courses.Get(CourseId);
    }
}