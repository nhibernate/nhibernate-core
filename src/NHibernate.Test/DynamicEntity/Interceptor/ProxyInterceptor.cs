using System;

namespace NHibernate.Test.DynamicEntity.Interceptor
{
	[Obsolete("Require dynamic proxies")]
	public class ProxyInterceptor : EmptyInterceptor
	{
		public override string GetEntityName(object entity)
		{
			string entityName = ProxyHelper.ExtractEntityName(entity) ?? base.GetEntityName(entity);
			return entityName;
		}

		public override object Instantiate(string entityName, object id)
		{
			if (typeof(Customer).FullName.Equals(entityName))
			{
				return ProxyHelper.NewCustomerProxy(id);
			}
			else if (typeof(Company).FullName.Equals(entityName))
			{
				return ProxyHelper.NewCompanyProxy(id);
			}
			return base.Instantiate(entityName, id);
		}
	}
}
