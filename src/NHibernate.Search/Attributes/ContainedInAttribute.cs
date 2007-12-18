using System;

namespace NHibernate.Search.Attributes
{
    /// <summary>
    /// Describe the owning entity as being part of the target entity's
    /// index (to be more accurate, being part of the indexed object graph)
    /// 
    /// Only necessary when an @Indexed class is used as a <see cref="IndexedEmbeddedAttribute" />
    /// target class. ContainedIn must mark the property pointing back
    /// to the IndexedEmbedded owning Entity
    /// 
    /// Not necessary if the class is an Embeddable class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ContainedInAttribute : Attribute
    {
    }
}