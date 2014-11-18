using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.TypeExtensionsTests
{
	public class CompatibilityWithCandidatePersistentMembers
	{
		public abstract class Geo
		{
			public string Descrition { get; set; }
			protected Geo Parent { get; set; }
			protected ICollection<Geo> Elements { get; set; }
		}

		[Test]
		public void GetFirstPropertyOfTypeShouldUseSameConceptsOfCandidatePersistentMembersProvider()
		{
			var memberProvider = new DefaultCandidatePersistentMembersProvider();
			var properties = memberProvider.GetRootEntityMembers(typeof(Geo));
			if(properties.Select(p => p.Name).Contains("Parent"))
			{
				Assert.That(typeof(Geo).GetFirstPropertyOfType(typeof(Geo)), Is.Not.Null);
			}
		}
	}
}