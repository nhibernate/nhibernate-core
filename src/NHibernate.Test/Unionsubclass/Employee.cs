namespace NHibernate.Test.Unionsubclass
{
	public class Employee: Human
	{
		private double salary;

		public virtual double Salary
		{
			get { return salary; }
			set { salary = value; }
		}
	}
}