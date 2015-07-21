using System;
using NHibernate.Exceptions;

namespace NHibernate.Test.NHSpecificTest.SqlConverterAndMultiQuery
{
	public class SqlConverter : ISQLExceptionConverter
	{
		public Exception Convert(AdoExceptionContextInfo adoExceptionContextInfo)
		{
			return new UnitTestException();
		}
	}

	public class UnitTestException : Exception{}
}
