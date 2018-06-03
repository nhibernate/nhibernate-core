using System;

namespace NHibernate.Test.NHSpecificTest.GH1719
{
    public class FileEntryEntity
    {
	    public virtual Guid Id { get; set; }
	    public virtual Guid? ParentId { get; set; }
	    public virtual string Name { get; set; }
	    public virtual FileDataEntity Data { get; set; }
	}
}
