using System;

using NHibernate.Dialect;
using NHibernate.SqlCommand;

using NUnit.Framework;

namespace NHibernate.Test.SqlCommandTest 
{

	[TestFixture]
	public class TemplateFixture 
	{

		public TemplateFixture() 
		{
		}

		/// <summary>
		/// Tests that a column enclosed by <c>`</c> is enclosed by the Dialect.OpenQuote 
		/// and Dialect.CloseQuote after the Template Renders the Where String.
		/// </summary>
		[Test]
		public void ReplaceWithDialectQuote() 
		{
			Dialect.Dialect dialect = new Dialect.MsSql2000Dialect();
			string whereFragment = "column_name = 'string value' and `backtick` = 1";

			string expectedFragment = "$PlaceHolder.[column_name] = 'string value' and $PlaceHolder.[backtick] = 1";

			Assert.AreEqual( expectedFragment, Template.RenderWhereStringTemplate(whereFragment, dialect) );

		}

	}
}
