using System;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Tuple.Entity
{
	/// <summary>
	/// A registry allowing users to define the default <see cref="IEntityTuplizer"/> class to use per <see cref="EntityMode"/>.
	/// </summary>
	[Serializable]
	public class EntityTuplizerFactory 
	{
		static readonly System.Type[] EntityTuplizerCtorSignature = { typeof(EntityMetamodel), typeof(PersistentClass) };

		public IEntityTuplizer BuildEntityTuplizer(string className, EntityMetamodel em, PersistentClass pc)
		{
			try
			{
				System.Type implClass = ReflectHelper.ClassForName(className);
				return (IEntityTuplizer)implClass.GetConstructor(EntityTuplizerCtorSignature).Invoke(new object[] { em, pc });
			}
			catch (Exception t)
			{
				throw new HibernateException("Could not build tuplizer [" + className + "]", t);
			}
		}

		public IEntityTuplizer BuildDefaultEntityTuplizer(EntityMode entityMode, EntityMetamodel entityMetamodel, PersistentClass persistentClass)
		{
			switch (entityMode)
			{
				case EntityMode.Poco:
					return new PocoEntityTuplizer(entityMetamodel, persistentClass);
				case EntityMode.Map:
					return new DynamicMapEntityTuplizer(entityMetamodel, persistentClass);
				default:
					throw new ArgumentOutOfRangeException(nameof(entityMode), entityMode, null);
			}
		}
	}
}
