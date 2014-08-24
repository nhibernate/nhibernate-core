namespace NHibernate.Loader.Custom
{
	/// <summary> Represents a return which names a fetched association. </summary>
	public abstract class FetchReturn : NonScalarReturn
	{
		private readonly NonScalarReturn owner;
		private readonly string ownerProperty;

		public FetchReturn(NonScalarReturn owner, string ownerProperty, string alias, LockMode lockMode)
			: base(alias, lockMode)
		{
			this.owner = owner;
			this.ownerProperty = ownerProperty;
		}

		/// <summary> Retrieves the return descriptor for the owner of this fetch. </summary>
		public NonScalarReturn Owner
		{
			get { return owner; }
		}

		/// <summary> The name of the property on the owner which represents this association. </summary>
		public string OwnerProperty
		{
			get { return ownerProperty; }
		}
	}
}