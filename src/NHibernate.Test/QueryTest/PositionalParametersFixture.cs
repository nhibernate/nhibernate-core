using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.QueryTest
{
	/// <summary>
	/// Summary description for PositionalParametersFixture.
	/// </summary>
	[TestFixture]
	public class PositionalParametersFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"Simple.hbm.xml"}; }
		}

		[Test, ExpectedException(typeof(QueryException))]
		public void TestMissingHQLParameters()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			try
			{
				IQuery q = s.CreateQuery("from s in class Simple where s.Name=? and s.Count=?");
				// Set the first property, but not the second
				q.SetParameter(0, "Fred");

				// Try to execute it
				IList list = q.List();
			}
			finally
			{
				t.Rollback();
				s.Close();
			}
		}

		[Test, ExpectedException(typeof(QueryException))]
		public void TestMissingHQLParameters2()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			try
			{
				IQuery q = s.CreateQuery("from s in class Simple where s.Name=? and s.Count=?");
				// Set the second property, but not the first - should give a nice not found at position xxx error
				q.SetParameter(1, "Fred");

				// Try to execute it
				IList list = q.List();
			}
			finally
			{
				t.Rollback();
				s.Close();
			}
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void TestPositionOutOfBounds()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			try
			{
				IQuery q = s.CreateQuery("from s in class Simple where s.Name=? and s.Count=?");
				// Try to set the third positional parameter
				q.SetParameter(3, "Fred");

				// Try to execute it
				IList list = q.List();
			}
			finally
			{
				t.Rollback();
				s.Close();
			}
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void TestNoPositionalParameters()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			try
			{
				IQuery q = s.CreateQuery("from s in class Simple where s.Name=:Name and s.Count=:Count");
				// Try to set the first property
				q.SetParameter(0, "Fred");

				// Try to execute it
				IList list = q.List();
			}
			finally
			{
				t.Rollback();
				s.Close();
			}
		}

		/// <summary>
		/// Verifying that a <see langword="null" /> value passed into SetParameter(index, val) throws
		/// an exception
		/// </summary>
		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void TestNullIndexedParameter()
		{
			ISession s = OpenSession();

			try
			{
				IQuery q = s.CreateQuery("from Simple as s where s.Name=?");
				q.SetParameter(0, null);
			}
			finally
			{
				s.Close();
			}
		}
	}
}