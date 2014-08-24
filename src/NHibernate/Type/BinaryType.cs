using System;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// BinaryType.
	/// </summary>
	[Serializable]
	public class BinaryType : AbstractBinaryType
	{
		internal BinaryType()
			: this(new BinarySqlType())
		{
		}

		internal BinaryType(BinarySqlType sqlType)
			: base(sqlType)
		{
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(byte[]); }
		}

		public override string Name
		{
			get { return "Byte[]"; }
		}

		protected internal override object ToExternalFormat(byte[] bytes)
		{
			return bytes;
		}

		protected internal override byte[] ToInternalFormat(object bytes)
		{
			return (byte[]) bytes;
		}

		public override int Compare(object x, object y)
		{
			return 0;
		}
	}
}