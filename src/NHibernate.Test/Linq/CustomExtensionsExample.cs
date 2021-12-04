using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using NHibernate.Cfg;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	public static class MyLinqExtensions
	{
		public static bool IsLike(this string source, string pattern)
		{
			pattern = Regex.Escape(pattern);
			pattern = pattern.Replace("%", ".*?").Replace("_", ".");
			pattern = pattern.Replace(@"\[", "[").Replace(@"\]","]").Replace(@"\^", "^");

			return Regex.IsMatch(source, pattern);
		}
	}

	public class MyLinqToHqlGeneratorsRegistry: DefaultLinqToHqlGeneratorsRegistry
	{
		public MyLinqToHqlGeneratorsRegistry():base()
		{
			RegisterGenerator(ReflectHelper.GetMethodDefinition(() => MyLinqExtensions.IsLike(null, null)),
							  new IsLikeGenerator());
			RegisterGenerator(ReflectHelper.GetMethodDefinition(() => new object().Equals(null)), new ObjectEqualsGenerator());
		}
	}

	public class IsLikeGenerator : BaseHqlGeneratorForMethod
	{
		public IsLikeGenerator()
		{
			SupportedMethods = new[] {ReflectHelper.GetMethodDefinition(() => MyLinqExtensions.IsLike(null, null))};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, 
			ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Like(visitor.Visit(arguments[0]).AsExpression(),
									visitor.Visit(arguments[1]).AsExpression());
		}
	}

	public class ObjectEqualsGenerator : BaseHqlGeneratorForMethod
	{
		public ObjectEqualsGenerator()
		{
			SupportedMethods = new[] { ReflectHelper.GetMethodDefinition(() => new object().Equals(null)) };
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject,
		                                     ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Equality(visitor.Visit(targetObject).AsExpression(),
			                        visitor.Visit(arguments[0]).AsExpression());
		}
	}

	[TestFixture]
	public class CustomExtensionsExample : LinqTestCase
	{
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			configuration.LinqToHqlGeneratorsRegistry<MyLinqToHqlGeneratorsRegistry>();
		}

		[Test]
		public void CanUseObjectEquals()
		{
			var users = db.Users.Where(o => ((object) EnumStoredAsString.Medium).Equals(o.NullableEnum1)).ToList();
			Assert.That(users.Count, Is.EqualTo(2));
			Assert.That(users.All(c => c.NullableEnum1 == EnumStoredAsString.Medium), Is.True);
		}

		[Test]
		public void CanUseMyCustomExtension()
		{
			var contacts = (from c in db.Customers where c.ContactName.IsLike("%Thomas%") select c).ToList();
			Assert.That(contacts.Count, Is.GreaterThan(0));
			Assert.That(contacts.All(c => c.ContactName.Contains("Thomas")), Is.True);
		}
	}
}
