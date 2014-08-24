namespace NHibernate.Test.NHSpecificTest.NH1700
{
	public class PayrollSegment
	{
		public virtual string Id { get; set; }
	}
	public class ActualPayrollSegment : PayrollSegment
	{
	}
	public class ProjectedPayrollSegment : PayrollSegment
	{
	}
	public class ClosedPayrollSegment : PayrollSegment
	{
	}
}