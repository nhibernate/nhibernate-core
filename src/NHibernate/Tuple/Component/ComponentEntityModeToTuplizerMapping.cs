using System;
using System.Collections.Generic;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Tuple.Component
{
	/// <summary> 
	/// Handles mapping <see cref="EntityMode"/>s to <see cref="IComponentTuplizer">ComponentTuplizers</see>.
	/// <p/>
	/// Most of the handling is really in the super class; here we just create
	/// the tuplizers and add them to the superclass
	/// </summary>
	[Serializable]
	public class ComponentEntityModeToTuplizerMapping : EntityModeToTuplizerMapping
	{
		private static readonly System.Type[] componentTuplizerCTORSignature = new System.Type[] { typeof(Mapping.Component) };

		public ComponentEntityModeToTuplizerMapping(Mapping.Component component)
		{
			PersistentClass owner = component.Owner;

			// create our own copy of the user-supplied tuplizer impl map
			Dictionary<EntityMode, string> userSuppliedTuplizerImpls;
			if (component.TuplizerMap != null)
				userSuppliedTuplizerImpls = new Dictionary<EntityMode, string>(component.TuplizerMap);
			else
				userSuppliedTuplizerImpls = new Dictionary<EntityMode, string>();

			// Build the dynamic-map tuplizer...
			ITuplizer dynamicMapTuplizer;
			string tuplizerImpl;
			if (!userSuppliedTuplizerImpls.TryGetValue(EntityMode.Map,out tuplizerImpl))
			{
				dynamicMapTuplizer = new DynamicMapComponentTuplizer(component);
			}
			else
			{
				dynamicMapTuplizer = BuildComponentTuplizer(tuplizerImpl, component);
				userSuppliedTuplizerImpls.Remove(EntityMode.Map);
			}

			// then the pojo tuplizer, using the dynamic-map tuplizer if no pojo representation is available
			ITuplizer pojoTuplizer;
			string tempObject2;
			userSuppliedTuplizerImpls.TryGetValue(EntityMode.Poco, out tempObject2);
			userSuppliedTuplizerImpls.Remove(EntityMode.Poco);
			tuplizerImpl = tempObject2;
			if (owner.HasPocoRepresentation && component.HasPocoRepresentation)
			{
				if (tuplizerImpl == null)
				{
					pojoTuplizer = new PocoComponentTuplizer(component);
				}
				else
				{
					pojoTuplizer = BuildComponentTuplizer(tuplizerImpl, component);
				}
			}
			else
			{
				pojoTuplizer = dynamicMapTuplizer;
			}

			// put the "standard" tuplizers into the tuplizer map first
			if (pojoTuplizer != null)
			{
				AddTuplizer(EntityMode.Poco, pojoTuplizer);
			}
			if (dynamicMapTuplizer != null)
			{
				AddTuplizer(EntityMode.Map, dynamicMapTuplizer);
			}

			// then handle any user-defined entity modes..
			foreach (KeyValuePair<EntityMode, string> entry in userSuppliedTuplizerImpls)
			{
				IComponentTuplizer tuplizer = BuildComponentTuplizer(entry.Value, component);
				AddTuplizer(entry.Key, tuplizer);
			}
		}

		private IComponentTuplizer BuildComponentTuplizer(string tuplizerImpl, Mapping.Component component)
		{
			try
			{
				System.Type implClass = ReflectHelper.ClassForName(tuplizerImpl);
				return (IComponentTuplizer)implClass.GetConstructor(componentTuplizerCTORSignature).Invoke(new object[] { component });
			}
			catch (Exception t)
			{
				throw new HibernateException("Could not build tuplizer [" + tuplizerImpl + "]", t);
			}
		}
	}
}
