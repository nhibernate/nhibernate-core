using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Expressions;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;

namespace NHibernate.Linq.Visitors
{
    public class HqlGeneratorExpressionTreeVisitor : NhThrowingExpressionTreeVisitor
    {
        protected readonly HqlTreeBuilder _hqlTreeBuilder;
        protected readonly HqlNodeStack _stack;
    	private readonly IDictionary<ConstantExpression, NamedParameter> _parameters;
    	private readonly IList<NamedParameterDescriptor> _requiredHqlParameters;
        static private readonly MethodGeneratorRegistry _methodGeneratorRegistry = MethodGeneratorRegistry.Initialise();

        public HqlGeneratorExpressionTreeVisitor(IDictionary<ConstantExpression, NamedParameter> parameters, IList<NamedParameterDescriptor> requiredHqlParameters)
        {
			_parameters = parameters;
			_requiredHqlParameters = requiredHqlParameters;
			_hqlTreeBuilder = new HqlTreeBuilder();
            _stack = new HqlNodeStack(_hqlTreeBuilder);
        }

        public IEnumerable<HqlTreeNode> GetHqlTreeNodes()
        {
            return _stack.Finish();
        }

        public virtual void Visit(Expression expression)
        {
            VisitExpression(expression);
        }

        public HqlNodeStack Stack
        {
            get { return _stack; }
        }

        public HqlTreeBuilder TreeBuilder
        {
            get { return _hqlTreeBuilder; }
        }

        protected override Expression VisitNhAverage(NhAverageExpression expression)
        {
            var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
            visitor.Visit(expression.Expression);

            _stack.PushLeaf(_hqlTreeBuilder.Cast(_hqlTreeBuilder.Average(visitor.GetHqlTreeNodes().Single()), expression.Type));

            return expression;
        }

        protected override Expression VisitNhCount(NhCountExpression expression)
        {
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
            visitor.Visit(expression.Expression);

            _stack.PushLeaf(_hqlTreeBuilder.Cast(_hqlTreeBuilder.Count(visitor.GetHqlTreeNodes().Single()), expression.Type));

            return expression;
        }

        protected override Expression VisitNhMin(NhMinExpression expression)
        {
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
            visitor.Visit(expression.Expression);

            _stack.PushLeaf(_hqlTreeBuilder.Cast(_hqlTreeBuilder.Min(visitor.GetHqlTreeNodes().Single()), expression.Type));

            return expression;
        }

        protected override Expression VisitNhMax(NhMaxExpression expression)
        {
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
            visitor.Visit(expression.Expression);

            _stack.PushLeaf(_hqlTreeBuilder.Cast(_hqlTreeBuilder.Max(visitor.GetHqlTreeNodes().Single()), expression.Type));

            return expression;
        }

        protected override Expression VisitNhSum(NhSumExpression expression)
        {
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
            visitor.Visit(expression.Expression);

            _stack.PushLeaf(_hqlTreeBuilder.Cast(_hqlTreeBuilder.Sum(visitor.GetHqlTreeNodes().Single()), expression.Type));

            return expression;
        }

        protected override Expression VisitNhDistinct(NhDistinctExpression expression)
        {
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
            visitor.Visit(expression.Expression);

            _stack.PushLeaf(_hqlTreeBuilder.Distinct());

            foreach (var node in visitor.GetHqlTreeNodes())
            {
                _stack.PushLeaf(node);
            }

            return expression;
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            _stack.PushLeaf(_hqlTreeBuilder.Ident(expression.ReferencedQuerySource.ItemName));

            return expression;
        }

        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            HqlTreeNode operatorNode = GetHqlOperatorNodeForBinaryOperator(expression);

            using (_stack.PushNode(operatorNode))
            {
                VisitExpression(expression.Left);

                VisitExpression(expression.Right);
            }

            return expression;
        }

        private HqlTreeNode GetHqlOperatorNodeForBinaryOperator(BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                    return _hqlTreeBuilder.Equality();

                case ExpressionType.NotEqual:
                    return _hqlTreeBuilder.Inequality();

                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return _hqlTreeBuilder.BooleanAnd();

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return _hqlTreeBuilder.BooleanOr();

                case ExpressionType.Add:
                    return _hqlTreeBuilder.Add();

                case ExpressionType.Subtract:
                    return _hqlTreeBuilder.Subtract();

                case ExpressionType.Multiply:
                    return _hqlTreeBuilder.Multiply();

                case ExpressionType.Divide:
                    return _hqlTreeBuilder.Divide();

                case ExpressionType.LessThan:
                    return _hqlTreeBuilder.LessThan();

                case ExpressionType.LessThanOrEqual:
                    return _hqlTreeBuilder.LessThanOrEqual();

                case ExpressionType.GreaterThan:
                    return _hqlTreeBuilder.GreaterThan();

                case ExpressionType.GreaterThanOrEqual:
                    return _hqlTreeBuilder.GreaterThanOrEqual();
            }

