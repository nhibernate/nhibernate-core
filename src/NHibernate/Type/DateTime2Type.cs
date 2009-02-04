using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.DateTime" /> Property to a <see cref="DbType.DateTime"/>
	/// </summary>
	[Serializable]
	public class DateTime2Type : DateTimeType
	{
		/// <summary></summary>
		internal DateTime2Type() : base(SqlTypeFactory.DateTime2)
		{
		}

		public override string Name
		{
			get { return "DateTime2"; }
		}
	}
}