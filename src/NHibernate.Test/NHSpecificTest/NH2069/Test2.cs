using System;

namespace NHibernate.Test.NHSpecificTest.NH2069
{
    public class Test2 : ITest2
    {
        public Test2() { }
               
        public Int64 Cid { get; set; }    //When using this property it works fine.

        public string Description { get; set; }
    }
}
