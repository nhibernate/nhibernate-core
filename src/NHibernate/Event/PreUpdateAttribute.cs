using System;

namespace NHibernate.Event
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PreUpdateAttribute : Attribute
    {
    }
}
