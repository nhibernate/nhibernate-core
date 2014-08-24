using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1419
{
	[Serializable]
	public class Blog
	{
		private Guid id;
		private string name;
		private IList<Entry> entries;

		public Guid ID
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public IList<Entry> Entries
		{
			get { return entries; }
			set { entries = value; }
		}

		public void AddEntry(Entry entry)
		{
			if (entries == null)
			{
				entries = new List<Entry>();
			}
			entry.Blog = this;
			entries.Add(entry);
		}
	}
}