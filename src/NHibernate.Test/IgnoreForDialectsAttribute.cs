using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class,
		AllowMultiple = true)]
	public class IgnoreForDialectsAttribute : Attribute, ITestAction
	{
		private readonly string[] _dialectTypePartialNames;
		private readonly System.Type[] _dialectTypes;

		public IgnoreForDialectsAttribute(params System.Type[] dialectTypes)
		{
			_dialectTypes = dialectTypes;
		}

		public IgnoreForDialectsAttribute(params string[] dialectTypePartialNames)
		{
			_dialectTypePartialNames = dialectTypePartialNames;
		}

		public string Message { get; set; }

		public void BeforeTest(TestDetails details)
		{
			var fixture = details.Fixture as TestCase;
			if (fixture != null)
			{
				var fixtureDialect = fixture.Dialect;
				if (fixtureDialect == null)
				{
					return;
				}
				if (_dialectTypes != null && _dialectTypes.Any(x => x.IsInstanceOfType(fixtureDialect)) ||
					_dialectTypePartialNames != null && _dialectTypePartialNames.Any(x=>fixtureDialect.GetType().Name.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0))
				{
					Assert.Ignore(string.Format("Ignored for dialect {0}. {1}", fixture.Dialect.GetType().Name, Message));
				}
			}
		}

		public void AfterTest(TestDetails details)
		{

		}

		public ActionTargets Targets
		{
			get { return ActionTargets.Test; }
		}


	}
}
