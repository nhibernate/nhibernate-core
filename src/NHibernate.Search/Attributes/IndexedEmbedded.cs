using System;

namespace NHibernate.Search.Attributes
{
    /// <summary>
    /// Specifies that an association is to be indexed in the root entity index
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IndexedEmbeddedAttribute : Attribute
    {
    }
}