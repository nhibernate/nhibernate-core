using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.TypesTest
{
    public class TimestampUtcClass
    {
        public int Id { get; set; }
        public DateTime Value { get; set; }
        public DateTime Revision { get; protected set; }
    }
}
