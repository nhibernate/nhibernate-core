using System;

namespace NHibernate.Loader.Custom
{
	public abstract class FetchReturn : NonScalarReturn
	{
		private readonly NonScalarReturn owner;
		private readonly string ownerProperty;

		public FetchReturn(
			NonScalarReturn owner,
			string ownerProperty,
			string alias,
			LockMode lockMode)
			:
				base(alias, lockMode)
		{
			this.owner = owner;
			this.ownerProperty = ownerProperty;
		}

		public NonScalarReturn Owner
		{
			get { return owner; }
		}

		public string OwnerProperty
		{
			get { return ownerProperty; }
		}
	}
}