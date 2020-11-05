using System;
using Antlr.Runtime;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public abstract class AbstractSelectExpression : HqlSqlWalkerNode, ISelectExpressionExtension
	{
		private string _alias;
		private int _scalarColumnIndex = -1;

		protected AbstractSelectExpression(IToken token) : base(token)
		{
		}

		public string Alias
		{
			get { return _alias; }
			set { _alias = value; }
		}

		// Since v5.4
		[Obsolete("Use overload with aliasCreator parameter instead.")]
		public void SetScalarColumn(int i)
		{
			_scalarColumnIndex = i;
			SetScalarColumnText(i);
		}

		/// <inheritdoc />
		public string[] SetScalarColumn(int i, Func<int, int, string> aliasCreator)
		{
			_scalarColumnIndex = i;
			return SetScalarColumnText(i, aliasCreator);
		}

		public int ScalarColumnIndex
		{
			get { return _scalarColumnIndex; }
		}

		public bool IsConstructor
		{
			get { return false; }
		}

		public virtual bool IsReturnableEntity
		{
			get { return false; }
		}

		public virtual FromElement FromElement 
		{
			get { return null; }
			set {}
		}

		public virtual bool IsScalar
		{
			get
			{
				// Default implementation:
				// If this node has a data type, and that data type is not an association, then this is scalar.
				return DataType != null && !DataType.IsAssociationType && !(DataType is SubqueryComponentType); // Moved here from SelectClause [jsd]
			}
		}

		// Since v5.4
		[Obsolete("Use overload with aliasCreator parameter instead.")]
		public abstract void SetScalarColumnText(int i);

		// 6.0 TODO: Make abstract
		/// <summary>
		/// Sets the index and text for select expression in the projection list.
		/// </summary>
		/// <param name="i">The index of the select expression in the projection list.</param>
		/// <param name="aliasCreator">The alias creator.</param>
		/// <returns>The generated scalar column names.</returns>
		public virtual string[] SetScalarColumnText(int i, Func<int, int, string> aliasCreator)
		{
#pragma warning disable 618
			SetScalarColumnText(i);
#pragma warning restore 618
			return null;
		}
	}
}
