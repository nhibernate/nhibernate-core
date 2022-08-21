namespace NHibernate.Test.NHSpecificTest.GH2201
{
	public class Entity
	{
		public virtual int EntityId { get; set; }
		public virtual string EntityNumber { get; set; }
		public virtual Entity ReferencedEntity { get; set; }
		public virtual Entity AdditionalEntity { get; set; }
		public virtual Entity SourceEntity { get; set; }
		public virtual Level1Entity Level1 { get; set; }
	}

	public class Level1Entity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Level2Entity Level2 { get; set; }
	}

	public class Level2Entity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}
