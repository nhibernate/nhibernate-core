namespace NHibernate.Test.NHSpecificTest.NH3079
{
	public class EmploymentCpId
	{
		public virtual int Id { get; set; }

		public virtual Person PersonObj { get; set; }

		public virtual Employer EmployerObj { get; set; }

		public override bool Equals(object obj)
		{
			if (!(obj is EmploymentCpId objCpId))
				return false;

			return Id == objCpId.Id && PersonObj.CpId == objCpId.PersonObj.CpId &&
				EmployerObj.CpId == objCpId.EmployerObj.CpId;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ PersonObj.CpId.GetHashCode() ^ EmployerObj.CpId.GetHashCode();
		}
	}
}
