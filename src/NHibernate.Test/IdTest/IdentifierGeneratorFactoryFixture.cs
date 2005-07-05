using System;

using NHibernate.Id;

using NUnit.Framework;

namespace NHibernate.Test.IdTest
{
	[TestFixture]
	public class IdentifierGeneratorFactoryFixture
	{
		/// <summary>
		/// Testing NH-325 to make sure that an exception is actually
		/// caught with a bad class name passed in.
		/// </summary>
		[Test]
		[ExpectedException( typeof(MappingException), "could not instantiate id generator for strategy 'Guid'" )]
		public void NonCreatableStrategy()
		{
			IIdentifierGenerator idGenerator = null;
			idGenerator = IdentifierGeneratorFactory.Create( "Guid", NHibernateUtil.Guid, null, new Dialect.MsSql2000Dialect() );
		}
	}
}
