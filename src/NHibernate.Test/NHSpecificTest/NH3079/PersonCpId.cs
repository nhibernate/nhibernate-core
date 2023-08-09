namespace NHibernate.Test.NHSpecificTest.NH3079
{
	public class PersonCpId
	{
		public int IdA { get; set; }

		public int IdB { get; set; }

		public override bool Equals(object obj)
		{
			if (!(obj is PersonCpId objCpId))
				return false;

			return IdA == objCpId.IdA && IdB == objCpId.IdB;
		}

		public override int GetHashCode()
		{
			return IdA.GetHashCode() ^ IdB.GetHashCode();
		}
	}
}
