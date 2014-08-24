using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A value is anything that is persisted by value, instead of
	/// by reference. It is essentially a Hibernate IType, together
	/// with zero or more columns. Values are wrapped by things with 
	/// higher level semantics, for example properties, collections, 
	/// classes.
	/// </summary>
	public interface IValue
	{
		/// <summary>
		/// Gets the number of columns that this value spans in the table.
		/// </summary>
		int ColumnSpan { get; }

		/// <summary>
		/// Gets an <see cref="IEnumerable{ISelectable}"/> of <see cref="Column"/> objects
		/// that this value is stored in.
		/// </summary>
		IEnumerable<ISelectable> ColumnIterator { get; }

		/// <summary>
		/// Gets the <see cref="IType"/> to read/write the Values.
		/// </summary>
		IType Type { get; }

		/// <summary>
		/// Gets the <see cref="Table"/> this Value is stored in.
		/// </summary>
		Table Table { get; }

		bool HasFormula { get; }

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if this Value is unique.
		/// </summary>
		bool IsAlternateUniqueKey { get;}

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if this Value can have
		/// null values.
		/// </summary>
		bool IsNullable { get; }

		bool[] ColumnUpdateability { get; }
		bool[] ColumnInsertability { get; }

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if this is a SimpleValue
		/// that does not involve foreign keys.
		/// </summary>
		bool IsSimpleValue { get; }

		/// <summary>
		/// 
		/// </summary>
		void CreateForeignKey();

		/// <summary>
		/// Determines if the Value is part of a valid mapping.
		/// </summary>
		/// <param name="mapping">The <see cref="IMapping"/> to validate.</param>
		/// <returns>
		/// <see langword="true" /> if the Value is part of a valid mapping, <see langword="false" />
		/// otherwise.
		/// </returns>
		/// <exception cref="MappingException"></exception>
		/// <remarks>
		/// Mainly used to make sure that Value maps to the correct number
		/// of columns.
		/// </remarks>
		bool IsValid(IMapping mapping);

		FetchMode FetchMode { get; }

		void SetTypeUsingReflection(string className, string propertyName, string accesorName);

		object Accept(IValueVisitor visitor);
	}
}