namespace NHibernate.Tool.HbmXsd
{
	public class StringTools
	{
		public static string CamelCase(string name)
		{
			// TODO: handle long acronyms such as SQL -> Sql

			string[] parts = name.Split('-');

			for (int i = 0; i < parts.Length; i++)
				parts[i] = parts[i].Substring(0, 1).ToUpper() + parts[i].Substring(1);

			return string.Join("", parts);
		}
	}
}