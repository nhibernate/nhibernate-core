using System;
using System.Collections;
using System.Data;
using System.Data.Common;
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

		protected override string[] Mappings
		{
			get { return Array.Empty<string>(); }
		}

		#endregion
		[Test]
		public void BadGrammar()
		{
			ISession session = OpenSession();
			var connection = session.Connection;
			try
			{
				var ps = connection.CreateCommand();
				ps.CommandType = CommandType.Text;
				ps.CommandText = "whatever";
				ps.ExecuteNonQuery();
			}
			catch (Exception sqle)
			{
				Assert.DoesNotThrow(
					() => ADOExceptionHelper.Convert(Sfi.SQLExceptionConverter, sqle, "could not get or update next value", null));
			}
			finally
			{
				session.Close();
			}
		}
	}
}
