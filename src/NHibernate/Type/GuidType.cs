using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Guid"/> Property 
	/// to a <see cref="DbType.Guid"/> column.
	/// </summary>
	[Serializable]
	public class GuidType : PrimitiveType, IDiscriminatorType
	{
		/// <summary></summary>
		internal GuidType() : base(SqlTypeFactory.Guid)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, int index)
		{
			if (rs.GetFieldType(index) == typeof (Guid))
			{
				return rs.GetGuid(index);
			}

			if (rs.GetFieldType(index) == typeof(byte[]))
			{
				return new Guid((byte[])(rs[index]));
			} 

			return new Guid(Convert.ToString(rs[index]));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, string name)
		{
		    return Get(rs, rs.GetOrdinal(name));
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof(Guid); }
		}

		public override void Set(IDbCommand cmd, object value, int index)
		{
			var dp = (IDataParameter) cmd.Parameters[index];

			dp.Value = dp.DbType == DbType.Binary ? ((Guid)value).ToByteArray() : value;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Guid"; }
		}

		public override object FromStringValue(string xml)
		{
			return new Guid(xml);
		}

		public object StringToObject(string xml)
		{
			return string.IsNullOrEmpty(xml) ? null : FromStringValue(xml);
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof(Guid); }
		}

		public override object DefaultValue
		{
			get { return Guid.Empty; }
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return "'" + value + "'";
		}
	}
}