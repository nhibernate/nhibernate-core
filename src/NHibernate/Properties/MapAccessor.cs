using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using NHibernate.Engine;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace NHibernate.Properties
{
	[Serializable]
	public class MapAccessor : IPropertyAccessor
	{
		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			return new MapGetter(propertyName);
		}

		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			return new MapSetter(propertyName);
		}

		public bool CanAccessThroughReflectionOptimizer => false;

		[Serializable]
		public sealed class MapSetter : ISetter
		{
			private static readonly ConcurrentDictionary<Tuple<System.Type, string>, CallSite<Func<CallSite, object, object, object>>>
				SetMemberSites = new ConcurrentDictionary<Tuple<System.Type, string>, CallSite<Func<CallSite, object, object, object>>>();

			private readonly string name;

			internal MapSetter(string name)
			{
				this.name = name;
			}

			public MethodInfo Method => null;

			public string PropertyName => null;

			public void Set(object target, object value)
			{
				switch (target)
				{
					case IDictionary d:
						d[name] = value;
						break;
					case IDictionary<string, object> d:
						d[name] = value;
						break;
					default:
						var site = SetMemberSites.GetOrAdd(
							System.Tuple.Create(target.GetType(), name),
							t => CallSite<Func<CallSite, object, object, object>>.Create(
								Binder.GetMember(
									CSharpBinderFlags.None,
									t.Item2,
									t.Item1,
									new[]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									})));

						site.Target(site, target, value);
						break;
				}
			}
		}

		[Serializable]
		public sealed class MapGetter : IGetter
		{
			private static readonly ConcurrentDictionary<Tuple<System.Type, string>, CallSite<Func<CallSite, object, object>>>
				GetMemberSites = new ConcurrentDictionary<Tuple<System.Type, string>, CallSite<Func<CallSite, object, object>>>();

			private readonly string name;

			internal MapGetter(string name)
			{
				this.name = name;
			}

			public MethodInfo Method => null;

			public string PropertyName => null;

			public System.Type ReturnType => typeof(object);

			public object GetForInsert(object owner, IDictionary mergeMap, ISessionImplementor session)
			{
				return Get(owner);
			}

			public object Get(object target)
			{
				switch (target)
				{
					case IDictionary d:
						return d[name];
					case IDictionary<string, object> d:
						d.TryGetValue(name, out var result);
						return result;
					default:
						var site = GetMemberSites.GetOrAdd(
							System.Tuple.Create(target.GetType(), name),
							t => CallSite<Func<CallSite, object, object>>.Create(
								Binder.GetMember(
									CSharpBinderFlags.None,
									t.Item2,
									t.Item1,
									new[] {CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)})));

						return site.Target(site, target);
				}
			}
		}
	}
}
