using System;

namespace Nullables.Tests.NHibernate
{
	/// <summary>
	/// Summary description for NullablesClass.
	/// </summary>
	public class NullablesClass
	{
		private int _id;
		private int _version;

		private NullableBoolean _booleanProp;
		private NullableByte _byteProp;
		private NullableDateTime _dateTimeProp;
		private NullableDecimal _decimalProp;
		private NullableDouble _doubleProp;
		private NullableGuid _guidProp;
		private NullableInt16 _int16Prop;
		private NullableInt32 _int32Prop;
		private NullableInt64 _int64Prop;
		private NullableSByte _sbyteProp;
		private NullableSingle _singleProp;
		
		public NullablesClass()
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

		public NullableBoolean BooleanProp
		{
			get { return _booleanProp; }
			set { _booleanProp = value; }
		}

		public NullableByte ByteProp
		{
			get { return _byteProp; }
			set { _byteProp = value; }
		}

		public NullableDateTime DateTimeProp
		{
			get { return _dateTimeProp; }
			set { _dateTimeProp = value; }
		}

		public NullableDecimal DecimalProp
		{
			get { return _decimalProp; }
			set { _decimalProp = value; }
		}

		public NullableDouble DoubleProp
		{
			get { return _doubleProp; }
			set { _doubleProp = value; }
		}

		public NullableGuid GuidProp
		{
			get { return _guidProp; }
			set { _guidProp = value; }
		}

		public NullableInt16 Int16Prop
		{
			get { return _int16Prop; }
			set { _int16Prop = value; }
		}

		public NullableInt32 Int32Prop
		{
			get { return _int32Prop; }
			set { _int32Prop = value; }
		}

		public NullableInt64 Int64Prop
		{
			get { return _int64Prop; }
			set { _int64Prop = value; }
		}

		public NullableSByte SByteProp 
		{
			get { return _sbyteProp; }
			set { _sbyteProp = value; }
		}

		public NullableSingle SingleProp
		{
			get { return _singleProp; }
			set { _singleProp = value; }
		}

		

	}
}
