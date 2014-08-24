using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.SessionIdLoggingContextTest
{
    public class ClassA
    {
        public virtual Guid Id { get; set; }
        public virtual IList<ClassA> Children { get; set; }
        public virtual string Name { get; set; }
    }
}
