namespace NHibernate.Test.NHSpecificTest.GH1879
{
	internal static class TestExtensions
	{
		public static string NameByExtension(this Client client)
		{
			return client.Name;
		}
	}
}
