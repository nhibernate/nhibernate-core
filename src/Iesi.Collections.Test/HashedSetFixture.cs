using System;
using System.Collections;

using Iesi.Collections;

using NUnit.Framework;

namespace Iesi.Collections.Test
{
	/// <summary>
	/// Summary description for HashedSetFixture.
	/// </summary>
	[TestFixture]
	public class HashedSetFixture : SetFixture
	{
		
		protected override ISet CreateInstance()
		{
			return new HashedSet();
		}
		
		protected override ISet CreateInstance(ICollection init)
		{
			return new HashedSet( init );
		}

		protected override Type ExpectedType
		{
			get { return typeof(HashedSet); }
		}

		[Test]
		public void Serialization() 
		{
			// serialize and then deserialize the ISet.
			System.IO.Stream stream = new System.IO.MemoryStream();
			System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			formatter.Serialize( stream, _set );
			stream.Position = 0;
			
			ISet desSet = (ISet)formatter.Deserialize( stream );
			stream.Close();

			Assert.AreEqual( 3, desSet.Count, "should have des 3 items" );
			Assert.IsTrue( desSet.Contains( one ), "should contain one" );
			Assert.IsTrue( desSet.Contains( two ), "should contain two" );
			Assert.IsTrue( desSet.Contains( three ), "should contain three" );
		}

		
	}
}
