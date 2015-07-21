using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH607
{
	public class PackageParty : PersistentObject
	{
		private IList<PPP> ppps;

		public PackageParty()
		{
			ppps = new List<PPP>();
		}

		public virtual IList<PPP> ParticipatingPackages
		{
			get { return ppps; }
			set { ppps = value; }
		}
	}
}