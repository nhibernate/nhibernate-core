using System;
using System.Data.SqlTypes;

namespace NHibernate.UserTypes.Tests
{
	public class SqlTypesClass
	{
		private int _id;
		private int _version;

		private SqlBoolean _booleanProp;
		private SqlByte _byteProp;
		private SqlDateTime _dateTimeProp;
		private SqlDecimal _decimalProp;
		private SqlDouble _doubleProp;
		private SqlGuid _guidProp;
		private SqlInt16 _int16Prop;
		private SqlInt32 _int32Prop;
		private SqlInt64 _int64Prop;
		private SqlSingle _singleProp;
		
		public SqlTypesClass()
		{
			
		}

		public int Id 
		{
			get { return _id; }
			set { _id = value; }
		}

		public int Version 
		{
			get { return _version; }
			set { _version = value; }
		}

		public SqlBoolean BooleanProp
		{
			get { return _booleanProp; }
			set { _booleanProp = value; }
		}

		public SqlByte ByteProp
		{
			get { return _byteProp; }
			set { _byteProp = value; }
		}

		public SqlDateTime DateTimeProp
		{
			get { return _dateTimeProp; }
			set { _dateTimeProp = value; }
		}

		public SqlDecimal DecimalProp
		{
			get { return _decimalProp; }
			set { _decimalProp = value; }
		}

		public SqlDouble DoubleProp
		{
			get { return _doubleProp; }
			set { _doubleProp = value; }
		}

		public SqlGuid GuidProp
		{
			get { return _guidProp; }
			set { _guidProp = value; }
		}

		public SqlInt16 Int16Prop
		{
			get { return _int16Prop; }
			set { _int16Prop = value; }
		}

		public SqlInt32 Int32Prop
		{
			get { return _int32Prop; }
			set { _int32Prop = value; }
		}

		public SqlInt64 Int64Prop
		{
			get { return _int64Prop; }
			set { _int64Prop = value; }
		}

		public SqlSingle SingleProp
		{
			get { return _singleProp; }
			set { _singleProp = value; }
		}
	}
}
