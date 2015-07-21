using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using NHibernate.Transform;
using NHibernate.Type;

namespace NHibernate.Linq
{
	public class IntermediateHqlTree
	{
		/* NOTE:
		 * Because common understanding of our users, we are flatting the behavior of Skip and Take methods.
		 * In RAM a query like primeNumbers.Skip(2).Take(6).Where(x=> x > 10) has a completely different results than primeNumbers.Where(x=> x > 10).Skip(2).Take(6) that has
		 * different results than primeNumbers.Take(6).Where(x=> x > 10).Skip(2) and so on.
		 * We are flatting/override even the double-usage of Skip and Take in the same query as: primeNumbers.Skip(2).Where(x=> x > 10).Take(6).Skip(3)
		 * We ***shouldn't*** change the behavior of the query just because we are translating it in SQL.
		 */
		private readonly bool _isRoot;
		private readonly List<Action<IQuery, IDictionary<string, Tuple<object, IType>>>> _additionalCriteria = new List<Action<IQuery, IDictionary<string, Tuple<object, IType>>>>();
		private readonly List<LambdaExpression> _listTransformers = new List<LambdaExpression>();
		private readonly List<LambdaExpression> _itemTransformers = new List<LambdaExpression>();
		private readonly List<LambdaExpression> _postExecuteTransformers = new List<LambdaExpression>();
		private bool _hasDistinctRootOperator;
		private HqlExpression _skipCount;
		private HqlExpression _takeCount;
		private HqlHaving _hqlHaving;
		private HqlTreeNode _root;
		private HqlOrderBy _orderBy;

		public HqlTreeNode Root
		{
			get
			{
				ExecuteAddHavingClause(_hqlHaving);
				ExecuteAddOrderBy(_orderBy);
				ExecuteAddSkipClause(_skipCount);
				ExecuteAddTakeClause(_takeCount);
				return _root;
			}
		}

		public HqlTreeBuilder TreeBuilder { get; private set; }

		public IntermediateHqlTree(bool root)
		{
			_isRoot = root;
			TreeBuilder = new HqlTreeBuilder();
			_root = TreeBuilder.Query(TreeBuilder.SelectFrom(TreeBuilder.From()));
		}

		public ExpressionToHqlTranslationResults GetTranslation()
		{
			if (_isRoot)
			{
				DetectOuterExists();
			}

			return new ExpressionToHqlTranslationResults(Root,
														 _itemTransformers,
														 _listTransformers,
														 _postExecuteTransformers,
														 _additionalCriteria);
		}

		public void AddDistinctRootOperator()
		{
			if (!_hasDistinctRootOperator)
			{
				Expression<Func<IEnumerable<object>, IList>> x =
					l => new DistinctRootEntityResultTransformer().TransformList(l.ToList());

				_listTransformers.Add(x);
				_hasDistinctRootOperator = true;
			}
		}

		public void AddItemTransformer(LambdaExpression transformer)
		{
			_itemTransformers.Add(transformer);
		}

		public void AddFromClause(HqlTreeNode from)
		{
			_root.NodesPreOrder.OfType<HqlFrom>().First().AddChild(from);
		}

		public void AddSelectClause(HqlTreeNode select)
		{
			_root.NodesPreOrder.OfType<HqlSelectFrom>().First().AddChild(select);
		}

		public void AddGroupByClause(HqlGroupBy groupBy)
		{
			this._root.AddChild(groupBy);
		}

		public void AddOrderByClause(HqlExpression orderBy, HqlDirectionStatement direction)
		{
			if (_orderBy == null)
				_orderBy = TreeBuilder.OrderBy();

			_orderBy.AddChild(orderBy);
			_orderBy.AddChild(direction);
		}

		public void AddSkipClause(HqlExpression toSkip)
		{
			_skipCount = toSkip;
		}

		public void AddTakeClause(HqlExpression toTake)
		{
			_takeCount = toTake;
		}

		private void ExecuteAddOrderBy(HqlTreeNode orderBy)
		{
			if (orderBy == null)
				return;

			if (_root.NodesPreOrder.All(x => x != orderBy))
				_root.AddChild(orderBy);
		}

		private void ExecuteAddTakeClause(HqlExpression toTake)
		{
			if (toTake == null)
			{
				return;
			}

			var hqlQuery = _root.NodesPreOrder.OfType<HqlQuery>().First();
			var takeRoot = hqlQuery.Children.OfType<HqlTake>().FirstOrDefault();

			// were present we ignore the new value
			if (takeRoot == null)
			{
				//We should check the value instead delegate the behavior to the result SQL-> MSDN: If count is less than or equal to zero, source is not enumerated and an empty IEnumerable<T> is returned.
				takeRoot = TreeBuilder.Take(toTake);
				hqlQuery.AddChild(takeRoot);
			}
		}

		private void ExecuteAddSkipClause(HqlExpression toSkip)
		{
			if (toSkip == null)
			{
				return;
			}
			// We should check the value instead delegate the behavior to the result SQL-> MSDN: If count is less than or equal to zero, all elements of source are yielded.
			var hqlQuery = _root.NodesPreOrder.OfType<HqlQuery>().First();
			var skipRoot = hqlQuery.Children.OfType<HqlSkip>().FirstOrDefault();
			if (skipRoot == null)
			{
				skipRoot = TreeBuilder.Skip(toSkip);
				hqlQuery.AddChild(skipRoot);
			}
		}

		private void ExecuteAddHavingClause(HqlHaving hqlHaving)
		{
			if (hqlHaving == null)
				return;

			if (!_root.NodesPreOrder.OfType<HqlHaving>().Any())
				_root.AddChild(hqlHaving);
		}

		public void AddWhereClause(HqlBooleanExpression where)
		{
			var currentWhere = _root.NodesPreOrder.OfType<HqlWhere>().FirstOrDefault();
			if (currentWhere == null)
			{
				currentWhere = TreeBuilder.Where(where);
				_root.AddChild(currentWhere);
			}
			else
			{
				var currentClause = (HqlBooleanExpression)currentWhere.Children.Single();

				currentWhere.ClearChildren();
				currentWhere.AddChild(TreeBuilder.BooleanAnd(currentClause, where));
			}
		}

		public void AddHavingClause(HqlBooleanExpression where)
		{
			if (_hqlHaving == null)
			{
				_hqlHaving = TreeBuilder.Having(where);
			}
			else
			{
				var currentClause = (HqlBooleanExpression) _hqlHaving.Children.Single();

				_hqlHaving.ClearChildren();
				_hqlHaving.AddChild(TreeBuilder.BooleanAnd(currentClause, where));
			}
		}

		private void DetectOuterExists()
		{
			if (_root is HqlExists)
			{
				_takeCount = TreeBuilder.Constant(1);
				_root = Root.Children.First();

				Expression<Func<IEnumerable<object>, bool>> x = l => l.Any();

				_listTransformers.Add(x);
			}
		}

		public void AddAdditionalCriteria(Action<IQuery, IDictionary<string, Tuple<object, IType>>> criteria)
		{
			_additionalCriteria.Add(criteria);
		}

		public void AddPostExecuteTransformer(LambdaExpression lambda)
		{
			_postExecuteTransformers.Add(lambda);
		}

		public void AddListTransformer(LambdaExpression lambda)
		{
			_listTransformers.Add(lambda);
		}

		public void SetRoot(HqlTreeNode newRoot)
		{
			_root = newRoot;
		}
	}
}