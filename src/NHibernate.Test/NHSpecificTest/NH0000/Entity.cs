using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH0000
{
    class Entity
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
    }
}
