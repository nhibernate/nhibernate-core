using System;

namespace NHibernate.Test.NHSpecificTest.NH3332
{
	public class DataTypeDescription
	{
		private Culture _culture;
		private DataType _dataType;
		private String _description;
		private Byte[] _rowVersionId;

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	
		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj);
		}
	
		public virtual String Description
		{ 
			get { return _description; }
			set { _description = value; }
		}

		public virtual Byte[] RowVersionId
		{ 
			get { return _rowVersionId; }
		}

		public virtual Culture Culture
		{
			get { return _culture; }
			set { _culture = value; }
		}
		
		public virtual DataType DataType
		{
			get { return _dataType; }
			set { _dataType = value; }
		}
	}
}
