using System;

namespace NHibernate.Mapping.ByCode
{
	public interface ICollectionElementRelation
	{
		void Element(Action<IElementMapper> mapping);
		void OneToMany(Action<IOneToManyMapper> mapping);
		void ManyToMany(Action<IManyToManyMapper> mapping);
		void Component(Action<IComponentElementMapper> mapping);
		void ManyToAny(System.Type idTypeOfMetaType, Action<IManyToAnyMapper> mapping);
	}

	public interface ICollectionElementRelation<TElement>
	{
		void Element();
		void Element(Action<IElementMapper> mapping);
		void OneToMany();
		void OneToMany(Action<IOneToManyMapper> mapping);
		void ManyToMany();
		void ManyToMany(Action<IManyToManyMapper> mapping);
		void Component(Action<IComponentElementMapper<TElement>> mapping);
		void ManyToAny(System.Type idTypeOfMetaType, Action<IManyToAnyMapper> mapping);
		void ManyToAny<TIdTypeOfMetaType>(Action<IManyToAnyMapper> mapping);
	}
}