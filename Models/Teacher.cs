using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Models
{
    public class Teacher : Record
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        const string Avatars_Folder = @"/App_Assets/Users/";
        const string Default_Avatar = @"no_avatar.png";
        [ImageAsset(Avatars_Folder, Default_Avatar)]
        public string Avatar { get; set; } = Avatars_Folder + Default_Avatar;
        [JsonIgnore]
        public string AvatarUrl
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Avatar))
                    return "/App_Assets/users/no_avatar.png";

                if (Avatar.StartsWith("/") || Avatar.StartsWith("~/"))
                    return Avatar.Replace("~", "");

                return "/App_Assets/users/" + Avatar;
            }
        }

        [JsonIgnore] public string FullName => LastName + " " + FirstName;
        [JsonIgnore] public string Caption => Code + " " + LastName + " " + FirstName;
        [JsonIgnore] public List<Allocation> Allocations => DB.Allocations.ToList().Where(a => a.TeacherId == Id).ToList();
        [JsonIgnore] public List<Allocation> NextSessionAllocations => DB.Allocations.ToList().Where(a => a.TeacherId == Id && a.IsNextSession).ToList();
        [JsonIgnore] public List<Course> Courses => Allocations.OrderBy(a => a.Course.Code).Select(a => a.Course).ToList();
        [JsonIgnore] public List<Course> NextSessionCourses => NextSessionAllocations.OrderBy(a => a.Course.Code).Select(a => a.Course).ToList();
        [JsonIgnore] public SelectList NextSessionCoursesToSelectList => SelectListUtilities<Course>.Convert(NextSessionCourses, "Caption");

        public void DeleteAllAllocations()
        {
            foreach (Allocation allocation in Allocations.ToList()) DB.Allocations.Delete(allocation.Id);
        }
        public void DeleteNextSessionAllocations()
        {
            foreach (Allocation allocation in NextSessionAllocations.ToList()) DB.Allocations.Delete(allocation.Id);
        }
        public void UpdateAllocations(List<int> selectedCoursesId)
        {
            DeleteNextSessionAllocations();
            if (selectedCoursesId != null)
                foreach (int courseId in selectedCoursesId)
                    DB.Allocations.Add(new Allocation { TeacherId = Id, CourseId = courseId });
        }
    }
}
