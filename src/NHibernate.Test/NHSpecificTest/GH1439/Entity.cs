namespace NHibernate.Test.NHSpecificTest.GH1439
{
	public class CompositeEntity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string LazyProperty { get; set; }

		public override int GetHashCode()
		{
			return (Id + "|" + Name).GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			return obj.GetHashCode() == GetHashCode();
		}
	}
}
