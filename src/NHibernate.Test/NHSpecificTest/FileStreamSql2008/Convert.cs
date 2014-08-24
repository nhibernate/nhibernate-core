using System.Text;

namespace NHibernate.Test.NHSpecificTest.FileStreamSql2008
{
	/// <summary>
	/// Byte[]-to-String and String-to-Byte[] converter
	/// </summary>
	public class Convert
	{
		public static byte[] ToBytes(string str)
		{
			return new ASCIIEncoding().GetBytes(str);
		}

		public static string ToStr(byte[] bytes)
		{
			return new ASCIIEncoding().GetString(bytes);
		}
	}
}