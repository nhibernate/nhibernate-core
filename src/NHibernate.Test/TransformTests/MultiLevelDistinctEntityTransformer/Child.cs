using System.Collections.Generic;

namespace NHibernate.Test.TransformTests.MultiLevelDistinctEntityTransformer
{
    public class Child
    {
        public int Id { get; set; }

        public Employee Employee { get; set; }

        public string FirstName { get; set; }

        public List<Passport> Passports { get; set; }

        public Child()
        {
            Passports =new List<Passport>();
        }
    }
}