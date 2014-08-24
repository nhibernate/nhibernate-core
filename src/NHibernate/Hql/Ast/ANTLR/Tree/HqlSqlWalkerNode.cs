using System;
using Antlr.Runtime;
using NHibernate.Hql.Ast.ANTLR.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// A semantic analysis node, that points back to the main analyzer.
	/// Authoer: josh
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class HqlSqlWalkerNode : SqlNode, IInitializableNode
	{
		/**
		 * A pointer back to the phase 2 processor.
		 */
		private HqlSqlWalker _walker;

		public HqlSqlWalkerNode(IToken token) : base(token)
		{
		}

		public virtual void Initialize(object param)
		{
			_walker = (HqlSqlWalker)param;
		}

		public HqlSqlWalker Walker
		{
			get { return _walker; }
		}

		internal SessionFactoryHelperExtensions SessionFactoryHelper
		{
			get { return _walker.SessionFactoryHelper; }
		}

		
		public IASTFactory ASTFactory
		{
			get { return _walker.ASTFactory; }
		}
		
		public AliasGenerator AliasGenerator
		{
			get { return _walker.AliasGenerator; }
		}
	}
}