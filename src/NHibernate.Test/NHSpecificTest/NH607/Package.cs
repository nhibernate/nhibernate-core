using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH607
{
	public class Package : PersistentObject
	{
		private IList<PackageItem> packageItems;

		public Package()
		{
			this.packageItems = new List<PackageItem>();
		}

		public virtual IList<PackageItem> PackageItems
		{
			get { return packageItems; }
			set { packageItems = value; }
		}
	}
}