using System;
using System.Collections;

using NUnit.Framework;

namespace NHibernate.UserTypes.Tests
{
	[TestFixture]
	public class EmptyStringFixture : EmptyAnsiStringFixture
	{
		protected override IList Mappings
		{
			get { return new string[] {"EmptyStringClass.hbm.xml"}; }
		}

		// no need to write different test than in the ansi string version
		// of the fixture
	}
}