using System;
using NHibernate.Cfg;

namespace NHibernate.Test.NHSpecificTest
{
	/// <summary>
	/// Base class that can be used for tests in NH* subdirectories.
	/// Assumes all mappings are in a single file named <c>Mappings.hbm.xml</c>
	/// in the subdirectory.
	/// </summary>
	public abstract class BugTestCase : TestCase
	{
		protected override string[] Mappings => new[] {"Mappings.hbm.xml"};

		protected sealed override string MappingsAssembly =>
			throw new InvalidOperationException("BugTestCase does not support overriding mapping assembly.");

		protected sealed override void AddMappings(Configuration configuration)
		{
			var mappings = Mappings;
			if (mappings == null || mappings.Length == 0)
				return;
			
			var type = GetType();
			foreach (var file in mappings)
				configuration.AddResource(type.Namespace + "." + file, type.Assembly);
		}
	}
}