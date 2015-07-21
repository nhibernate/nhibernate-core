using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH607
{
	public class PackageItem : PersistentObject
	{
		private IList<PPP> ppps;
		private Package package;

		public PackageItem()
		{
			this.ppps = new List<PPP>();
		}

		public virtual Package Package
		{
			get { return package; }
			set { package = value; }
		}

		public virtual IList<PPP> PackagePartyParticipants
		{
			get { return ppps; }
			set { ppps = value; }
		}
	}
}