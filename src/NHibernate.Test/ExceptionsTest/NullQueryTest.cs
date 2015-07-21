using System;
using System.Collections;
using System.Data;
using NHibernate.Exceptions;
using NUnit.Framework;

namespace NHibernate.Test.ExceptionsTest
{
	/// <summary>
	/// NH-1997
	/// </summary>
	[TestFixture]
	public class NullQueryTest : TestCase
	{
		#region Overrides of TestCase

		protected override IList Mappings
		{
			get { return new string[0]; }
		}

		#endregion
		[Test]
		public void BadGrammar()
		{
			ISession session = OpenSession();
			IDbConnection connection = session.Connection;
			try
			{
				IDbCommand ps = connection.CreateCommand();
				ps.CommandType = CommandType.Text;
				ps.CommandText = "whatever";
				ps.ExecuteNonQuery();
			}
			catch (Exception sqle)
			{
				Assert.DoesNotThrow(
					() => ADOExceptionHelper.Convert(sessions.SQLExceptionConverter, sqle, "could not get or update next value", null));
			}
			finally
			{
				session.Close();
			}
		}
	}
}
