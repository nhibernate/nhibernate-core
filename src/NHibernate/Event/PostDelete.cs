using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Event
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PostDelete : Attribute
    {
    }
}
