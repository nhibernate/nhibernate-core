using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping;

namespace NHibernate.Tuple.Entity
{
	/// <summary> 
	/// Handles mapping <see cref="EntityMode"/>s to <see cref="IEntityTuplizer"/>s.
	/// </summary>
	/// <remarks>
	/// Most of the handling is really in the super class; here we just create
	/// the tuplizers and add them to the superclass
	/// </remarks>
	//[Serializable]
	//public class EntityEntityModeToTuplizerMapping : EntityModeToTuplizerMapping
	//{
	//  private static readonly System.Type[] ENTITY_TUP_CTOR_SIG = new System.Type[] { typeof(EntityMetamodel), typeof(PersistentClass) };

	//  /// <summary> 
	//  /// Instantiates a EntityEntityModeToTuplizerMapping based on the given
	//  /// entity mapping and metamodel definitions. 
	//  /// </summary>
	//  /// <param name="mappedEntity">The entity mapping definition. </param>
	//  /// <param name="em">The entity metamodel definition. </param>
	//  public EntityEntityModeToTuplizerMapping(PersistentClass mappedEntity, EntityMetamodel em)
	//  {
	//    // create our own copy of the user-supplied tuplizer impl map
	//    System.Collections.IDictionary userSuppliedTuplizerImpls = new System.Collections.Hashtable();
	//    if (mappedEntity.TuplizerMap != null)
	//    {
	//      SupportClass.MapSupport.PutAll(userSuppliedTuplizerImpls, mappedEntity.TuplizerMap);
	//    }

	//    // Build the dynamic-map tuplizer...
	//    ITuplizer dynamicMapTuplizer;
	//    System.Object tempObject;
	//    tempObject = userSuppliedTuplizerImpls[EntityMode.Map];
	//    userSuppliedTuplizerImpls.Remove(EntityMode.Map);
	//    System.String tuplizerImpl = (System.String)tempObject;
	//    if (tuplizerImpl == null)
	//    {
	//      dynamicMapTuplizer = new DynamicMapEntityTuplizer(em, mappedEntity);
	//    }
	//    else
	//    {
	//      dynamicMapTuplizer = buildEntityTuplizer(tuplizerImpl, mappedEntity, em);
	//    }

	//    // then the pojo tuplizer, using the dynamic-map tuplizer if no pojo representation is available
	//    Tuplizer pojoTuplizer = null;
	//    System.Object tempObject2;
	//    tempObject2 = userSuppliedTuplizerImpls[EntityMode.POJO];
	//    userSuppliedTuplizerImpls.Remove(EntityMode.POJO);
	//    tuplizerImpl = ((System.String)tempObject2);
	//    if (mappedEntity.hasPojoRepresentation())
	//    {
	//      if (tuplizerImpl == null)
	//      {
	//        pojoTuplizer = new PojoEntityTuplizer(em, mappedEntity);
	//      }
	//      else
	//      {
	//        pojoTuplizer = buildEntityTuplizer(tuplizerImpl, mappedEntity, em);
	//      }
	//    }
	//    else
	//    {
	//      pojoTuplizer = dynamicMapTuplizer;
	//    }

	//    // then dom4j tuplizer, if dom4j representation is available
	//    Tuplizer dom4jTuplizer = null;
	//    System.Object tempObject3;
	//    tempObject3 = userSuppliedTuplizerImpls[EntityMode.DOM4J];
	//    userSuppliedTuplizerImpls.Remove(EntityMode.DOM4J);
	//    tuplizerImpl = ((System.String)tempObject3);
	//    if (mappedEntity.hasDom4jRepresentation())
	//    {
	//      if (tuplizerImpl == null)
	//      {
	//        dom4jTuplizer = new Dom4jEntityTuplizer(em, mappedEntity);
	//      }
	//      else
	//      {
	//        dom4jTuplizer = buildEntityTuplizer(tuplizerImpl, mappedEntity, em);
	//      }
	//    }
	//    else
	//    {
	//      dom4jTuplizer = null;
	//    }

	//    // put the "standard" tuplizers into the tuplizer map first
	//    if (pojoTuplizer != null)
	//    {
	//      addTuplizer(EntityMode.POJO, pojoTuplizer);
	//    }
	//    if (dynamicMapTuplizer != null)
	//    {
	//      addTuplizer(EntityMode.MAP, dynamicMapTuplizer);
	//    }
	//    if (dom4jTuplizer != null)
	//    {
	//      addTuplizer(EntityMode.DOM4J, dom4jTuplizer);
	//    }

	//    // then handle any user-defined entity modes...
	//    if (!(userSuppliedTuplizerImpls.Count == 0))
	//    {
	//      //UPGRADE_TODO: Method 'java.util.Map.entrySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilMapentrySet'"
	//      System.Collections.IEnumerator itr = new SupportClass.HashSetSupport(userSuppliedTuplizerImpls).GetEnumerator();
	//      //UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
	//      while (itr.MoveNext())
	//      {
	//        //UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
	//        System.Collections.DictionaryEntry entry = (System.Collections.DictionaryEntry)itr.Current;
	//        EntityMode entityMode = (EntityMode)entry.Key;
	//        EntityTuplizer tuplizer = buildEntityTuplizer((System.String)entry.Value, mappedEntity, em);
	//        addTuplizer(entityMode, tuplizer);
	//      }
	//    }
	//  }

	//  private static EntityTuplizer buildEntityTuplizer(System.String className, PersistentClass pc, EntityMetamodel em)
	//  {
	//    try
	//    {
	//      System.Type implClass = ReflectHelper.classForName(className);
	//      return (EntityTuplizer)implClass.GetConstructor(ENTITY_TUP_CTOR_SIG).Invoke(new System.Object[] { em, pc });
	//    }
	//    //UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to 'System.Exception' which has different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
	//    catch (System.Exception t)
	//    {
	//      throw new HibernateException("Could not build tuplizer [" + className + "]", t);
	//    }
	//  }

	//}
}