            throw new InvalidOperationException();
        }

        protected override Expression VisitUnaryExpression(UnaryExpression expression)
        {
            HqlTreeNode operatorNode = GetHqlOperatorNodeforUnaryOperator(expression);

            using (_stack.PushNode(operatorNode))
            {
                VisitExpression(expression.Operand);
            }

            return expression;
        }

        private HqlTreeNode GetHqlOperatorNodeforUnaryOperator(UnaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Not:
                    return _hqlTreeBuilder.Not();
            }
            
            throw new InvalidOperationException();
        }

        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            using (_stack.PushNode(_hqlTreeBuilder.Dot()))
            {
                Expression newExpression = VisitExpression(expression.Expression);

                _stack.PushLeaf(_hqlTreeBuilder.Ident(expression.Member.Name));

                if (newExpression != expression.Expression)
                {
                    return Expression.MakeMemberAccess(newExpression, expression.Member);
                }
            }

            return expression;
        }

        protected override Expression VisitConstantExpression(ConstantExpression expression)
        {
            if (expression.Value != null)
            {
                System.Type t = expression.Value.GetType();

                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof (NhQueryable<>))
                {
                    _stack.PushLeaf(_hqlTreeBuilder.Ident(t.GetGenericArguments()[0].Name));
                    return expression;
                }
            }

        	NamedParameter namedParameter;

			if (_parameters.TryGetValue(expression, out namedParameter))
			{
				_stack.PushLeaf(_hqlTreeBuilder.Cast(_hqlTreeBuilder.Parameter(namedParameter.Name), namedParameter.Value.GetType()));
				_requiredHqlParameters.Add(new NamedParameterDescriptor(namedParameter.Name, null, new []{ _requiredHqlParameters.Count + 1}, false));
			}
			else
			{
				_stack.PushLeaf(_hqlTreeBuilder.Constant(expression.Value));
			}

            return expression;
        }

        protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
        {
            var generator = _methodGeneratorRegistry.GetMethodGenerator(expression.Method);

            generator.BuildHql(expression.Object, expression.Arguments, this);

            return expression;
        }

        protected override Expression VisitLambdaExpression(LambdaExpression expression)
        {
            VisitExpression(expression.Body);

            return expression;
        }

        protected override Expression VisitParameterExpression(ParameterExpression expression)
        {
            _stack.PushLeaf(_hqlTreeBuilder.Ident(expression.Name));

            return expression;
        }

        protected override Expression VisitConditionalExpression(ConditionalExpression expression)
        {
            using (_stack.PushNode(_hqlTreeBuilder.Case()))
            {
                using (_stack.PushNode(_hqlTreeBuilder.When()))
                {
                    VisitExpression(expression.Test);

                    VisitExpression(expression.IfTrue);
                }

                if (expression.IfFalse != null)
                {
                    using (_stack.PushNode(_hqlTreeBuilder.Else()))
                    {
                        VisitExpression(expression.IfFalse);
                    }
                }
            }

            return expression;
        }

        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            CommandData query = QueryModelVisitor.GenerateHqlQuery(expression.QueryModel, _parameters, _requiredHqlParameters);

            _stack.PushLeaf(query.Statement);
            
            return expression;
        }


        // Called when a LINQ expression type is not handled above.
        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            string itemText = FormatUnhandledItem(unhandledItem);
            var message = string.Format("The expression '{0}' (type: {1}) is not supported by this LINQ provider.", itemText, typeof(T));
            return new NotSupportedException(message);
        }

        private string FormatUnhandledItem<T>(T unhandledItem)
        {
            var itemAsExpression = unhandledItem as Expression;
            return itemAsExpression != null ? FormattingExpressionTreeVisitor.Format(itemAsExpression) : unhandledItem.ToString();
        }
    }

    public class MethodGeneratorRegistry
    {
        public static MethodGeneratorRegistry Initialise()
        {
            var registry = new MethodGeneratorRegistry();

            // TODO - could use reflection here
            registry.Register(new QueryableMethodsGenerator());
            registry.Register(new StringMethodsGenerator());

            return registry;
        }

        private readonly Dictionary<MethodInfo, IHqlGeneratorForMethod> _registeredMethods = new Dictionary<MethodInfo, IHqlGeneratorForMethod>();

        public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
        {
            IHqlGeneratorForMethod methodGenerator;

            if (method.IsGenericMethod)
            {
                method = method.GetGenericMethodDefinition();
            }

            if (_registeredMethods.TryGetValue(method, out methodGenerator))
            {
                return methodGenerator;
            }

            throw new NotSupportedException();
        }

        public void RegisterMethodGenerator(MethodInfo method, IHqlGeneratorForMethod generator)
        {
            _registeredMethods.Add(method, generator);
        }

        private void Register(IHqlGeneratorForType typeMethodGenerator)
        {
            typeMethodGenerator.RegisterMethods(this);
        }

    }

    public interface IHqlGeneratorForMethod
    {
        IEnumerable<MethodInfo> SupportedMethods { get; }
        void BuildHql(Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlGeneratorExpressionTreeVisitor hqlVisitor);
    }

    public interface IHqlGeneratorForType
    {
        void RegisterMethods(MethodGeneratorRegistry methodGeneratorRegistry);
    }

    abstract public class BaseHqlGeneratorForType : IHqlGeneratorForType
    {
        protected readonly List<IHqlGeneratorForMethod> MethodRegistry = new List<IHqlGeneratorForMethod>();

        public void RegisterMethods(MethodGeneratorRegistry methodGeneratorRegistry)
        {
            foreach (var generator in MethodRegistry)
            {
                foreach (var method in generator.SupportedMethods)
                {
                    methodGeneratorRegistry.RegisterMethodGenerator(method, generator);
                }
            }
        }
    }

    public abstract class BaseHqlGeneratorForMethod : IHqlGeneratorForMethod
    {
        public IEnumerable<MethodInfo> SupportedMethods { get; protected set; }

        public abstract void BuildHql(Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlGeneratorExpressionTreeVisitor hqlVisitor);
    }

    public class StringMethodsGenerator : BaseHqlGeneratorForType
    {
        public StringMethodsGenerator()
        {
            // TODO - could use reflection
            MethodRegistry.Add(new StartsWithGenerator());
            MethodRegistry.Add(new EndsWithGenerator());
            MethodRegistry.Add(new ContainsGenerator());
            MethodRegistry.Add(new EqualsGenerator());
        }

        class StartsWithGenerator : BaseHqlGeneratorForMethod
        {
            public StartsWithGenerator()
            {
                SupportedMethods = new[] { ReflectionHelper.GetMethod<string>(x => x.StartsWith(null)) };
            }

            public override void BuildHql(Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlGeneratorExpressionTreeVisitor hqlVisitor)
            {
                using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Like()))
                {
                    hqlVisitor.Visit(targetObject);

                    // TODO - this sucks.  Concat() just pushes a method node, and we have to do all the child stuff.  
                    // Sort out the tree stuff so it works properly
                    using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Concat()))
                    {
                        hqlVisitor.Stack.PushLeaf(hqlVisitor.TreeBuilder.Ident("concat"));

                        using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.ExpressionList()))
                        {
                            hqlVisitor.Visit(arguments[0]);

                            hqlVisitor.Stack.PushLeaf(hqlVisitor.TreeBuilder.Constant("%"));
                        }
                    }
                }
            }
        }

        class EndsWithGenerator : BaseHqlGeneratorForMethod
        {
            public EndsWithGenerator()
            {
                SupportedMethods = new[] { ReflectionHelper.GetMethod<string>(x => x.EndsWith(null)) };
            }

            public override void BuildHql(Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlGeneratorExpressionTreeVisitor hqlVisitor)
            {
                using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Like()))
                {
                    hqlVisitor.Visit(targetObject);

                    // TODO - this sucks.  Concat() just pushes a method node, and we have to do all the child stuff.  
                    // Sort out the tree stuff so it works properly
                    using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Concat()))
                    {
                        hqlVisitor.Stack.PushLeaf(hqlVisitor.TreeBuilder.Ident("concat"));

                        using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.ExpressionList()))
                        {
                            hqlVisitor.Stack.PushLeaf(hqlVisitor.TreeBuilder.Constant("%"));

                            hqlVisitor.Visit(arguments[0]);
                        }
                    }
                }
            }
        }

        class ContainsGenerator : BaseHqlGeneratorForMethod
        {
            public ContainsGenerator()
            {
                SupportedMethods = new[] { ReflectionHelper.GetMethod<string>(x => x.Contains(null)) };
            }

            public override void BuildHql(Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlGeneratorExpressionTreeVisitor hqlVisitor)
            {
                using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Like()))
                {
                    hqlVisitor.Visit(targetObject);

                    // TODO - this sucks.  Concat() just pushes a method node, and we have to do all the child stuff.  
                    // Sort out the tree stuff so it works properly
                    using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Concat()))
                    {
                        hqlVisitor.Stack.PushLeaf(hqlVisitor.TreeBuilder.Ident("concat"));

                        using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.ExpressionList()))
                        {
                            hqlVisitor.Stack.PushLeaf(hqlVisitor.TreeBuilder.Constant("%"));

                            hqlVisitor.Visit(arguments[0]);

                            hqlVisitor.Stack.PushLeaf(hqlVisitor.TreeBuilder.Constant("%"));
                        }
                    }
                }
            }
        }

        class EqualsGenerator : BaseHqlGeneratorForMethod
        {
            public EqualsGenerator()
            {
                SupportedMethods = new[] { ReflectionHelper.GetMethod<string>(x => x.Equals((string)null)) };
            }

            public override void BuildHql(Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlGeneratorExpressionTreeVisitor hqlVisitor)
            {
                using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Equality()))
                {
                    hqlVisitor.Visit(targetObject);

                    hqlVisitor.Visit(arguments[0]);
                }
            }
        }
    }

    public class QueryableMethodsGenerator : BaseHqlGeneratorForType
    {
        public QueryableMethodsGenerator()
        {
            // TODO - could use reflection
            MethodRegistry.Add(new AnyGenerator());
            MethodRegistry.Add(new AllGenerator());
            MethodRegistry.Add(new MinGenerator());
            MethodRegistry.Add(new MaxGenerator());
        }

        class AnyGenerator : BaseHqlGeneratorForMethod
        {
            public AnyGenerator()
            {
                SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethod(() => Queryable.Any<object>(null)),
                                           ReflectionHelper.GetMethod(() => Queryable.Any<object>(null, null)),
                                           ReflectionHelper.GetMethod(() => Enumerable.Any<object>(null)),
                                           ReflectionHelper.GetMethod(() => Enumerable.Any<object>(null, null))
                                       };
            }

            public override void BuildHql(Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlGeneratorExpressionTreeVisitor hqlVisitor)
            {
                // Any has one or two arguments.  Arg 1 is the source and arg 2 is the optional predicate
                using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Exists()))
                {
                    using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Query()))
                    {
                        using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.SelectFrom()))
                        {
                            using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.From()))
                            {
                                using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Range()))
                                {
                                    hqlVisitor.Visit(arguments[0]);

                                    if (arguments.Count > 1)
                                    {
                                        var expr = (LambdaExpression)arguments[1];
                                        hqlVisitor.Stack.PushLeaf(hqlVisitor.TreeBuilder.Alias(expr.Parameters[0].Name));
                                    }
                                }
                            }
                        }
                        if (arguments.Count > 1)
                        {
                            using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Where()))
                            {
                                hqlVisitor.Visit(arguments[1]);
                            }
                        }
                    }
                }
            }
        }

        class AllGenerator : BaseHqlGeneratorForMethod
        {
            public AllGenerator()
            {
                SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethod(() => Queryable.All<object>(null, null)),
                                           ReflectionHelper.GetMethod(() => Enumerable.All<object>(null, null))
                                       };
            }

            public override void BuildHql(Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlGeneratorExpressionTreeVisitor hqlVisitor)
            {
                // All has two arguments.  Arg 1 is the source and arg 2 is the predicate
                using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Not()))
                {
                    using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Exists()))
                    {
                        using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Query()))
                        {
                            using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.SelectFrom()))
                            {
                                using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.From()))
                                {
                                    using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Range()))
                                    {
                                        hqlVisitor.Visit(arguments[0]);

                                        var expr = (LambdaExpression)arguments[1];

                                        hqlVisitor.Stack.PushLeaf(hqlVisitor.TreeBuilder.Alias(expr.Parameters[0].Name));
                                    }
                                }
                            }

                            using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Where()))
                            {
                                using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Not()))
                                {
                                    hqlVisitor.Visit(arguments[1]);
                                }
                            }
                        }
                    }
                }
            }
        }

        class MinGenerator : BaseHqlGeneratorForMethod
        {
            public MinGenerator()
            {
                SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethod(() => Queryable.Min<object>(null)),
                                           ReflectionHelper.GetMethod(() => Enumerable.Min<object>(null))
                                       };
            }

            public override void BuildHql(Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlGeneratorExpressionTreeVisitor hqlVisitor)
            {
                using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Min()))
                {
                    hqlVisitor.Visit(arguments[1]);
                }
            }
        }

        class MaxGenerator : BaseHqlGeneratorForMethod
        {
            public MaxGenerator()
            {
                SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethod(() => Queryable.Max<object>(null)),
                                           ReflectionHelper.GetMethod(() => Enumerable.Max<object>(null))
                                       };
            }

            public override void BuildHql(Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlGeneratorExpressionTreeVisitor hqlVisitor)
            {
                using (hqlVisitor.Stack.PushNode(hqlVisitor.TreeBuilder.Max()))
                {
                    hqlVisitor.Visit(arguments[1]);
                }
            }
        }
    }
}