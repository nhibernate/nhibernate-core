using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.UInt16"/> Property 
	/// to a <see cref="DbType.UInt16"/> column.
	/// </summary>
	[Serializable]
	public class UInt16Type : ValueTypeType, IDiscriminatorType, IVersionType
	{
		/// <summary></summary>
		internal UInt16Type() : base(SqlTypeFactory.UInt16)
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
			return Convert.ToUInt16(rs[index]);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, string name)
		{
			return Convert.ToUInt16(rs[name]);
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof(UInt16); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set(IDbCommand rs, object value, int index)
		{
			IDataParameter parm = rs.Parameters[index] as IDataParameter;
			parm.Value = value;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "UInt16"; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ObjectToSQLString(object value)
		{
			return value.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public object StringToObject(string xml)
		{
			return FromStringValue(xml);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override object FromStringValue(string xml)
		{
			return ushort.Parse(xml);
		}

		#region IVersionType Members

		public virtual object Next(object current, ISessionImplementor session)
		{
			return (ushort) ((ushort) current + 1);
		}

		public virtual object Seed(ISessionImplementor session)
		{
			return (ushort) 1;
		}

		public IComparer Comparator
		{
			get { return Comparer.DefaultInvariant; }
		}

		#endregion
	}
}