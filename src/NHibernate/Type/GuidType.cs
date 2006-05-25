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
	public class GuidType : ValueTypeType, IDiscriminatorType
	{
		/// <summary></summary>
		internal GuidType() : base( new GuidSqlType() )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, int index )
		{
			return new Guid( Convert.ToString( rs[ index ] ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, string name )
		{
			return new Guid( Convert.ToString( rs[ name ] ) );
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( Guid ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set( IDbCommand cmd, object value, int index )
		{
			IDataParameter parm = cmd.Parameters[ index ] as IDataParameter;
			parm.Value = value;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Guid"; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ObjectToSQLString( object value )
		{
			return "'" + value.ToString() + "'";
		}

		public override object FromStringValue(string xml)
		{
			return new Guid( xml );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public object StringToObject( string xml )
		{
			return FromString( xml );
		}
	}
}