using System;
using NHibernate.Type;

namespace NHibernate.Mapping.ByCode
{
	public interface IAnyMapper : IEntityPropertyMapper
	{
		void MetaType(IType metaType);
		void MetaType<TMetaType>();
		void MetaType(System.Type metaType);

		void IdType(IType idType);
		void IdType<TIdType>();
		void IdType(System.Type idType);

		void Columns(Action<IColumnMapper> idColumnMapping, Action<IColumnMapper> classColumnMapping);

		/// <summary>
		/// Add or modify a value-class pair.
		/// </summary>
		/// <param name="value">The value of the DB-field dor a given association instance (should override <see cref="object.ToString"/>)</param>
		/// <param name="entityType">The class associated to the specific <paramref name="value"/>. </param>
		void MetaValue(object value, System.Type entityType);


		void Cascade(Cascade cascadeStyle);
		void Index(string indexName);
		void Lazy(bool isLazy);
		void Update(bool consideredInUpdateQuery);
		void Insert(bool consideredInInsertQuery);
	}
}