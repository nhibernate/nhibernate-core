using System;
using System.Collections;
using System.Data.Common;
using System.Text;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary> Logic to bind stream of byte into a VARBINARY </summary>
	[Serializable]
	public abstract partial class AbstractBinaryType : MutableType, IVersionType, IComparer
	{
		internal AbstractBinaryType() : this(new BinarySqlType())
		{
		}

		internal AbstractBinaryType(BinarySqlType sqlType)
			: base(sqlType)
		{
		}

		#region IVersionType Members

		//      Note : simply returns null for seed() and next() as the only known
		//      application of binary types for versioning is for use with the
		//      TIMESTAMP datatype supported by Sybase and SQL Server, which
		//      are completely db-generated values...

		public object Next(object current, ISessionImplementor session)
		{
			return current;
		}

		public object Seed(ISessionImplementor session)
		{
			return null;
		}

		public override bool IsEqual(object x, object y)
		{
			if (x == y)
				return true;

			if (x == null || y == null)
				return false;

			return ArrayHelper.ArrayEquals(ToInternalFormat(x), ToInternalFormat(y));
		}

		public IComparer Comparator
		{
			get { return this; }
		}

		#endregion

		public abstract override string Name { get; }

		/// <summary> Convert the byte[] into the expected object type</summary>
		protected internal abstract object ToExternalFormat(byte[] bytes);

		/// <summary> Convert the object into the internal byte[] representation</summary>
		protected internal abstract byte[] ToInternalFormat(object bytes);

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			byte[] internalValue = ToInternalFormat(value);

			var parameter = cmd.Parameters[index];

			// set the parameter value before the size check, since ODBC changes the size automatically
			parameter.Value = internalValue;

			// Avoid silent truncation which happens in ADO.NET if the parameter size is set.
			if (parameter.Size > 0 && internalValue.Length > parameter.Size)
				throw new HibernateException("The length of the byte[] value exceeds the length configured in the mapping/parameter.");
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			int length = (int) rs.GetBytes(index, 0, null, 0, 0);
			byte[] buffer = new byte[length];
			if (length > 0)
			{
				// The "if" is to make happy MySQL NH-2096
				rs.GetBytes(index, 0, buffer, 0, length);
			}
			return ToExternalFormat(buffer);
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
		}

		public override int GetHashCode(object x)
		{
			byte[] bytes = ToInternalFormat(x);
			int hashCode = 1;
			unchecked
			{
				for (int j = 0; j < bytes.Length; j++)
				{
					hashCode = 31 * hashCode + bytes[j];
				}
			}
			return hashCode;
		}

		public override int Compare(object x, object y)
		{
			byte[] xbytes = ToInternalFormat(x);
			byte[] ybytes = ToInternalFormat(y);
			if (xbytes.Length < ybytes.Length)
				return -1;
			if (xbytes.Length > ybytes.Length)
				return 1;
			for (int i = 0; i < xbytes.Length; i++)
			{
				if (xbytes[i] < ybytes[i])
					return -1;
				if (xbytes[i] > ybytes[i])
					return 1;
			}
			return 0;
		}

		public override string ToString(object val)
		{
			// convert to HEX string
			byte[] bytes = ToInternalFormat(val);
			StringBuilder buf = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
			{
				String hexStr = Convert.ToString(bytes[i] - Byte.MinValue, 16);
				if (hexStr.Length == 1)
					buf.Append('0');
				buf.Append(hexStr);
			}
			return buf.ToString();
		}

		public override object DeepCopyNotNull(object value)
		{
			byte[] bytes = ToInternalFormat(value);
			byte[] result = new byte[bytes.Length];
			Array.Copy(bytes, 0, result, 0, bytes.Length);
			return ToExternalFormat(result);
		}

		public override object FromStringValue(string xml)
		{
			if (xml == null)
				return null;
			if (xml.Length % 2 != 0)
				throw new ArgumentException("The string is not a valid xml representation of a binary content.");

			byte[] bytes = new byte[xml.Length / 2];
			for (int i = 0; i < bytes.Length; i++)
			{
				string hexStr = xml.Substring(i * 2, ((i + 1) * 2) - (i * 2));
				bytes[i] = (byte) (Convert.ToInt32(hexStr, 16) + Byte.MinValue);
			}
			return ToExternalFormat(bytes);
		}
	}
}
