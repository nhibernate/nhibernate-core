using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH2812
{
    public class EntityWithAByteValue
    {
        public virtual Guid Id { get; protected set; }
        public virtual byte ByteValue { get; set; }
    }
}
