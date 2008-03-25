using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH607
{
	public class PackageParty : PersistentObject
	{
		private IList ppps;

		public PackageParty()
		{
			ppps = new ArrayList();
		}

		public virtual IList ParticipatingPackages
		{
			get { return ppps; }
			set { ppps = value; }
		}
	}
}