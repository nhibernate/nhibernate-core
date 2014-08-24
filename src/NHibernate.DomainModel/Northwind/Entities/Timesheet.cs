using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Timesheet
    {
        private IList<User> users;

        public virtual int Id { get; set; }
        public virtual DateTime SubmittedDate { get; set; }
        public virtual bool Submitted { get; set; }
        public virtual IList<TimesheetEntry> Entries { get; set; }

        public virtual IEnumerable<User> Users
        {
            get
            {
                if (users == null)
                    users = new List<User>();
                return users;
            }
        }
    }

    public class TimesheetEntry
    {
        public virtual int Id { get; set; }
        public virtual DateTime EntryDate { get; set; }
        public virtual int NumberOfHours { get; set; }
        public virtual string Comments { get; set; }
    }
}