using System;
using System.Data;

using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	
	[TestFixture]
	public class SimpleFooBarFixture : TestCase 
	{

		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "FooBar.hbm.xml"} );
		}

	}
}
