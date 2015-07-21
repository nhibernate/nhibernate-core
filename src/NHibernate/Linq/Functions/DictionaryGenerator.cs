using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	public class DictionaryItemGenerator : BaseHqlGeneratorForMethod
	{
		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			string memberName;
			if (VisitorUtil.IsDynamicComponentDictionaryGetter(method, targetObject, arguments, visitor.SessionFactory, out memberName))
			{
				return treeBuilder.Dot(visitor.Visit(targetObject).AsExpression(), treeBuilder.Ident(memberName));
			}
			return treeBuilder.DictionaryItem(visitor.Visit(targetObject).AsExpression(), visitor.Visit(arguments[0]).AsExpression());
		}
	}

	public class DictionaryContainsKeyGenerator : BaseHqlGeneratorForMethod
	{
		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.In(visitor.Visit(arguments[0]).AsExpression(), treeBuilder.Indices(visitor.Visit(targetObject).AsExpression()));
		}
	}

	public abstract class GenericDictionaryRuntimeMethodHqlGeneratorBase<TGenerator> : IRuntimeMethodHqlGenerator
		where TGenerator : IHqlGeneratorForMethod, new()
	{
		private readonly IHqlGeneratorForMethod generator = new TGenerator();

		protected abstract string MethodName { get; }

		public bool SupportsMethod(MethodInfo method)
		{
			return (method != null) && (method.Name == MethodName) && method.IsMethodOf(typeof(IDictionary<,>));
		}

		public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
		{
			return generator;
		}
	}
	
	public abstract class DictionaryRuntimeMethodHqlGeneratorBase<TGenerator> : IRuntimeMethodHqlGenerator
		where TGenerator : IHqlGeneratorForMethod, new()
	{
		private readonly IHqlGeneratorForMethod generator = new TGenerator();

		protected abstract string MethodName { get; }

		public bool SupportsMethod(MethodInfo method)
		{
			return (method != null) && (method.Name == MethodName) && method.IsMethodOf(typeof(IDictionary));
		}

		public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
		{
			return generator;
		}
	}

	public class GenericDictionaryContainsKeyRuntimeHqlGenerator : GenericDictionaryRuntimeMethodHqlGeneratorBase<DictionaryContainsKeyGenerator>
	{
		protected override string MethodName
		{
			get { return "ContainsKey"; }
		}
	}
	
	public class DictionaryContainsKeyRuntimeHqlGenerator : DictionaryRuntimeMethodHqlGeneratorBase<DictionaryContainsKeyGenerator>
	{
		protected override string MethodName
		{
			get { return "Contains"; }
		}
	}

	public class DictionaryItemRuntimeHqlGenerator : DictionaryRuntimeMethodHqlGeneratorBase<DictionaryItemGenerator>
	{
		protected override string MethodName
		{
			get { return "get_Item"; }
		}
	}
	
	public class GenericDictionaryItemRuntimeHqlGenerator : GenericDictionaryRuntimeMethodHqlGeneratorBase<DictionaryItemGenerator>
	{
		protected override string MethodName
		{
			get { return "get_Item"; }
		}
	}
}