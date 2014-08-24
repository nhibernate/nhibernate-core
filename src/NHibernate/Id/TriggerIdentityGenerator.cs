using NHibernate.Engine;
using NHibernate.Id.Insert;

namespace NHibernate.Id
{
	public class TriggerIdentityGenerator : AbstractPostInsertGenerator
	{
		#region Overrides of AbstractPostInsertGenerator

		public override IInsertGeneratedIdentifierDelegate GetInsertGeneratedIdentifierDelegate(
			IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory, bool isGetGeneratedKeysEnabled)
		{
			return new OutputParamReturningDelegate(persister, factory);
		}

		#endregion
	}
}