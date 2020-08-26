using System.Data;

namespace NHibernate.Util
{
	public static class DbTypeExtensions
	{
		/// <summary>
		/// Checks whether the type is a <see cref="DbType.String"/>, <see cref="DbType.AnsiString"/>, <see cref="DbType.StringFixedLength"/> or <see cref="DbType.AnsiStringFixedLength"/>
		/// </summary>
		/// <param name="dbType"></param>
		/// <returns></returns>
		public static bool IsStringType(this DbType dbType) => dbType == DbType.String || dbType == DbType.AnsiString || dbType == DbType.StringFixedLength || dbType == DbType.AnsiStringFixedLength;
	}
}
