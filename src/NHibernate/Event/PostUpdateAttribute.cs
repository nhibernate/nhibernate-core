using System;

namespace NHibernate.Event
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PostUpdateAttribute : Attribute
    {
    }
}
