namespace NHibernate.Shards.Session
{
	public abstract class BaseStatefulInterceptorFactory : EmptyInterceptor, IStatefulInterceptorFactory
	{
		public abstract IInterceptor NewInstance();
	}
}