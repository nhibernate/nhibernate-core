using System;
using System.Collections;

namespace NHibernate.Test.IdTest
{
	public abstract class IdFixtureBase : TestCase
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
						String.Format("IdTest.{0}Class.hbm.xml", TypeName)
					};
			}
		}
	}
}