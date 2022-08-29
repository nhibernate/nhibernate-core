using System;
using NHibernate.Test.Immutable.EntityWithMutableCollection;
using NUnit.Framework;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection.NonInverse
{
	[TestFixture]
	public class EntityWithNonInverseOneToManyTest : AbstractEntityWithOneToManyTest
	{
		protected override string[] Mappings
		{
			get
			{
				return new string[] { "Immutable.EntityWithMutableCollection.NonInverse.ContractVariation.hbm.xml" };
			}
		}
	}
}
