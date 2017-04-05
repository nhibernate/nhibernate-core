using System;

namespace NHibernate.Test.NHSpecificTest.NH2931
{
    public abstract class Entity
    {
        public Guid Id { get; private set; }
    }
    public class BaseClass : Entity
    {
        public string BaseProperty { get; set; }
    }
    public class DerivedClass : BaseClass
    {
        public string DerivedProperty { get; set; }
    }
}
