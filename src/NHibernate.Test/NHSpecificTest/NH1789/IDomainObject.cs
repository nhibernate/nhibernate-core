namespace NHibernate.Test.NHSpecificTest.NH1789
{
	/// <summary>
	/// Domain Object
	/// </summary>
	public interface IDomainObject
	{
		/// <summary>
		/// Database unique ID for this object. This ID shouldn't have any business meaning.
		/// </summary>
		long ID { get; set; }

		///<summary>
		/// This is a string which uniquely identifies an instance in the case that the ids 
		/// are both transient
		///</summary>
		string BusinessKey { get; }

		/// <summary>
		/// Transient objects are not associated with an 
		/// item already in storage.  For instance, a 
		/// Customer is transient if its ID is 0.
		/// </summary>
		bool IsTransient();

		/// <summary>
		/// Returns the concrete type of the object, not the proxy one.
		/// </summary>
		/// <returns></returns>
		System.Type GetConcreteType();

		bool Equals(object that);
		int GetHashCode();
	}
}