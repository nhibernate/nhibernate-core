using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Type;

namespace NHibernate.Mapping.ByCode
{
	// 6.0 TODO: inherit from IColumnsAndFormulasMapper
	public interface IPropertyMapper : IEntityPropertyMapper, IColumnsMapper
	{
		void Type(IType persistentType);
		void Type<TPersistentType>();
		void Type<TPersistentType>(object parameters);
		void Type(System.Type persistentType, object parameters);
		void Length(int length);
		void Precision(short precision);
		void Scale(short scale);
		void NotNullable(bool notnull);
		void Unique(bool unique);
		void UniqueKey(string uniquekeyName);
		void Index(string indexName);
		// 6.0 TODO: remove once inherited from IColumnsAndFormulasMapper
		void Formula(string formula);
		void Update(bool consideredInUpdateQuery);
		void Insert(bool consideredInInsertQuery);
		void Lazy(bool isLazy);
		void Generated(PropertyGeneration generation);
	}

	public static class PropertyMapperExtensions
	{
		// 6.0 TODO: Move to IPropertyMapper
		public static void FetchGroup(this IPropertyMapper mapper, string name)
		{
			if (mapper is PropertyMapper property)
			{
				property.FetchGroup(name);
			}
		}
	}
}
