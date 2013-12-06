using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.Util;

namespace NHibernate.Driver
{
	/// <summary>
	/// Some Data Providers (ie - SqlClient) do not support Multiple Active Result Sets (MARS).
	/// NHibernate relies on being able to create MARS to read Components and entities inside
	/// of Collections.
	/// </summary>
	/// <remarks>
	/// This is a completely off-line DataReader - the underlying DbDataReader that was used to create
	/// this has been closed and no connections to the Db exists.
	/// </remarks>
	public class NDataReader : DbDataReader
	{
		private NResult[] results;

		private bool isClosed;

		// a DataReader is positioned before the first valid record
		private int currentRowIndex = -1;

		// a DataReader is positioned on the first Result
		private int currentResultIndex = 0;

		private byte[] cachedByteArray;
		private char[] cachedCharArray;
		private int cachedColIndex = -1;

		/// <summary>
		/// Creates a NDataReader from a <see cref="DbDataReader" />
		/// </summary>
		/// <param name="reader">The <see cref="DbDataReader" /> to get the records from the Database.</param>
		/// <param name="isMidstream"><see langword="true" /> if we are loading the <see cref="DbDataReader" /> in the middle of reading it.</param>
		/// <remarks>
		/// NHibernate attempts to not have to read the contents of an <see cref="DbDataReader"/> into memory until it absolutely
		/// has to.  What that means is that it might have processed some records from the <see cref="DbDataReader"/> and will
		/// pick up the <see cref="DbDataReader"/> midstream so that the underlying <see cref="DbDataReader"/> can be closed 
		/// so a new one can be opened.
		/// </remarks>
		public NDataReader(DbDataReader reader, bool isMidstream)
		{
			var resultList = new List<NResult>(2);

			try
			{
				// if we are in midstream of processing a DataReader then we are already
				// positioned on the first row (index=0)
				if (isMidstream)
				{
					currentRowIndex = 0;
				}

				// there will be atleast one result 
				resultList.Add(new NResult(reader, isMidstream));

				while (reader.NextResult())
				{
					// the second, third, nth result is not processed midstream
					resultList.Add(new NResult(reader, false));
				}

				results = resultList.ToArray();
			}
			catch (Exception e)
			{
				throw new ADOException("There was a problem converting an DbDataReader to NDataReader", e);
			}
			finally
			{
				reader.Close();
			}
		}

		/// <summary>
		/// Sets the values that can be cached back to null and sets the 
		/// index of the cached column to -1
		/// </summary>
		private void ClearCache()
		{
			// clear out the caches because we have a new result with diff values.
			cachedByteArray = null;
			cachedCharArray = null;

			cachedColIndex = -1;
		}

		private NResult GetCurrentResult()
		{
			return results[currentResultIndex];
		}

		private object GetValue(string name)
		{
			return GetCurrentResult().GetValue(currentRowIndex, name);
		}

		/// <summary></summary>
		public override int RecordsAffected
		{
			get { throw new NotImplementedException("NDataReader should only be used for SELECT statements!"); }
		}

		public override bool HasRows
		{
			get { return results.LongLength > 0; }
		}

		/// <summary></summary>
		public override bool IsClosed
		{
			get { return isClosed; }
		}

		/// <summary></summary>
		public override bool NextResult()
		{
			currentResultIndex++;
			currentRowIndex = -1;

			if (currentResultIndex >= results.Length)
			{
				// move it back to the last result
				currentResultIndex--;
				return false;
			}
			ClearCache();

			return true;
		}

		/// <summary></summary>
		public override void Close()
		{
			isClosed = true;
		}

		/// <summary></summary>
		public override bool Read()
		{
			currentRowIndex++;

			if (currentRowIndex >= results[currentResultIndex].RowCount)
			{
				// reset it back to the last row
				currentRowIndex--;
				return false;
			}

			ClearCache();

			return true;
		}

		/// <summary></summary>
		public override int Depth
		{
			get { return currentResultIndex; }
		}

