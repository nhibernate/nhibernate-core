namespace NHibernate.Test.NHSpecificTest.NH3634
{
	class CachedPerson
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Connection Connection { get; set; }
	}
}