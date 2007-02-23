using System;
using System.Collections;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Base class for fixtures testing individual types, created to avoid
	/// code duplication in derived classes. It assumes that the fixture
	/// uses mapping file named "(TypeName)Class.hbm.xml" placed in TypesTest
	/// directory in NHibernate.Test assembly.
	/// </summary>
	public abstract class TypeFixtureBase : TestCase
	{
		protected abstract string TypeName { get; }

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						String.Format("TypesTest.{0}Class.hbm.xml", TypeName)
					};
			}
		}
	}
}