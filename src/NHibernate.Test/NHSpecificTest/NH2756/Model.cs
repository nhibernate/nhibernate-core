using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2756
{
	public class Document
	{
		public Document()
		{
			Files = new List<DocumentFile>();
		}

		public virtual Guid Id { get; set; }
		public virtual ICollection<DocumentFile> Files { get; set; }
	}

	public class DocumentFile
	{
		public string Description { get; set; }
		public string Filename { get; private set; }
		public File File { get; set; }
	}

	public class File
	{
		public virtual Guid Id { get; set; }
		public virtual string Filename { get; set; }
	}
}
