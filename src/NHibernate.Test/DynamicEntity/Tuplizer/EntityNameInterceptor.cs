using System;

namespace NHibernate.Test.DynamicEntity.Tuplizer
{
	[Obsolete("Require dynamic proxies")]
	public class EntityNameInterceptor : EmptyInterceptor
	{
		public override string GetEntityName(object entity)
		{
			string entityName = ProxyHelper.ExtractEntityName(entity) ?? base.GetEntityName(entity);
			return entityName;
		}
	}
}
