using System;
using System.Collections;
using System.Data;

namespace NHibernate.Driver 
{

	/// <summary>
	/// Some Data Providers (ie - SqlClient) do not support Multiple Active Result Sets (MARS).
	/// NHibernate relies on being able to create MARS to read Components and entities inside
	/// of Collections.
	/// </summary>
	/// <remarks>
	/// This is a completely off-line DataReader - the underlying IDataReader that was used to create
	/// this has been closed and no connections to the Db exists.
	/// </remarks>
	public class NDataReader : IDataReader
	{
		private NDataReader.NResult[] results; 

		private bool isClosed = false;
		
		// a DataReader is positioned before the first valid record
		private int currentRowIndex = -1;

		// a DataReader is positioned on the first Result
		private int currentResultIndex = 0;

		private byte[] cachedByteArray;
		private char[] cachedCharArray;
		private int cachedColIndex = -1;

		/// <summary>
		/// Creates a NDataReader from a <see cref="IDataReader" />
		/// </summary>
		/// <param name="reader">The <see cref="IDataReader" /> to get the records from the Database.</param>
		/// <param name="isMidstream"><c>true</c> if we are loading the <see cref="IDataReader" /> in the middle of reading it.</param>
		/// <remarks>
		/// NHibernate attempts to not have to read the contents of an <see cref="IDataReader"/> into memory until it absolutely
		/// has to.  What that means is that it might have processed some records from the <see cref="IDataReader"/> and will
		/// pick up the <see cref="IDataReader"/> midstream so that the underlying <see cref="IDataReader"/> can be closed 
		/// so a new one can be opened.
		/// </remarks>
		public NDataReader(IDataReader reader, bool isMidstream) 
		{
			ArrayList resultList = new ArrayList(2);
	
			try 
			{
				// if we are in midstream of processing a DataReader then we are already
				// positioned on the first row (index=0)
				if( isMidstream ) currentRowIndex = 0;

				// there will be atleast one result 
				resultList.Add( new NResult(reader, isMidstream) );

				while( reader.NextResult() ) 
				{
					// the second, third, nth result is not processed midstream
					resultList.Add( new NResult(reader, false) );
				}

				results = (NResult[]) resultList.ToArray( typeof(NResult) );
			}
			catch(Exception e) 
			{
				throw new ADOException("There was a problem converting an IDataReader to NDataReader", e);
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

		private NDataReader.NResult GetCurrentResult() 
		{
			return results[currentResultIndex];
		}

		private object GetValue(string name) 
		{
			return GetCurrentResult().GetValue(currentRowIndex, name);
		}

		#region IDataReader Members

		public int RecordsAffected
		{
			get
			{
				// TODO:  Add NDataReader.RecordsAffected getter implementation
				throw new NotImplementedException("NDataReader should only be used for SELECT statements!");
			}
		}

		public bool IsClosed
		{
			get
			{
				return isClosed;
			}
		}

		public bool NextResult()
		{
			currentResultIndex++;
			
			if (currentResultIndex >= results.Length) 
			{
				// move it back to the last result
				currentResultIndex--;
				return false;
			}
			ClearCache();

			return true;
		}

		public void Close()
		{
			isClosed = true;
		}

		public bool Read()
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

		public int Depth
		{
			get
			{
				return currentResultIndex;
			}
		}

		public DataTable GetSchemaTable()
		{
			return GetCurrentResult().GetSchemaTable();
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			isClosed = true;
			ClearCache();
			results = null;
		}

		#endregion

		#region IDataRecord Members

		public int GetInt32(int i)
		{
			return Convert.ToInt32( GetValue(i) );
		}

		public object this[string name]
		{
			get { return GetValue(name); }
		}

		public object this[int i]
		{
			get { return GetValue(i); }
		}

		public object GetValue(int i)
		{
			return GetCurrentResult().GetValue(currentRowIndex, i);
		}

		public bool IsDBNull(int i)
		{
			return GetValue(i).Equals(System.DBNull.Value);
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferOffset, int length)
		{
			if(cachedByteArray==null || cachedColIndex!=i) 
			{
				cachedColIndex = i;
				cachedByteArray = (byte[]) GetValue(i);
			}

			long remainingLength = cachedByteArray.Length - fieldOffset;
			
			if(remainingLength < length) 
			{
				length = (int)remainingLength;
			}
			
			Array.Copy(cachedByteArray, fieldOffset, buffer, bufferOffset, length);
			
			return length;
		}

		public byte GetByte(int i)
		{
			return Convert.ToByte( GetValue(i) );
		}

		public System.Type GetFieldType(int i)
		{
			return GetCurrentResult().GetFieldType(i);
		}

		public decimal GetDecimal(int i)
		{
			return Convert.ToDecimal( GetValue(i) );
		}

		public int GetValues(object[] values)
		{
			
			return GetCurrentResult().GetValues(currentRowIndex, values);
		}

		public string GetName(int i)
		{
			return GetCurrentResult().GetName(i);
		}

		public int FieldCount
		{
			get { return GetCurrentResult().GetFieldCount(); }
		}

		public long GetInt64(int i)
		{
			return Convert.ToInt64( GetValue(i) );
		}

		public double GetDouble(int i)
		{
			return Convert.ToDouble( GetValue(i) );
		}

		public bool GetBoolean(int i)
		{
			return Convert.ToBoolean( GetValue(i) );
		}

		public Guid GetGuid(int i)
		{
			return (Guid)GetValue(i);
		}

		public DateTime GetDateTime(int i)
		{
			return Convert.ToDateTime( GetValue(i) );
		}

		public int GetOrdinal(string name)
		{
			return GetCurrentResult().GetOrdinal(name);
		}

		public string GetDataTypeName(int i)
		{
			return GetCurrentResult().GetDataTypeName(i);
		}

		public float GetFloat(int i)
		{
			return Convert.ToSingle( GetValue(i) );
		}

		public IDataReader GetData(int i)
		{
			throw new NotImplementedException("GetData(int) has not been implemented.");
		}

		public long GetChars(int i, long fieldOffset, char[] buffer, int bufferOffset, int length)
		{
			if(cachedCharArray==null || cachedColIndex!=i) 
			{
				cachedColIndex = i;
				cachedCharArray = (char[])GetValue(i);
			}

			long remainingLength = cachedCharArray.Length - fieldOffset;
			
			if(remainingLength < length) 
			{
				length = (int)remainingLength;
			}
			
			Array.Copy(cachedCharArray, fieldOffset, buffer, bufferOffset, length);
			
			return length;
		}

		public string GetString(int i)
		{
			return Convert.ToString( GetValue(i) );
		}

		public char GetChar(int i)
		{
			return Convert.ToChar( GetValue(i) );
		}

		public short GetInt16(int i)
		{
			return Convert.ToInt16( GetValue(i) );
		}

		#endregion

		/// <summary>
		/// Stores a Result from a DataReader in memory.
		/// </summary>
		private class NResult 
		{
			// [row][column]
			private readonly object[][] records;
			private int colCount = 0;

			private DataTable schemaTable;

			// key = field name
			// index = field index
			private readonly IDictionary fieldNameToIndex = new Hashtable();
			private readonly ArrayList fieldIndexToName = new ArrayList();
			private readonly ArrayList fieldTypes = new ArrayList();
			private readonly ArrayList fieldDataTypeNames = new ArrayList();

			/// <summary>
			/// Initializes a new instance of the NResult class.
			/// </summary>
			/// <param name="reader">The IDataReader to populate the Result with.</param>
			/// <param name="isMidstream">
			/// <c>true</c> if the <see cref="IDataReader"/> is already positioned on the record
			/// to start reading from.
			/// </param>
			internal NResult(IDataReader reader, bool isMidstream) 
			{
				schemaTable = reader.GetSchemaTable();
				
				ArrayList recordsList = new ArrayList();
				int rowIndex = 0;

				// if we are in the middle of processing the reader then don't bother
				// to move to the next record - just use the current one.
				while( isMidstream || reader.Read() )				
				{
					if(rowIndex==0) 
					{
						for(int i = 0; i < reader.FieldCount; i++) 
						{
							string fieldName = reader.GetName(i);
							fieldNameToIndex[fieldName] = i;
							fieldIndexToName.Add(fieldName);
							fieldTypes.Add( reader.GetFieldType(i) );
							fieldDataTypeNames.Add( reader.GetDataTypeName(i) );
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
				

				records = (object[][])recordsList.ToArray( typeof(object[]) );

			}

			public string GetDataTypeName(int colIndex) 
			{
				return (string)fieldDataTypeNames[colIndex];
			}

			public int GetFieldCount() 
			{
				return fieldIndexToName.Count;
			}

			public System.Type GetFieldType(int colIndex) 
			{
				return (System.Type)fieldTypes[colIndex];
			}

			public string GetName(int colIndex) 
			{
				return (string)fieldIndexToName[colIndex];
			}
			
			public DataTable GetSchemaTable() 
			{
				return schemaTable; 
			}
			
			public int GetOrdinal(string colName) 
			{
				return (int)fieldNameToIndex[colName];
			}

			public object GetValue(int rowIndex, int colIndex) 
			{
				return records[rowIndex][colIndex];
			}

			public object GetValue(int rowIndex, string colName) 
			{
				return GetValue( rowIndex, GetOrdinal(colName) );
			}

			public int GetValues(int rowIndex, object[] values) 
			{
				Array.Copy(records[rowIndex], 0, values, 0, colCount); 
				return colCount;
			}

			public int RowCount 
			{
				get { return records.Length;}
			}
		}		
	}
}
