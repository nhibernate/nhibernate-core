namespace NHibernate.Test.NHSpecificTest.NH3079
{
	public class EmployerCpId
	{
		public virtual int IdA { get; set; }

		public virtual int IdB { get; set; }

		public override bool Equals(object obj)
		{
			if (!(obj is EmployerCpId objCpId))
				return false;

			return IdA == objCpId.IdA && IdB == objCpId.IdB;
		}

		public override int GetHashCode()
		{
			return IdA.GetHashCode() ^ IdB.GetHashCode();
		}
	}
}
