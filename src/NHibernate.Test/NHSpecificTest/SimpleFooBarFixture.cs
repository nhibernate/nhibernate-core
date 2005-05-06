using System;
using System.Data;

using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	
	[TestFixture]
	public class SimpleFooBarFixture : TestCase 
	{
		protected override System.Collections.IList Mappings
		{
			get
			{
				return new string[] { "FooBar.hbm.xml"};
			}
		}

	}
}
