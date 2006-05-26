using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH607
{
	public class Package : PersistentObject
	{
		private IList packageItems;

		public Package()
		{
			this.packageItems = new ArrayList();
		}

		public virtual IList PackageItems
		{
			get { return packageItems; }
			set { packageItems = value; }
		}
	}
}