using DAL;
using Newtonsoft.Json;
using System;

namespace Models
{
    public class Student : Record
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        [JsonIgnore]
        public int Year => int.Parse(Code.Substring(0, 4));
    }
}