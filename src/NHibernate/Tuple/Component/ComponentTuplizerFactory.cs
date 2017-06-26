using System;
using NHibernate.Util;

namespace NHibernate.Tuple.Component
{
	/// <summary>
	/// A registry allowing users to define the default <see cref="IComponentTuplizer"/> class to use per <see cref="EntityMode"/>;.
	/// </summary>
	[Serializable]
	public class ComponentTuplizerFactory 
	{
		static readonly System.Type[] ComponentTuplizerCtorSignature = { typeof(Mapping.Component) };

		public IComponentTuplizer BuildDefaultComponentTuplizer(EntityMode entityMode, Mapping.Component component)
		{
			switch (entityMode)
			{
				case EntityMode.Poco:
					return new PocoComponentTuplizer(component);
				case EntityMode.Map:
					return new DynamicMapComponentTuplizer(component);
				default:
					throw new ArgumentOutOfRangeException(nameof(entityMode), entityMode, null);
			}
		}

		public IComponentTuplizer BuildComponentTuplizer(string tuplizerImpl, Mapping.Component component)
		{
			try
			{
				System.Type implClass = ReflectHelper.ClassForName(tuplizerImpl);
				return (IComponentTuplizer)implClass.GetConstructor(ComponentTuplizerCtorSignature).Invoke(new object[] { component });
			}
			catch (Exception t)
			{
				throw new HibernateException("Could not build tuplizer [" + tuplizerImpl + "]", t);
			}
		}
	}
}
