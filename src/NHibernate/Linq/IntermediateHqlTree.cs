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
        private readonly bool _root;
        private readonly List<Action<IQuery, IDictionary<string, Tuple<object, IType>>>> _additionalCriteria = new List<Action<IQuery, IDictionary<string, Tuple<object, IType>>>>();
        private readonly List<LambdaExpression> _listTransformers = new List<LambdaExpression>();
        private readonly List<LambdaExpression> _itemTransformers = new List<LambdaExpression>();
        private readonly List<LambdaExpression> _postExecuteTransformers = new List<LambdaExpression>();
        private bool _hasDistinctRootOperator;

        public HqlTreeNode Root { get; private set; }
        public HqlTreeBuilder TreeBuilder { get; private set; }

        public IntermediateHqlTree(bool root)
        {
            _root = root;
            TreeBuilder = new HqlTreeBuilder();
            Root = TreeBuilder.Query(TreeBuilder.SelectFrom(TreeBuilder.From()));
        }

        public ExpressionToHqlTranslationResults GetTranslation()
        {
            if (_root)
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
            Root.NodesPreOrder.Where(n => n is HqlFrom).First().AddChild(from);
        }

        public void AddSelectClause(HqlTreeNode select)
        {
            Root.NodesPreOrder.Where(n => n is HqlSelectFrom).First().AddChild(select);
        }

        public void AddGroupByClause(HqlGroupBy groupBy)
        {
            Root.As<HqlQuery>().AddChild(groupBy);
        }

        public void AddOrderByClause(HqlExpression orderBy, HqlDirectionStatement direction)
        {
            var orderByRoot = Root.NodesPreOrder.Where(n => n is HqlOrderBy).FirstOrDefault();

            if (orderByRoot == null)
            {
                orderByRoot = TreeBuilder.OrderBy();
                Root.As<HqlQuery>().AddChild(orderByRoot);
            }

            orderByRoot.AddChild(orderBy);
            orderByRoot.AddChild(direction);
        }

        public void AddWhereClause(HqlBooleanExpression where)
        {
            var currentWhere = Root.NodesPreOrder.Where(n => n is HqlWhere).FirstOrDefault();

            if (currentWhere == null)
            {
                currentWhere = TreeBuilder.Where(where);
                Root.As<HqlQuery>().AddChild(currentWhere);
            }
            else
            {
                var currentClause = (HqlBooleanExpression)currentWhere.Children.Single();

                currentWhere.ClearChildren();
                currentWhere.AddChild(TreeBuilder.BooleanAnd(currentClause, where));
            }
        }

        private void DetectOuterExists()
        {
            if (Root is HqlExists)
            {
                Root = Root.Children.First();

                _additionalCriteria.Add((q, p) => q.SetMaxResults(1));

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
            Root = newRoot;
        }
    }
}