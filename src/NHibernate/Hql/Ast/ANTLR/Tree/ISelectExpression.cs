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
		/// Appends AST nodes that represent the columns after the current AST node.
		/// (e.g. 'as col0_O_')
		/// </summary>
		/// <param name="i">The index of the select expression in the projection list.</param>
		void SetScalarColumnText(int i);

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
}

