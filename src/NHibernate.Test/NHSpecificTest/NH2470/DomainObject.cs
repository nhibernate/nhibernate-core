using System;

namespace NHibernate.Test.NHSpecificTest.NH2470
{
	public abstract class DomainObject
    {
        public virtual Guid ID { get; set; }
        public virtual int EntityVersion { get; set; }
    }
}