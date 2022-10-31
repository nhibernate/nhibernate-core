using System;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents an element of a projection list, i.e. a select expression.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public interface ISelectExpression
	{
		/// <summary>
		/// Returns the data type of the select expression.
		/// </summary>
		IType DataType { get; }

		/// <summary>
		/// Set the scalar column index and appends AST nodes that represent the columns after the current AST node.
		/// (e.g. 'as col0_O_')
		/// </summary>
		/// <param name="i">The index of the select expression in the projection list.</param>
		// Since v5.4
		[Obsolete("This method has no more usage in NHibernate and will be removed in a future version.")]
		void SetScalarColumnText(int i);

		/// <summary>
		/// Sets the index and text for select expression in the projection list.
		/// </summary>
		/// <param name="i">The index of the select expression in the projection list.</param>
		// Since v5.4
		[Obsolete("Use SetScalarColumn extension method with aliasCreator parameter instead.")]
		void SetScalarColumn(int i);

		/// <summary>
		/// Gets index of the select expression in the projection list.
		/// </summary>
		/// <value>The index of the select expression in the projection list.</value>
		int ScalarColumnIndex { get; }

		/// <summary>
		/// Returns the FROM element that this expression refers to.
		/// </summary>
		FromElement FromElement { get; }

		/// <summary>
		/// Returns true if the element is a constructor (e.g. new Foo).
		/// </summary>
		bool IsConstructor { get; }

		/// <summary>
		/// Returns true if this select expression represents an entity that can be returned.
		/// </summary>
		bool IsReturnableEntity { get; }

		/// <summary>
		/// Sets the text of the node.
		/// </summary>
		string Text { set; }

		bool IsScalar { get; }

		string Alias { get; set; }
	}

	// 6.0 TODO: Merge into ISelectExpression
	internal interface ISelectExpressionExtension : ISelectExpression
	{
		/// <summary>
		/// Sets the index and text for select expression in the projection list.
		/// </summary>
		/// <param name="i">The index of the select expression in the projection list.</param>
		/// <param name="aliasCreator">The alias creator.</param>
		string[] SetScalarColumn(int i, Func<int, int, string> aliasCreator);
	}

	// 6.0 TODO: Remove
	public static class SelectExpressionExtensions
	{
		/// <summary>
		/// Sets the index and text for select expression in the projection list.
		/// </summary>
		/// <param name="selectExpression">The select expression.</param>
		/// <param name="i">The index of the select expression in the projection list.</param>
		/// <param name="aliasCreator">The alias creator.</param>
		public static string[] SetScalarColumn(this ISelectExpression selectExpression, int i, Func<int, int, string> aliasCreator)
		{
			if (selectExpression is ISelectExpressionExtension selectExpressionExtension)
			{
				return selectExpressionExtension.SetScalarColumn(i, aliasCreator);
			}

#pragma warning disable 618
			selectExpression.SetScalarColumn(i);
#pragma warning restore 618
			return null;
		}
	}
}
