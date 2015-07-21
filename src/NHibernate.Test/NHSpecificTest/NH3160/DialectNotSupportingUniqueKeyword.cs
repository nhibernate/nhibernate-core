using NHibernate.Dialect;

namespace NHibernate.Test.NHSpecificTest.NH3160
{
	class DialectNotSupportingUniqueKeyword : GenericDialect
	{
		public override bool SupportsUnique
		{
			get { return false; }
		}

		public override bool SupportsUniqueConstraintInCreateAlterTable
		{
			get { return false; }
		}
	}
}
