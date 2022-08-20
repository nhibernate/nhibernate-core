namespace NHibernate.Test.NHSpecificTest.GH2201
{
	public class Entity
	{
		public virtual int EntityId { get; set; }
		public virtual string EntityNumber { get; set; }
		public virtual Entity ReferencedEntity { get; set; }
		public virtual Entity AdditionalEntity { get; set; }
		public virtual Entity SourceEntity { get; set; }
	}
}
