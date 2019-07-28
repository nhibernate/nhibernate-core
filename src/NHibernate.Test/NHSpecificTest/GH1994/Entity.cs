using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1994
{
	public class Base
	{
		public virtual Guid Key { get; set; }

		public virtual bool IsDeleted { get; set; }
	}

	public class Asset : Base
	{
		public virtual ISet<Document> Documents { get; set; } = new HashSet<Document>();
		public virtual ISet<Document> DocumentsFiltered { get; set; } = new HashSet<Document>();
		public virtual IList<Document> DocumentsBag { get; set; } = new List<Document>();
	}

	public class Document : Base
	{
		public virtual ISet<Asset> Assets { get; set; } = new HashSet<Asset>();
	}
}
