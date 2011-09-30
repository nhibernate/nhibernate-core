using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NHXXXX
{
    class Child
    {
        public string Code { get; set; }
    }
    class Entity
    {
        public Entity()
        {
            Children = new List<Child>();
        }
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<Child> Children { get; set; }
    }
}
