using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH2053
{
    public class Dog: Animal
    {
        public virtual Boolean Talkable { get; set; }
    }
}
