using NHibernate.Linq.Functions;

namespace NHibernate.Test.NHSpecificTest.GH9999
{
	public class EnhancedLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
	{
		public EnhancedLinqToHqlGeneratorsRegistry()
		{
			this.Merge(new DateTimeDayOfYearPropertyHqlGenerator());
			this.Merge(new DateTimeAddYearsMethodHqlGenerator());
		}
	}
}
