using System;

namespace NHibernate
{
	public enum ConnectionReleaseMode
	{
		AfterStatement,
		AfterTransaction,
		OnClose
	}
}
