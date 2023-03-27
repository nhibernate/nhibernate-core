using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
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
			pattern = pattern.Replace(@"\[", "[").Replace(@"\]", "]").Replace(@"\^", "^");

			return Regex.IsMatch(source, pattern);
		}

		public static TimeSpan GetTime(this DateTime dateTime)
		{
			return dateTime.TimeOfDay;
		}
	}

	public class MyLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
	{
		public MyLinqToHqlGeneratorsRegistry()
		{
			RegisterGenerator(ReflectHelper.GetMethodDefinition(() => MyLinqExtensions.IsLike(null, null)),
							  new IsLikeGenerator());
			RegisterGenerator(ReflectHelper.GetMethodDefinition(() => MyLinqExtensions.GetTime(default(DateTime))), new GetTimeGenerator());
		}
	}

	public class GetTimeGenerator : BaseHqlGeneratorForMethod
	{
		public GetTimeGenerator()
		{
			SupportedMethods = new[] { ReflectHelper.GetMethodDefinition(() => MyLinqExtensions.GetTime(default(DateTime))) };
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.MethodCall("cast", visitor.Visit(arguments[0]).AsExpression(), treeBuilder.Ident(NHibernateUtil.TimeAsTimeSpan.Name));
		}
	}

	public class IsLikeGenerator : BaseHqlGeneratorForMethod
	{
		public IsLikeGenerator()
		{
			SupportedMethods = new[] { ReflectHelper.GetMethodDefinition(() => MyLinqExtensions.IsLike(null, null)) };
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject,
			ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Like(visitor.Visit(arguments[0]).AsExpression(),
									visitor.Visit(arguments[1]).AsExpression());
		}
	}

	[TestFixture]
	public class CustomExtensionsExample : LinqTestCase
	{
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			configuration.LinqToHqlGeneratorsRegistry<MyLinqToHqlGeneratorsRegistry>();
		}

		[Test(Description = "GH-2963")]
		public void CanUseComparisonWithExtensionOnMappedProperty()
		{
			if (!TestDialect.SupportsTime)
			{
				Assert.Ignore("Time type is not supported");
			}

			var time = DateTime.UtcNow.GetTime();
			db.Users.Where(u => u.RegisteredAt.GetTime() > time).Select(u => u.Id).ToList();
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
