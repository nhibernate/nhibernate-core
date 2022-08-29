using NHibernate.Engine;
using NHibernate.Id.Insert;

namespace NHibernate.Id
{
	public interface IPostInsertIdentifierGenerator : IIdentifierGenerator
	{
		IInsertGeneratedIdentifierDelegate GetInsertGeneratedIdentifierDelegate(IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory, bool isGetGeneratedKeysEnabled);
	}
}
