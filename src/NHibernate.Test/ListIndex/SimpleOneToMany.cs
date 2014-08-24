using System.Collections.Generic;

namespace NHibernate.Test.ListIndex
{
	public class Image
	{
		public virtual int Id { get; set; }
		public virtual string Path { get; set; }
	}

	public class Galery
	{
		public Galery()
		{
			Images = new List<Image>();
		}
		public virtual int Id { get; set; }

		public virtual IList<Image> Images { get; set; }
	}
}