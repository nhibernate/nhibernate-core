using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace NHibernate.Context
{
    public class WcfInstanceStateExtension : IExtension<InstanceContext>
    {
        public IDictionary Map { get; set; }

        // we don't really need implementations for these methods in this case
        public void Attach(InstanceContext owner) { }
        public void Detach(InstanceContext owner) { }
    }
}
