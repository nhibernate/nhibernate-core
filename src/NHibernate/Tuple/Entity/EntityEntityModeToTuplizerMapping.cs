using System;
using System.Collections.Generic;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Tuple.Entity
{
	/// <summary> 
	/// Handles mapping <see cref="EntityMode"/>s to <see cref="IEntityTuplizer"/>s.
	/// </summary>
	/// <remarks>
	/// Most of the handling is really in the super class; here we just create
	/// the tuplizers and add them to the superclass
	/// </remarks>
	[Serializable]
	public class EntityEntityModeToTuplizerMapping : EntityModeToTuplizerMapping
	{
		private static readonly System.Type[] entityTuplizerCTORSignature = new System.Type[] { typeof(EntityMetamodel), typeof(PersistentClass) };

		/// <summary> 
		/// Instantiates a EntityEntityModeToTuplizerMapping based on the given
		/// entity mapping and metamodel definitions. 
		/// </summary>
		/// <param name="mappedEntity">The entity mapping definition. </param>
		/// <param name="em">The entity metamodel definition. </param>
		public EntityEntityModeToTuplizerMapping(PersistentClass mappedEntity, EntityMetamodel em)
		{
			// create our own copy of the user-supplied tuplizer impl map
			Dictionary<EntityMode, string> userSuppliedTuplizerImpls = mappedEntity.TuplizerMap != null
			                                                           	? new Dictionary<EntityMode, string>(
			                                                           	  	mappedEntity.TuplizerMap)
			                                                           	: new Dictionary<EntityMode, string>();

			// Build the dynamic-map tuplizer...
			ITuplizer dynamicMapTuplizer;
			string tuplizerImpl;
			if (!userSuppliedTuplizerImpls.TryGetValue(EntityMode.Map, out tuplizerImpl))
			{
				dynamicMapTuplizer = new DynamicMapEntityTuplizer(em, mappedEntity);
			}
			else
			{
				dynamicMapTuplizer = BuildEntityTuplizer(tuplizerImpl, mappedEntity, em);
				userSuppliedTuplizerImpls.Remove(EntityMode.Map);
			}

			// then the pojo tuplizer, using the dynamic-map tuplizer if no pojo representation is available
			ITuplizer pojoTuplizer;

			string tempObject2;
			userSuppliedTuplizerImpls.TryGetValue(EntityMode.Poco, out tempObject2);
			userSuppliedTuplizerImpls.Remove(EntityMode.Poco);
			tuplizerImpl = tempObject2;
			if (mappedEntity.HasPocoRepresentation)
			{
				if (tuplizerImpl == null)
				{
					pojoTuplizer = new PocoEntityTuplizer(em, mappedEntity);
				}
				else
				{
					pojoTuplizer = BuildEntityTuplizer(tuplizerImpl, mappedEntity, em);
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

			// then handle any user-defined entity modes...
			foreach (KeyValuePair<EntityMode, string> pair in userSuppliedTuplizerImpls)
			{
				IEntityTuplizer tuplizer = BuildEntityTuplizer(pair.Value, mappedEntity, em);
				AddTuplizer(pair.Key, tuplizer);
			}
		}

		private static IEntityTuplizer BuildEntityTuplizer(string className, PersistentClass pc, EntityMetamodel em)
		{
			try
			{
				System.Type implClass = ReflectHelper.ClassForName(className);
				return (IEntityTuplizer)implClass.GetConstructor(entityTuplizerCTORSignature).Invoke(new object[] { em, pc });
			}
			catch (Exception t)
			{
				throw new HibernateException("Could not build tuplizer [" + className + "]", t);
			}
		}

	}
}
