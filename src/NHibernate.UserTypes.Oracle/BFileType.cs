using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.UserTypes.Oracle
{
	/// <summary>
	/// NHibernate data type for Oracle-specific <c>BFILE</c>.
	/// </summary>
	/// <remarks>
	/// The property of BFileType in hbm.xml file must have a pseudo "ROWID" column following
	/// the real column for NHibernate limit, for example:
	/// <code>
	///		&lt;property type="NHibernate.UserTypes.Oracle.BFileType, NHibernate.UserTypes.Oracle" name="Image"&gt;
	///			&lt;column name="IMAGE" /&gt;
	///			&lt;column name="ROWID" /&gt;
	///		&lt;/property&gt;
	/// </code>
	/// </remarks>
	[Serializable]
	public class BFileType : IUserType
	{
		public static string DbFunc = "BFILENAME"; //this is for oracle
		public string DirName, FileName;
		public byte[] Binary;

		public BFileType()
		{
		}

		public BFileType(string dirName, string fileName, byte[] binary)
		{
			DirName = dirName;
			FileName = fileName;
			Binary = binary;
		}

		public SqlType[] SqlTypes
		{
			get { return new SqlType[] {NHibernateUtil.String.SqlType, NHibernateUtil.String.SqlType}; }
		}

		public System.Type ReturnedType
		{
			get { return typeof(BFileType); }
		}

		public new bool Equals(object x, object y)
		{
			return x == y;
		}

		public int GetHashCode(object x)
		{
			return NHibernateUtil.BinaryBlob.GetHashCode(((BFileType) x).Binary, 0);
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			byte[] bin = (byte[]) NHibernateUtil.BinaryBlob.NullSafeGet(rs, names[0]);
			if (bin != null)
			{
				BFileType bf = new BFileType();
				bf.Binary = bin;
				return bf;
			}
			return null;
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			string cmdText = cmd.CommandText;
			int s = cmdText.IndexOf(DbFunc + '(');
			if (s < 0)
			{
				if (cmdText.StartsWith("INSERT"))
				{
					cmdText = cmdText.Replace(", ROWID", "");
					cmdText = cmdText.Replace(
						string.Format(":p{0}, :p{1}", index, index + 1),
						string.Format("{0}(:p{1}, :p{2})", DbFunc, index, index + 1));
				}
				else //update
				{
					cmdText = cmdText.Replace(
						string.Format(":p{0}, ROWID = :p{1}", index, index + 1),
						string.Format("{0}(:p{1}, :p{2})", DbFunc, index, index + 1));
				}
				cmd.CommandText = cmdText;
			}

			if (value == null)
			{
				((IDataParameter) cmd.Parameters[index]).Value = DBNull.Value;
				((IDataParameter) cmd.Parameters[index + 1]).Value = DBNull.Value;
			}
			else
			{
				BFileType bf = value as BFileType;
				((IDataParameter) cmd.Parameters[index]).Value = bf.DirName;
				((IDataParameter) cmd.Parameters[index + 1]).Value = bf.FileName;
			}
		}

		public object DeepCopy(object value)
		{
			if (value == null)
			{
				return null;
			}

			BFileType valueBFile = (BFileType) value;
			return new BFileType(
				valueBFile.DirName,
				valueBFile.FileName,
				valueBFile.Binary);
		}

		public bool IsMutable
		{
			get { return true; }
		}

		public object Replace(object original, object target, object owner)
		{
			return DeepCopy(original);
		}

		public object Assemble(object cached, object owner)
		{
			return DeepCopy(cached);
		}

		public object Disassemble(object value)
		{
			return DeepCopy(value);
		}
	}
}