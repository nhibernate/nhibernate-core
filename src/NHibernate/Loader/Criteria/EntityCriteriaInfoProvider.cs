using System;
using NHibernate.Persister;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Loader.Criteria
{
	public class EntityCriteriaInfoProvider : ICriteriaInfoProvider, IExtendedCriteriaInfoProvider
	{
		readonly IQueryable persister;

		public EntityCriteriaInfoProvider(IQueryable persister)
		{
			this.persister = persister;
		}

		public String Name
		{
			get
			{
				return persister.EntityName;
			}
		}

		public string[] Spaces
		{
			get
			{
				return persister.QuerySpaces;
			}
		}

		public IPropertyMapping PropertyMapping
		{
			get
			{
				return persister;
			}
		}

		IPersister IExtendedCriteriaInfoProvider.Persister => persister as IPersister;

		public IType GetType(String relativePath)
		{
			return persister.ToType(relativePath);
		}
	}
}
