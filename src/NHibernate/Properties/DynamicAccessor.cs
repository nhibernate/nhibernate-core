using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace NHibernate.Properties
{
    [Serializable()]
    public class DynamicAccessor : IPropertyAccessor
    {
        #region IPropertyAccessor Members

        public IGetter GetGetter(System.Type theClass, string propertyName)
        {
            return new DynamicGetter(propertyName);
        }

        public ISetter GetSetter(System.Type theClass, string propertyName)
        {
            return new DynamicSetter(propertyName);
        }

        public bool CanAccessThroughReflectionOptimizer
        {
            get { return false; }
        }

        #endregion

        [Serializable()]
        public sealed class DynamicGetter : IGetter
        {
            private readonly string name;

            public DynamicGetter(string name)
            {
                this.name = name;
            }

            public object Get(object target)
            {
                var binder = Binder.GetMember(CSharpBinderFlags.None, this.name, target.GetType(),
                new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
                var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);

                object ret = null;
                try
                {

                    ret = callsite.Target(callsite, target);
                }
                catch { }

                return ret;
                
            }

            public System.Type ReturnType
            {
                get
                {
                    return null;
                }
            }

            public string PropertyName
            {
                get { return name; }
            }

            public System.Reflection.MethodInfo Method
            {
                get { throw new NotImplementedException(); }
            }

            public object GetForInsert(object owner, System.Collections.IDictionary mergeMap, Engine.ISessionImplementor session)
            {
                return Get(owner);
            }
        }

        [Serializable()]
        public sealed class DynamicSetter : ISetter
        {
            private readonly string name;

            public DynamicSetter(string name)
            {
                this.name = name;
            }

            public void Set(object target, object value)
            {
                var binder = Binder.SetMember(CSharpBinderFlags.None, this.name, target.GetType(), new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null) });
                var callsite = CallSite<Func<CallSite, object, object, object>>.Create(binder);
                callsite.Target(callsite, target, value);
            }

            public string PropertyName
            {
                get { return name; }
            }

            public System.Reflection.MethodInfo Method
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}
