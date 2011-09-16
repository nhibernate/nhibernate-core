using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NUnit.Framework;
using SharpTestsEx;

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

        public static int IsOneInDbZeroInLocal(this string source)
        {
            

            return 0;
        }
	}

	public class MyLinqToHqlGeneratorsRegistry: DefaultLinqToHqlGeneratorsRegistry
	{
		public MyLinqToHqlGeneratorsRegistry():base()
		{
			RegisterGenerator(ReflectionHelper.GetMethodDefinition(() => MyLinqExtensions.IsLike(null, null)),
			                  new IsLikeGenerator());

            RegisterGenerator(ReflectionHelper.GetMethodDefinition(() => MyLinqExtensions.IsOneInDbZeroInLocal(null)),
                              new IsTrueInDbFalseInLocalGenerator());
		}
	}

	public class IsLikeGenerator : BaseHqlGeneratorForMethod
	{
		public IsLikeGenerator()
		{
			SupportedMethods = new[] {ReflectionHelper.GetMethodDefinition(() => MyLinqExtensions.IsLike(null, null))};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, 
			ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Like(visitor.Visit(arguments[0]).AsExpression(),
			                        visitor.Visit(arguments[1]).AsExpression());
		}
	}

    public class IsTrueInDbFalseInLocalGenerator : BaseHqlGeneratorForMethod
    {
        public IsTrueInDbFalseInLocalGenerator()
        {
            SupportedMethods = new[] { ReflectionHelper.GetMethodDefinition(() => MyLinqExtensions.IsOneInDbZeroInLocal(null)) };
        }
        
        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject,
            ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            return treeBuilder.Constant(1);
        }
    }

	public class CustomExtensionsExample : LinqTestCase
	{
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			configuration.LinqToHqlGeneratorsRegistry<MyLinqToHqlGeneratorsRegistry>();
		}

		[Test]
		public void CanUseMyCustomExtension()
		{
			var contacts = (from c in db.Customers where c.ContactName.IsLike("%Thomas%") select c).ToList();
			contacts.Count.Should().Be.GreaterThan(0);
			contacts.Select(customer => customer.ContactName).All(c => c.Satisfy(customer => customer.Contains("Thomas")));
		}

        [Test]
        public void CanUseMyCustomExtensionInProjection()
        {
            var boolProjection= (from c in db.Customers where c.ContactName.IsOneInDbZeroInLocal()==1 select c.ContactName.IsOneInDbZeroInLocal()).FirstOrDefault();
            boolProjection.Should().Be(1);
        
        }
	}
}