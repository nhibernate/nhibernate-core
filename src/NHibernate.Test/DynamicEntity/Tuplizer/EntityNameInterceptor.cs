namespace NHibernate.Test.DynamicEntity.Tuplizer
{
	public class EntityNameInterceptor : EmptyInterceptor
	{
		public override string GetEntityName(object entity)
		{
			string entityName = ProxyHelper.ExtractEntityName(entity) ?? base.GetEntityName(entity);
			return entityName;
		}
	}
}