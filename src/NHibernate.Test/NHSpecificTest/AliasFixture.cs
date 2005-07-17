using System;
using System.Collections;

using NUnit.Framework;
using NHibernate.SqlCommand;

namespace NHibernate.Test.NHSpecificTest
{
	[TestFixture]
	public class AliasFixture
	{
		[Test]
		public void NoLeadingUnderscores()
		{
			Alias alias = new Alias( "suffix" );
			Dialect.Dialect dialect = new Dialect.MsSql2000Dialect();
			
			Assert.IsFalse( 
				alias.ToAliasString( "__someIdentifier", dialect )
				.StartsWith( "_" ) );

			Assert.IsFalse( 
				alias.ToUnquotedAliasString( "__someIdentifier", dialect )
				.StartsWith( "_" ) );
		}
	}
}
