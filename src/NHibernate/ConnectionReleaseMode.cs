using System;

namespace NHibernate
{
	public enum ConnectionReleaseMode
	{
		AfterStatement,
		AfterTransaction,
		OnClose
	}

	public static class ConnectionReleaseModeParser
	{
		public static ConnectionReleaseMode Convert(string value)
		{
			switch (value)
			{
				case "after_statement":
					throw new HibernateException("aggressive connection release (after_statement) not supported by NHibernate");
				case "after_transaction":
					return ConnectionReleaseMode.AfterTransaction;
				case "on_close":
					return ConnectionReleaseMode.OnClose;
				default:
					throw new HibernateException("could not determine appropriate connection release mode [" + value + "]");
			}
		}

		public static string ToString(ConnectionReleaseMode value)
		{
			switch (value)
			{
				case ConnectionReleaseMode.AfterStatement:
					return "after_statement";
				case ConnectionReleaseMode.AfterTransaction:
					return "after_transaction" ;
				case ConnectionReleaseMode.OnClose:
					return "on_close";
				default:
					throw new ArgumentOutOfRangeException("value");
			}
		}
	}
}