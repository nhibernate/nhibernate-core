using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3694
{
	public class EntityWithComponentsCollection
    {
        public virtual int Id { get; set; }
        public virtual ISet<Component> Components { get; set; }
    }

    public class Component
    {
        public virtual string Val1 { get; set; }
        public virtual string Val2 { get; set; }
    }
}