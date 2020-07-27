using System;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Test.NHSpecificTest.GH2437
{
	/// <summary>
	/// Short Number Date support with format yyyyMMdd
	/// </summary>
	[Serializable]
	public class SqlNumberDate : SqlNumberDateTime
	{
		protected override string Format { get; } = "yyyyMMdd";
		public static readonly DateTime DefaultDate = new DateTime(1900, 1, 1);

		/// <summary>
		/// Write an instance of the mapped class to a prepared statement.
		///             Implementors should handle possibility of null values.
		///             A multi-column type should be written to parameters starting from index.
		/// </summary>
		/// <param name="cmd">a IDbCommand</param><param name="value">the object to write</param><param name="index">command parameter index</param><exception cref="T:NHibernate.HibernateException">HibernateException</exception>
		public override void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			Int32? result;
			if (value != null)
			{
				var dateTime = (DateTime?) value;
				if (dateTime < DefaultDateTime)
				{
					result = default(Int32?);
				}
				else
				{
					result = Int32.Parse(dateTime?.ToString(Format));
				}
			}
			else
			{
				result = default(Int32?);
			}

			NHibernateUtil.Int32.NullSafeSet(cmd, result, index, session);
		}

		/// <summary>
		/// The SQL types for the columns mapped by this type. 
		/// </summary>
		public override SqlType[] SqlTypes
		{
			get
			{
				return new[] { new SqlType(DbType.Int32) };
			}
		}
	}
}