		/// <summary></summary>
		public override DataTable GetSchemaTable()
		{
			return GetCurrentResult().GetSchemaTable();
		}

		protected override void Dispose(bool disposing)
		{
			isClosed = true;
			ClearCache();
			results = null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override int GetInt32(int i)
		{
			return Convert.ToInt32(GetValue(i));
		}

		/// <summary></summary>
		public override object this[string name]
		{
			get { return GetValue(name); }
		}

		/// <summary></summary>
		public override object this[int i]
		{
			get { return GetValue(i); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override object GetValue(int i)
		{
			return GetCurrentResult().GetValue(currentRowIndex, i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override bool IsDBNull(int i)
		{
			return GetValue(i).Equals(DBNull.Value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="fieldOffset"></param>
		/// <param name="buffer"></param>
		/// <param name="bufferOffset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferOffset, int length)
		{
			if (cachedByteArray == null || cachedColIndex != i)
			{
				cachedColIndex = i;
				cachedByteArray = (byte[]) GetValue(i);
			}

			long remainingLength = cachedByteArray.Length - fieldOffset;

			if (buffer == null)
			{
				return remainingLength;
			}

			if (remainingLength < length)
			{
				length = (int) remainingLength;
			}

			Array.Copy(cachedByteArray, fieldOffset, buffer, bufferOffset, length);

			return length;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override byte GetByte(int i)
		{
			return Convert.ToByte(GetValue(i));
		}

		public override IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override System.Type GetFieldType(int i)
		{
			return GetCurrentResult().GetFieldType(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override decimal GetDecimal(int i)
		{
			return Convert.ToDecimal(GetValue(i));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public override int GetValues(object[] values)
		{
			return GetCurrentResult().GetValues(currentRowIndex, values);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override string GetName(int i)
		{
			return GetCurrentResult().GetName(i);
		}

		/// <summary></summary>
		public override int FieldCount
		{
			get { return GetCurrentResult().GetFieldCount(); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override long GetInt64(int i)
		{
			return Convert.ToInt64(GetValue(i));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override double GetDouble(int i)
		{
			return Convert.ToDouble(GetValue(i));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override bool GetBoolean(int i)
		{
			return Convert.ToBoolean(GetValue(i));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override Guid GetGuid(int i)
		{
			return (Guid) GetValue(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override DateTime GetDateTime(int i)
		{
			return Convert.ToDateTime(GetValue(i));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override int GetOrdinal(string name)
		{
			return GetCurrentResult().GetOrdinal(name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override string GetDataTypeName(int i)
		{
			return GetCurrentResult().GetDataTypeName(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override float GetFloat(int i)
		{
			return Convert.ToSingle(GetValue(i));
		}

		protected override DbDataReader GetDbDataReader(int ordinal)
		{
			throw new NotImplementedException("GetDbDataReader(int) has not been implemented.");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="fieldOffset"></param>
		/// <param name="buffer"></param>
		/// <param name="bufferOffset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public override long GetChars(int i, long fieldOffset, char[] buffer, int bufferOffset, int length)
		{
			if (cachedCharArray == null || cachedColIndex != i)
			{
				cachedColIndex = i;
				cachedCharArray = (char[]) GetValue(i);
			}

			long remainingLength = cachedCharArray.Length - fieldOffset;

			if (remainingLength < length)
			{
				length = (int) remainingLength;
			}

			Array.Copy(cachedCharArray, fieldOffset, buffer, bufferOffset, length);

			return length;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override string GetString(int i)
		{
			return Convert.ToString(GetValue(i));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override char GetChar(int i)
		{
			return Convert.ToChar(GetValue(i));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override short GetInt16(int i)
		{
			return Convert.ToInt16(GetValue(i));
		}

		/// <summary>
		/// Stores a Result from a DataReader in memory.
		/// </summary>
		private class NResult
		{
			// [row][column]
			private readonly object[][] records;
			private int colCount = 0;

			private readonly DataTable schemaTable;

			// key = field name
			// index = field index
			private readonly IDictionary<string, int> fieldNameToIndex = new Dictionary<string, int>();
			private readonly IList<string> fieldIndexToName = new List<string>();
			private readonly IList<System.Type> fieldTypes = new List<System.Type>();
			private readonly IList<string> fieldDataTypeNames = new List<string>();

			/// <summary>
			/// Initializes a new instance of the NResult class.
			/// </summary>
			/// <param name="reader">The DbDataReader to populate the Result with.</param>
			/// <param name="isMidstream">
			/// <see langword="true" /> if the <see cref="DbDataReader"/> is already positioned on the record
			/// to start reading from.
			/// </param>
			internal NResult(DbDataReader reader, bool isMidstream)
			{
				schemaTable = reader.GetSchemaTable();

				List<object[]> recordsList = new List<object[]>();
				int rowIndex = 0;

				// if we are in the middle of processing the reader then don't bother
				// to move to the next record - just use the current one.
				while (isMidstream || reader.Read())
				{
					if (rowIndex == 0)
					{
						for (int i = 0; i < reader.FieldCount; i++)
						{
							string fieldName = reader.GetName(i);
							fieldNameToIndex[fieldName] = i;
							fieldIndexToName.Add(fieldName);
							fieldTypes.Add(reader.GetFieldType(i));
							fieldDataTypeNames.Add(reader.GetDataTypeName(i));
						}

						colCount = reader.FieldCount;
					}

					rowIndex++;

					object[] colValues = new object[reader.FieldCount];
					reader.GetValues(colValues);
					recordsList.Add(colValues);

					// we can go back to reading a reader like normal and don't need
					// to consider where we started from.
					isMidstream = false;
				}

				records = recordsList.ToArray();
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="colIndex"></param>
			/// <returns></returns>
			public string GetDataTypeName(int colIndex)
			{
				return fieldDataTypeNames[colIndex];
			}

			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			public int GetFieldCount()
			{
				return fieldIndexToName.Count;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="colIndex"></param>
			/// <returns></returns>
			public System.Type GetFieldType(int colIndex)
			{
				return fieldTypes[colIndex];
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="colIndex"></param>
			/// <returns></returns>
			public string GetName(int colIndex)
			{
				return fieldIndexToName[colIndex];
			}

			/// <summary></summary>
			public DataTable GetSchemaTable()
			{
				return schemaTable;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="colName"></param>
			/// <returns></returns>
			public int GetOrdinal(string colName)
			{
				// Martijn Boland, 20041106: perform a case-sensitive search first and if that returns
				// null, perform a case-insensitive search (as being described in the IDataRecord 
				// interface, see http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpref/html/frlrfSystemDataIDataRecordClassItemTopic1.asp.
				// This is necessary for databases that don't preserve the case of field names when
				// they are created without quotes (e.g. DB2, PostgreSQL).
				int value;
				if (fieldNameToIndex.TryGetValue(colName, out value))
					return value;

				foreach (KeyValuePair<string, int> pair in fieldNameToIndex)
				{
					if (StringHelper.EqualsCaseInsensitive(pair.Key, colName))
					{
						return pair.Value;
					}
				}

				throw new IndexOutOfRangeException(String.Format("No column with the specified name was found: {0}.", colName));
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="rowIndex"></param>
			/// <param name="colIndex"></param>
			/// <returns></returns>
			public object GetValue(int rowIndex, int colIndex)
			{
				return records[rowIndex][colIndex];
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="rowIndex"></param>
			/// <param name="colName"></param>
			/// <returns></returns>
			public object GetValue(int rowIndex, string colName)
			{
				return GetValue(rowIndex, GetOrdinal(colName));
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="rowIndex"></param>
			/// <param name="values"></param>
			/// <returns></returns>
			public int GetValues(int rowIndex, object[] values)
			{
				Array.Copy(records[rowIndex], 0, values, 0, colCount);
				return colCount;
			}

			/// <summary></summary>
			public int RowCount
			{
				get { return records.Length; }
			}
		}
	}
}