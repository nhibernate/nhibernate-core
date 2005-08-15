using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest
{
	/// <summary>
	/// Base class that can be used for tests in NH* subdirectories.
	/// Assumes all mappings are in a single file named <c>Mappings.hbm.xml</c>
	/// in the subdirectory.
	/// </summary>
	public abstract class BugTestCase : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		public abstract string BugNumber { get; }

		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"NHSpecificTest." + BugNumber + ".Mappings.hbm.xml"
					};
			}
		}
	}
}
