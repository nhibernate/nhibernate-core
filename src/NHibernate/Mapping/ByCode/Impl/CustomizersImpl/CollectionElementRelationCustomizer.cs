using System;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class CollectionElementRelationCustomizer<TElement> : ICollectionElementRelation<TElement>
	{
		private readonly ICustomizersHolder customizersHolder;
		private readonly IModelExplicitDeclarationsHolder explicitDeclarationsHolder;
		private readonly PropertyPath propertyPath;

		public CollectionElementRelationCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			this.explicitDeclarationsHolder = explicitDeclarationsHolder;
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		#region ICollectionElementRelation<TElement> Members

		public void Element(Action<IElementMapper> mapping)
		{
			var collectionElementCustomizer = new CollectionElementCustomizer(propertyPath, customizersHolder);
			mapping(collectionElementCustomizer);
		}

		public void OneToMany(Action<IOneToManyMapper> mapping)
		{
			var oneToManyCustomizer = new OneToManyCustomizer(explicitDeclarationsHolder, propertyPath, customizersHolder);
			mapping(oneToManyCustomizer);
		}

		public void ManyToMany(Action<IManyToManyMapper> mapping)
		{
			var manyToManyCustomizer = new ManyToManyCustomizer(explicitDeclarationsHolder, propertyPath, customizersHolder);
			mapping(manyToManyCustomizer);
		}

		public void Component(Action<IComponentElementMapper<TElement>> mapping)
		{
			explicitDeclarationsHolder.AddAsComponent(typeof (TElement));
			var componetElementCustomizer = new ComponentElementCustomizer<TElement>(explicitDeclarationsHolder, propertyPath, customizersHolder);
			mapping(componetElementCustomizer);
		}

		#endregion
	}
}