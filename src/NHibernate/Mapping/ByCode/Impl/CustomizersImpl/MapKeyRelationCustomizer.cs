using System;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class MapKeyRelationCustomizer<TKey> : IMapKeyRelation<TKey>
	{
		private readonly ICustomizersHolder customizersHolder;
		private readonly IModelExplicitDeclarationsHolder explicitDeclarationsHolder;
		private readonly PropertyPath propertyPath;

		public MapKeyRelationCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			this.explicitDeclarationsHolder = explicitDeclarationsHolder;
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		#region IMapKeyRelation<TKey> Members

		public void Element()
		{
			Element(x => { });
		}

		public void Element(Action<IMapKeyMapper> mapping)
		{
			var mapKeyCustomizer = new MapKeyCustomizer(propertyPath, customizersHolder);
			mapping(mapKeyCustomizer);
		}

		public void ManyToMany()
		{
			ManyToMany(x => { });
		}

		public void ManyToMany(Action<IMapKeyManyToManyMapper> mapping)
		{
			var manyToManyCustomizer = new MapKeyManyToManyCustomizer(explicitDeclarationsHolder, propertyPath, customizersHolder);
			mapping(manyToManyCustomizer);
		}

		public void Component(Action<IComponentMapKeyMapper<TKey>> mapping)
		{
			var manyToManyCustomizer = new MapKeyComponentCustomizer<TKey>(explicitDeclarationsHolder, propertyPath, customizersHolder);
			mapping(manyToManyCustomizer);
		}

		#endregion
	}
}