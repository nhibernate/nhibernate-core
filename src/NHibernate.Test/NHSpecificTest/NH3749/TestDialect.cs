
namespace NHibernate.Test.NHSpecificTest.NH3749
{
	public class TestDialect : Dialect.Dialect
	{
		public override bool SupportsNullInUnique
		{
			get { return false; }
		}
	}
}
