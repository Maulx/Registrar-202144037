using DAL;
using Newtonsoft.Json;
using System;


namespace Models
{
    public class Registration
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int Year { get; set; }
    }
}