using NHibernate.Shards.Session;

namespace NHibernate.Shards.Id
{
	public interface IGeneratorRequiringControlSessionProvider
	{
		void SetControlSessionProvider(IControlSessionProvider provider);
	}
}