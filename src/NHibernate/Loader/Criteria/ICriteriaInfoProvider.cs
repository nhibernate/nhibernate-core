using System;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Loader.Criteria
{
    public interface ICriteriaInfoProvider 
    {
        string Name { get; }
        string[] Spaces { get; }
        IPropertyMapping PropertyMapping { get; }
        IType GetType(String relativePath);
    }
}