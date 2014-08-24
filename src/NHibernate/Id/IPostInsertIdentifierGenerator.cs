using NHibernate.Id.Insert;
using NHibernate.Engine;

namespace NHibernate.Id
{
	public interface IPostInsertIdentifierGenerator : IIdentifierGenerator
	{
		IInsertGeneratedIdentifierDelegate GetInsertGeneratedIdentifierDelegate(IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory, bool isGetGeneratedKeysEnabled);
	}
}