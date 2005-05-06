using System;

using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the ByteType.
	/// </summary>
	[TestFixture]
	public class ByteTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "Byte"; }
		}

		/// <summary>
		/// Verify Equals will correctly determine when the property
		/// is dirty.
		/// </summary>
		[Test]
		public void Equals() 
		{
			ByteType type = (ByteType)NHibernateUtil.Byte;
			
			Assert.IsTrue( type.Equals( (byte)5, (byte)5 ) );
			Assert.IsFalse( type.Equals( (byte)5, (byte)6 ) );
		}

		[Test]
		public void ReadWrite() 
		{
			ByteClass basic = new ByteClass();
			basic.Id = 1;
			basic.ByteValue = (byte)43;

			ISession s = OpenSession();
			s.Save(basic);
			s.Flush();
			s.Close();

			s = OpenSession();
			basic = (ByteClass)s.Load( typeof(ByteClass), 1 );

			Assert.AreEqual( (byte)43, basic.ByteValue );

			s.Delete( basic );
			s.Flush();
			s.Close();
		}
	}
}

	