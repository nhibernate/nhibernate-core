using System;

using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for StringClobTypeFixture.
	/// </summary>
	[TestFixture]
	public class StringClobTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "StringClob"; }
		}

		[Test]
		public void ReadWrite() 
		{
			ISession s = OpenSession();
			StringClobClass b = new StringClobClass();
			b.StringClob = "foo/bar/baz";
			s.Save(b);
			s.Flush();
			s.Close();

			s = OpenSession();
			b = (StringClobClass)s.Load( typeof(StringClobClass), b.Id );
			Assert.AreEqual( "foo/bar/baz", b.StringClob );
			s.Delete( b );
			s.Flush();
			s.Close();
		}
	}
}
