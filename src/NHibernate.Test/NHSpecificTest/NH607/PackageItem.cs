using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH607
{
	public class PackageItem : PersistentObject
	{
		private IList ppps;
		private Package package;

		public PackageItem()
		{
			this.ppps = new ArrayList();
		}

		public virtual Package Package
		{
			get { return package; }
			set { package = value; }
		}

		public virtual IList PackagePartyParticipants
		{
			get { return ppps; }
			set { ppps = value; }
		}
	}
}