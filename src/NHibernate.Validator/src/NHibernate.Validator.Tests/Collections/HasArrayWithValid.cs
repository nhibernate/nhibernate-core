namespace NHibernate.Validator.Tests.Collections
{
	public class HasArrayWithValid
	{
		[Valid,Size(Min = 2, Max = 4)]
		public Show[] Shows;
	}
}