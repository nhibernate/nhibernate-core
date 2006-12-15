using System;

namespace NHibernate
{
	public enum ConnectionReleaseMode
	{
		AfterTransaction,
		OnClose

		// Parse method moved to SettingsFactory
	}
}
