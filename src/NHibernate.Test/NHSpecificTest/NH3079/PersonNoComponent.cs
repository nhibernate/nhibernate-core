namespace NHibernate.Test.NHSpecificTest.NH3079
{
	public class PersonNoComponent
	{
		public virtual int IdA { get; set; }

		public virtual int IdB { get; set; }

		public virtual string Name { get; set; }

		public override bool Equals(object obj)
		{
			if (!(obj is PersonNoComponent objNoComponent))
				return false;

			return IdA == objNoComponent.IdA && IdB == objNoComponent.IdB;
		}

		public override int GetHashCode()
		{
			return IdA.GetHashCode() ^ IdB.GetHashCode();
		}
	}
}
