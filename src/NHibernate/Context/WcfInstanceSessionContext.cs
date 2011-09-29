using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using NHibernate.Engine;

namespace NHibernate.Context
{
    /// <summary>
    /// Provides a <see cref="ISessionFactory.GetCurrentSession()">current session</see>
    /// for the current InstanceContext in WCF. If you have your wcf service set up to run InstanceContextMode = InstanceContextMode.PerSession
    /// this context will allow you to reuse sessions across wcf calls by the same client. 
    /// </summary>
    public class WcfInstanceSessionContext : MapBasedSessionContext
    {
        public WcfInstanceSessionContext(ISessionFactoryImplementor factory) : base(factory) { }

        private static WcfInstanceStateExtension WcfOperationState
        {
            get
            {
                var extension = OperationContext.Current.InstanceContext.Extensions.Find<WcfInstanceStateExtension>();

                if (extension == null)
                {
                    extension = new WcfInstanceStateExtension();
                    OperationContext.Current.InstanceContext.Extensions.Add(extension);
                }

                return extension;
            }
        }

        protected override IDictionary GetMap()
        {
            return WcfOperationState.Map;
        }

        protected override void SetMap(IDictionary value)
        {
            WcfOperationState.Map = value;
        }
    }

    public class WcfInstanceStateExtension : IExtension<InstanceContext>
    {
        public IDictionary Map { get; set; }

        // we don't really need implementations for these methods in this case
        public void Attach(InstanceContext owner) { }
        public void Detach(InstanceContext owner) { }
    }
}
