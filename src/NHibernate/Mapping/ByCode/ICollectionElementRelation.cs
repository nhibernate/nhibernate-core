using System;

namespace NHibernate.Mapping.ByCode
{
	public interface ICollectionElementRelation
	{
		void Element(Action<IElementMapper> mapping);
		void OneToMany(Action<IOneToManyMapper> mapping);
		void ManyToMany(Action<IManyToManyMapper> mapping);
		void Component(Action<IComponentElementMapper> mapping);
	}

	public interface ICollectionElementRelation<TElement>
	{
		void Element(Action<IElementMapper> mapping);
		void OneToMany(Action<IOneToManyMapper> mapping);
		void ManyToMany(Action<IManyToManyMapper> mapping);
		void Component(Action<IComponentElementMapper<TElement>> mapping);
	}
}