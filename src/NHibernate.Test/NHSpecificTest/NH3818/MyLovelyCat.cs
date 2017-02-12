using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3818
{
    class MyLovelyCat
    {
        public virtual Guid GUID { get; set; }
        
        public virtual String Name { get; set; }
        public virtual String Color { get; set; }
        public virtual DateTime Birthdate { get; set; }
        public virtual Decimal Price { get; set; }
    }
}
