namespace NHibernate.Test.NHSpecificTest.EntityWithUserTypeCanHaveLinqGenerators
{
	public class EntityWithUserTypeProperty
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IExample Example { get; set; }
		public virtual double DoubleStoredAsString { get; set; }
	}
}
