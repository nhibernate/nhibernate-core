using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1228
{
	public class Shelf
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<Folder> Folders { get; protected set; } = new HashSet<Folder>();
	}

	public class Folder
	{
		public virtual int Id { get; set; }
		public virtual Shelf Shelf { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<Sheet> Sheets { get; protected set; } = new HashSet<Sheet>();
	}

	public class Sheet
	{
		public virtual int Id { get; set; }
		public virtual Folder Folder { get; set; }
		public virtual string Name { get; set; }
	}
}
