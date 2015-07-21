using System;
namespace NHibernate.Test.NHSpecificTest.NH1217
{
    interface IDomainBase
    {
        Int32 Id { get; set; }
        int VersionNumber { get; set; }
    }
}
