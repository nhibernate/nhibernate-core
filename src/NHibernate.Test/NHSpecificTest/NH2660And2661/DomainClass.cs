using System;

namespace NHibernate.Test.NHSpecificTest.NH2660And2661
{
    public class DomainClass
    {
        private int id;
        private DateTime data;
        public int Id { get { return id; } set { id = value; } }
        public DateTime Data { get { return data; } set { data = value; } }
    }
}
