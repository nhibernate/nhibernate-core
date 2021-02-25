using System;

namespace NHibernate.Test.NHSpecificTest.GH1719
{
    public class FileDataEntity
    {
	    public virtual Guid Id { get; set; }
	    public virtual FileEntryEntity Entry { get; set; }
	}
}
