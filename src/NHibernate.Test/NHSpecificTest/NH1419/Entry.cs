using System;

namespace NHibernate.Test.NHSpecificTest.NH1419
{
	[Serializable]
	public class Entry
	{
		private Guid id;
		private Blog blog;
		private string subject;

		public Guid ID
		{
			get { return id; }
			set { id = value; }
		}

		public Blog Blog
		{
			get { return blog; }
			set { blog = value; }
		}

		public string Subject
		{
			get { return subject; }
			set { subject = value; }
		}
	}
}
