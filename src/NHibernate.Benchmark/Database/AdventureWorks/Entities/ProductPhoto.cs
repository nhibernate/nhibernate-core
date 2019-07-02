using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ProductPhoto
	{
		public virtual int Id { get; set; }
		public virtual byte[] LargePhoto { get; set; }
		public virtual string LargePhotoFileName { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual byte[] ThumbNailPhoto { get; set; }
		public virtual string ThumbnailPhotoFileName { get; set; }
		public virtual ICollection<ProductProductPhoto> ProductProductPhoto { get; set; }
	}
}
