using System;

using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Exceptions
{
	public sealed class ADOExceptionHelper
	{
		private ADOExceptionHelper()
		{
		}

		/// <summary>
		/// Converts the given SQLException into NHibernate's ADOException hierarchy, as well as performing
		/// appropriate logging.
		/// </summary>
		/// <!--<param name="converter">The converter to use.</param>-->
		/// <param name="sqlException">The exception to convert.</param>
		/// <param name="message">An optional error message.</param>
		/// <returns>The converted ADOException.</returns>
		public static ADOException Convert( /*ISQLExceptionConverter converter,*/ System.Exception sqlException, string message )
		{
			ADOExceptionReporter.LogExceptions( sqlException, message );
			// return converter.Convert( sqlException, message );
			return new ADOException( message, sqlException );
		}

		public static ADOException Convert( System.Exception sqlException, string message, SqlString sql )
		{
			ADOExceptionReporter.LogExceptions( sqlException, message + " [" + sql + "]" );
			return new ADOException( message, sqlException, sql );

		}
	}
}