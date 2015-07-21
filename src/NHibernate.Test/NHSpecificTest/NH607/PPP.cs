using System;

namespace NHibernate.Test.NHSpecificTest.NH607
{
	public class PPP : PersistentObject
	{
		private PackageItem packageItem;
		private PackageParty packageParty;

		public PPP()
		{
		}

		public virtual PackageParty PackageParty
		{
			get { return packageParty; }
			set { packageParty = value; }
		}

		public virtual PackageItem PackageItem
		{
			get { return packageItem; }
			set { packageItem = value; }
		}
	}
}