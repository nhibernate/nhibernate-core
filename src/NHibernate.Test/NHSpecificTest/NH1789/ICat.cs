namespace NHibernate.Test.NHSpecificTest.NH1789
{
	/// <summary>
	/// An interface
	/// </summary>
	public interface ICat : IDomainObject
	{
		/// <summary>
		/// Name of the cat
		/// </summary>
		string Name { get; set; }
	}
}