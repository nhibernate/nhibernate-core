using Antlr.Runtime.Tree;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Persister.Collection;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NHibernate.Hql.Ast.ANTLR
{
	internal static class HqlFilterPreprocessor
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(HqlFilterPreprocessor));

		/// <summary>
		/// Handles HQL AST transformation for collection filters (which are created with <see cref="ISession.CreateFilter"/>).
		/// 
		/// Adds FROM elements to implicit FROM clause.
		/// E.g., 
		/// <code>
		/// ( query ( SELECT_FROM {filter-implied FROM} ) ( where ( = X 10 ) ) )
		/// </code>
		/// gets converted to 
		/// <code>
		/// ( query ( SELECT_FROM ( FROM NHibernate.DomainModel.Many this ) ) ( where ( = X 10 ) ) )
		/// </code>
		/// </summary>
		/// <param name="ast">The root node of HQL query</param>
		/// <param name="collectionRole">Collection that is being filtered</param>
		/// <param name="factory">Session factory</param>
		internal static void AddImpliedFromToQuery(IASTNode ast, string collectionRole, ISessionFactoryImplementor factory)
		{
			if (ast.Type != HqlParser.QUERY)
			{
				var msg = string.Format(CultureInfo.InvariantCulture,
					"invalid query type for collection filtering: expected {0}, got {1}", HqlParser.tokenNames[HqlParser.QUERY], HqlParser.tokenNames[ast.Type]);
				throw new QueryException(msg);
			}
			var selectFromClause = ast.Where(x => x.Type == HqlParser.SELECT_FROM).Single();
			var fromClause = selectFromClause.Where(x => x.Type == HqlParser.FROM).Single();
			
			fromClause.Text = "FROM"; // Just for prettier debug output
			AddImpliedFromToFromNode(fromClause, collectionRole, factory);
		}

		private static void AddImpliedFromToFromNode(IASTNode fromClause, string collectionRole, ISessionFactoryImplementor factory)
		{
			SessionFactoryHelperExtensions _sessionFactoryHelper = new SessionFactoryHelperExtensions(factory);
			IQueryableCollection persister = _sessionFactoryHelper.GetCollectionPersister(collectionRole);
			IType collectionElementType = persister.ElementType;
			if (!collectionElementType.IsEntityType)
			{
				throw new QueryException("collection of values in filter: this");
			}

			string collectionElementEntityName = persister.ElementPersister.EntityName;

			ITreeAdaptor adaptor = new ASTTreeAdaptor();

			IASTNode fromElement = (IASTNode)adaptor.Create(HqlParser.FILTER_ENTITY, collectionElementEntityName);
			IASTNode alias = (IASTNode)adaptor.Create(HqlParser.ALIAS, "this");

			fromClause.AddChild(fromElement);
			fromClause.AddChild(alias);

			// Show the modified AST.
			if (log.IsDebugEnabled)
			{
				log.Debug("AddImpliedFormToFromNode() : Filter - Added 'this' as a from element...");
			}
		}
	}
}
