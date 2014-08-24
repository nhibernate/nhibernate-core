using NHibernate.Type;
namespace NHibernate.Test.Interceptor
{
	public class CollectionInterceptor : EmptyInterceptor
	{
		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
		{
			((User)entity).Actions.Add("updated");
			return false;
		}

		public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			((User)entity).Actions.Add("created");
			return false;
		}
	}
}