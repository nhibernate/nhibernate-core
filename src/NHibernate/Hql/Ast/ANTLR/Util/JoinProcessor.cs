using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.SqlCommand;

namespace NHibernate.Hql.Ast.ANTLR.Util
{
	/// <summary>
	/// Performs the post-processing of the join information gathered during semantic analysis.
	/// The join generating classes are complex, this encapsulates some of the JoinSequence-related
	/// code.
	/// Author: Joshua Davis
	/// Ported by: Steve Strong
 	/// </summary>
	[CLSCompliant(false)]
	public class JoinProcessor
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(JoinProcessor));

		private readonly HqlSqlWalker _walker;
		private readonly SyntheticAndFactory _syntheticAndFactory;

		/// <summary>
		/// Constructs a new JoinProcessor.
		/// </summary>
		/// <param name="walker">The walker to which we are bound, giving us access to needed resources.</param>
		public JoinProcessor(HqlSqlWalker walker) 
		{
			_walker = walker;
			_syntheticAndFactory = new SyntheticAndFactory( walker );
		}

		/// <summary>
		/// Translates an AST join type (i.e., the token type) into a JoinFragment.XXX join type.
		/// </summary>
		/// <param name="astJoinType">The AST join type (from HqlSqlWalker)</param>
		/// <returns>a JoinType.XXX join type.</returns>
		public static JoinType ToHibernateJoinType(int astJoinType)
		{
			switch (astJoinType)
			{
				case HqlSqlWalker.LEFT_OUTER:
					return JoinType.LeftOuterJoin;
				case HqlSqlWalker.INNER:
					return JoinType.InnerJoin;
				case HqlSqlWalker.RIGHT_OUTER:
					return JoinType.RightOuterJoin;
				case HqlSqlWalker.FULL:
					return JoinType.FullJoin;
				case HqlSqlWalker.CROSS:
					return JoinType.CrossJoin;
				default:
					throw new AssertionFailure("undefined join type " + astJoinType);
			}
		}

		// Since v5.3
		[Obsolete("Use ProcessJoins taking an IRestrictableStatement instead")]
		public void ProcessJoins(QueryNode query)
		{
			IRestrictableStatement rs = query;
			ProcessJoins(rs);
		}

		public void ProcessJoins(IRestrictableStatement query) 
		{
			FromClause fromClause = query.FromClause;
			IList<FromElement> fromElements;
			if ( DotNode.UseThetaStyleImplicitJoins ) 
			{
				// for regression testing against output from the old parser...
				// found it easiest to simply reorder the FromElements here into ascending order
				// in terms of injecting them into the resulting sql ast in orders relative to those
				// expected by the old parser; this is definitely another of those "only needed
				// for regression purposes".  The SyntheticAndFactory, then, simply injects them as it
				// encounters them.
				fromElements = new List<FromElement>();
				var t = fromClause.GetFromElementsTyped();

				for (int i = t.Count - 1; i >= 0; i--)
				{
					fromElements.Add(t[i]);
				}
			}
			else
			{
				fromElements = fromClause.GetFromElementsTyped();

				for (var index = fromElements.Count - 1; index >= 0; index--)
				{
					var fromElement = fromElements[index];
					// We found an implied from element that is used in the WITH clause of another from element, so it need to become part of it's join sequence
					if (fromElement.IsImplied && fromElement.IsPartOfJoinSequence == null)
					{
						var origin = fromElement.Origin;
						while(origin.IsImplied)
						{
							origin = origin.Origin;
							origin.IsPartOfJoinSequence = false;
						}

						if (origin.WithClauseFragment?.Contains(fromElement.TableAlias + ".") == true)
						{
							List<FromElement> elements = new List<FromElement>();
							while(fromElement.IsImplied)
							{
								elements.Add(fromElement);
								// This from element will be rendered as part of the origins join sequence
								fromElement.Text = string.Empty;
								fromElement.IsPartOfJoinSequence = true;
								fromElement = fromElement.Origin;
							}

							for (var i = elements.Count - 1; i >= 0; i--)
							{
								origin.JoinSequence.AddJoin(elements[i]);
							}
						}
					}
				}
			}

			// Iterate through the alias,JoinSequence pairs and generate SQL token nodes.
			foreach (FromElement fromElement in fromElements)
			{
				if(fromElement.IsPartOfJoinSequence == true)
					continue;

				JoinSequence join = fromElement.JoinSequence;

				join.SetSelector(new JoinSequenceSelector(_walker, fromClause, fromElement));

				// the delete and update statements created here will never be executed when IsMultiTable is true,
				// only the where clause will be used by MultiTableUpdateExecutor/MultiTableDeleteExecutor. In that case
				// we have to use the alias from the persister.
				AddJoinNodes( query, join, fromElement);
			}
		}

		private void AddJoinNodes(IRestrictableStatement query, JoinSequence join, FromElement fromElement)
		{
			JoinFragment joinFragment = join.ToJoinFragment(
					_walker.EnabledFilters,
					fromElement.UseFromFragment || fromElement.IsDereferencedBySuperclassOrSubclassProperty,
					fromElement.WithClauseFragment
			);

			SqlString frag = joinFragment.ToFromFragmentString;
			SqlString whereFrag = joinFragment.ToWhereFragmentString;

			// If the from element represents a JOIN_FRAGMENT and it is
			// a theta-style join, convert its type from JOIN_FRAGMENT
			// to FROM_FRAGMENT
			if ( fromElement.Type == HqlSqlWalker.JOIN_FRAGMENT &&
					( join.IsThetaStyle || SqlStringHelper.IsNotEmpty( whereFrag ) ) ) 
			{
				fromElement.Type = HqlSqlWalker.FROM_FRAGMENT;
				fromElement.JoinSequence.SetUseThetaStyle( true ); // this is used during SqlGenerator processing
			}

			// If there is a FROM fragment and the FROM element is an explicit, then add the from part.
			if ( fromElement.UseFromFragment /*&& StringHelper.isNotEmpty( frag )*/ ) 
			{
				SqlString fromFragment = ProcessFromFragment( frag, join ).Trim();
				if ( log.IsDebugEnabled() ) 
				{
					log.Debug("Using FROM fragment [{0}]", fromFragment);
				}

				ProcessDynamicFilterParameters(fromFragment,fromElement,_walker);
			}

			_syntheticAndFactory.AddWhereFragment( 
					joinFragment,
					whereFrag,
					query,
					fromElement,
					_walker
			);
		}

		private static SqlString ProcessFromFragment(SqlString frag, JoinSequence join) 
		{
			SqlString fromFragment = frag.Trim();
			// The FROM fragment will probably begin with ', '.  Remove this if it is present.
			if ( fromFragment.StartsWithCaseInsensitive( ", " ) ) {
				fromFragment = fromFragment.Substring( 2 );
			}
			return fromFragment;
		}

		public static void ProcessDynamicFilterParameters(
				SqlString sqlFragment,
				IParameterContainer container,
				HqlSqlWalker walker) 
		{
			if ( walker.EnabledFilters.Count == 0
					&& ( ! HasDynamicFilterParam( sqlFragment ) )
					&& ( ! ( HasCollectionFilterParam( sqlFragment ) ) ) ) 
			{
				return;
			}

			container.Text = sqlFragment.ToString(); // dynamic-filters are processed altogether by Loader
		}

		private static bool HasDynamicFilterParam(SqlString sqlFragment)
		{
			return !ParserHelper.HasHqlVariable(sqlFragment);
		}

		private static bool HasCollectionFilterParam(SqlString sqlFragment)
		{
			return sqlFragment.IndexOfOrdinal("?") < 0;
		}

		private class JoinSequenceSelector : JoinSequence.ISelector
		{
			private FromClause _fromClause;
			private FromElement _fromElement;
			private HqlSqlWalker _walker;

			internal JoinSequenceSelector(HqlSqlWalker walker, FromClause fromClause, FromElement fromElement)
			{
				_walker = walker;
				_fromClause = fromClause;
				_fromElement = fromElement;
			}

			public bool IncludeSubclasses(string alias) 
			{
				// The uber-rule here is that we need to include  subclass joins if
				// the FromElement is in any way dereferenced by a property from
				// the subclass table; otherwise we end up with column references
				// qualified by a non-existent table reference in the resulting SQL...
				bool containsTableAlias = _fromClause.ContainsTableAlias( alias );

				if ( _fromElement.IsDereferencedBySubclassProperty) 
				{
					// TODO : or should we return 'containsTableAlias'??
					log.Info("forcing inclusion of extra joins [alias={0}, containsTableAlias={1}]", alias, containsTableAlias);
					return true;
				}
				bool shallowQuery = _walker.IsShallowQuery;
				bool includeSubclasses = _fromElement.IncludeSubclasses;
				bool subQuery = _fromClause.IsScalarSubQuery;
				return includeSubclasses && containsTableAlias && !subQuery && !shallowQuery;
			}
		}
	}
}
