using System;

namespace NHibernate.Test.NHSpecificTest.NH2069
{
    public interface ITest : ITestBase
    {
        string Description { get; set;}
        ITest2 Category { get; set; }
    }
}
