namespace NHibernate.Test.NHSpecificTest.NH1553.MsSQL
{
	/// <summary>
	/// A simple entity for the test.
	/// </summary>
	public class Person
	{
		public virtual long Id { get; set; }

		public virtual long Version { get; set; }

		public virtual long IdentificationNumber { get; set; }
	}
}