namespace NHibernate
{
	public enum ConnectionReleaseMode
	{
		AfterStatement,
		AfterTransaction,
		OnClose

		// Parse method moved to SettingsFactory
	}
}