using System;
using NHibernate.Hql.Util;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Loader.Criteria
{
	public class ScalarCollectionCriteriaInfoProvider : ICriteriaInfoProvider
	{
		private readonly String role;
		private readonly IQueryableCollection persister;
		private readonly SessionFactoryHelper helper;
		public ScalarCollectionCriteriaInfoProvider(SessionFactoryHelper helper, String role)
		{
			this.role = role;
			this.helper = helper;
			this.persister = helper.RequireQueryableCollection(role);
		}

		public String Name
		{
			get
			{
				return role;
			}
		}

		public string[] Spaces
		{
			get
			{
				return persister.CollectionSpaces;
			}
		}

		public IPropertyMapping PropertyMapping
		{
			get
			{
				return helper.GetCollectionPropertyMapping(role);
			}
		}

		public IType GetType(String relativePath)
		{
			//not sure what things are going to be passed here, how about 'id', maybe 'index' or 'key' or 'elements' ???
			return PropertyMapping.ToType(relativePath);
		}

	}
}