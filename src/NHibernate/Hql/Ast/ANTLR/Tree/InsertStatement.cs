using System;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary> 
	/// Defines a top-level AST node representing an HQL "insert select" statement. 
	/// </summary>
	[Serializable]
	[CLSCompliant(false)]
	public class InsertStatement : AbstractStatement
	{
		public InsertStatement(IToken token) : base(token) {}
		public override bool NeedsExecutor
		{
			get { return true; }
		}

		public override int StatementType
		{
			get { return HqlSqlWalker.INSERT; }
		}

		/// <summary> Retreive this insert statement's into-clause. </summary>
		/// <returns> The into-clause </returns>
		public IntoClause IntoClause
		{
			get{return (IntoClause)GetFirstChild();}
		}

		/// <summary> Retreive this insert statement's select-clause.</summary>
		/// <returns> The select-clause. </returns>
		public SelectClause SelectClause
		{
			get{return ((QueryNode)IntoClause.NextSibling).GetSelectClause();}
		}

		/// <summary> Performs detailed semantic validation on this insert statement tree. </summary>
		/// <exception cref="QueryException">Indicates validation failure.</exception>
		public virtual void Validate()
		{
			IntoClause.ValidateTypes(SelectClause);
		}
	}
}