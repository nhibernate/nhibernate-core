using System;
using NHibernate.Tuple;

namespace NHibernate.Test.DynamicEntity.Tuplizer
{
	public class MyEntityInstantiator : IInstantiator
	{
		private readonly System.Type entityType;

		public MyEntityInstantiator(System.Type entityType)
		{
			this.entityType = entityType;
		}

		public object Instantiate(object id)
		{
			if (typeof(Person).Equals(entityType))
			{
				return ProxyHelper.NewPersonProxy(id);
			}
			if (typeof(Customer).Equals(entityType))
			{
				return ProxyHelper.NewCustomerProxy(id);
			}
			else if (typeof(Company).Equals(entityType))
			{
				return ProxyHelper.NewCompanyProxy(id);
			}
			else if (typeof(Address).Equals(entityType))
			{
				return ProxyHelper.NewAddressProxy(id);
			}
			else
			{
				throw new ArgumentException("unknown entity for instantiation [" + entityType.FullName + "]");
			}
		}

		public object Instantiate()
		{
			return Instantiate(null);
		}

		public bool IsInstance(object obj)
		{
			try
			{
				return entityType.IsInstanceOfType(obj);
			}
			catch (Exception e)
			{
				throw new HibernateException("could not get handle to entity-name as interface : " + e);
			}
		}
	}
}