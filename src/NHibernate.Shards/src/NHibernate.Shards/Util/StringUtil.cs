namespace NHibernate.Shards.Util
{
	public class StringUtil
	{
		/// <summary>
		/// Helper function for making null strings safe for comparisons, etc.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string MakeSafe(string s)
		{
			return s ?? string.Empty;
		}

		/// <summary>
		/// the string provided with its first character capitalized
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string Capitalize(string s)
		{
			if (s.Length == 0)
				return s;

			char first = s[0];
			char capitalized = char.ToUpper(first);
			return (first == capitalized) ? s : capitalized + s.Substring(1);
		}
	}
}