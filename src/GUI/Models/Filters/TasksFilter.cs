using System;

namespace LT.DigitalOffice.GUI.Models.Filters
{
    public class TasksFilter
    {
        public Guid? ProjectId { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? AssignedTo { get; set; }
    }
}