using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Util;

namespace NHibernate.Loader.Entity
{
	public class CascadeEntityLoader : AbstractEntityLoader
	{
		public CascadeEntityLoader(IOuterJoinLoadable persister, CascadingAction action, ISessionFactoryImplementor factory)
			: base(persister, persister.IdentifierType, factory, new CollectionHelper.EmptyMapClass<string, IFilter>())
		{
			JoinWalker walker = new CascadeEntityJoinWalker(persister, action, factory);
			InitFromWalker(walker);

			PostInstantiate();

			log.Debug(string.Format("Static select for action {0} on entity {1}: {2}", action, entityName, SqlString));
		}
	}
}