using NHibernate.Id.Insert;

namespace NHibernate.Id
{
	public interface IPostInsertIdentifierGenerator
	{
		IInsertGeneratedIdentifierDelegate GetInsertGeneratedIdentifierDelegate(IPostInsertIdentityPersister persister, Dialect.Dialect dialect, bool isGetGeneratedKeysEnabled);
	}
}