using System;
using System.Data;

namespace NHibernate.Driver
{
	/// <summary>
	/// An implementation of <see cref="IDataReader"/> that will work with either an 
	/// <see cref="IDataReader"/> returned by Execute or with an <see cref="IDataReader"/>
	/// whose contents have been read into a <see cref="NDataReader"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This allows NHibernate to use the underlying <see cref="IDataReader"/> for as long as
	/// possible without the need to read everything into the <see cref="NDataReader"/>.
	/// </para>
	/// <para>
	/// The consumer of the <see cref="IDataReader"/> returned from <see cref="Engine.IBatcher"/> does
	/// not need to know the underlying reader and can use it the same even if it switches from an
	/// <see cref="IDataReader"/> to <see cref="NDataReader"/> in the middle of its use.
	/// </para>
	/// </remarks>
	public class NHybridDataReader : IDataReader
	{
		log4net.ILog log = log4net.LogManager.GetLogger( typeof(NHybridDataReader) );

		IDataReader _reader;
		bool _isMidstream = false;

		/// <summary>
		/// Initializes a new instance of the NHybridDataReader class.
		/// </summary>
		/// <param name="reader">The underlying IDataReader to use.</param>
		public NHybridDataReader(IDataReader reader) : this(reader, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the NHybridDataReader class.
		/// </summary>
		/// <param name="reader">The underlying IDataReader to use.</param>
		/// <param name="inMemory"><c>true</c> if the contents of the IDataReader should be read into memory right away.</param>
		public NHybridDataReader(IDataReader reader, bool inMemory) 
		{
			if( inMemory ) 
			{
				_reader = new NDataReader( reader, false );
			}
			else 
			{
				_reader = reader;
			}
		}

		/// <summary>
		/// Reads all of the contents into memory because another <see cref="IDataReader"/>
		/// needs to be opened.
		/// </summary>
		/// <remarks>
		/// This will result in a no op if the reader is closed or is already in memory.
		/// </remarks>
		public void ReadIntoMemory() 
		{
			if( _reader.IsClosed==false && _reader.GetType()!=typeof(NDataReader) ) 
			{
				if( log.IsDebugEnabled ) 
				{
					log.Debug("Moving IDataReader into an NDataReader.  It was converted in midstream " + _isMidstream.ToString() );
				}
				_reader = new NDataReader( _reader, _isMidstream );
			}
		}

		/// <summary>
		/// Gets if the object is in the middle of reading a Result.
		/// </summary>
		/// <value><c>true</c> if NextResult and Read have been called on the <see cref="IDataReader"/>.</value>
		public bool IsMidstream 
		{
			get { return _isMidstream; }
		}

		#region IDataReader Members

		public int RecordsAffected
		{
			get { return _reader.RecordsAffected; }
		}

		public bool IsClosed
		{
			get { return _reader.IsClosed; }
		}

		public bool NextResult()
		{
			// we are not in middle of a result
			_isMidstream = false;
			return _reader.NextResult();
		}

		public void Close()
		{
			_reader.Close();
		}

		public bool Read()
		{
			_isMidstream = true;
			return _reader.Read();
		}

		public int Depth
		{
			get { return _reader.Depth; }
		}

		public DataTable GetSchemaTable()
		{
			return _reader.GetSchemaTable();
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			_reader.Dispose();
		}

		#endregion

		#region IDataRecord Members

		public int GetInt32(int i)
		{
			return _reader.GetInt32(i);
		}

		public object this[string name]
		{
			get { return _reader[name]; }
		}

		object System.Data.IDataRecord.this[int i]
		{
			get { return _reader[i]; }
		}

		public object GetValue(int i)
		{
			return _reader.GetValue(i);
		}

		public bool IsDBNull(int i)
		{
			return _reader.IsDBNull(i);
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return _reader.GetBytes( i, fieldOffset, buffer, bufferoffset, length );
		}

		public byte GetByte(int i)
		{
			return _reader.GetByte(i);
		}

		public System.Type GetFieldType(int i)
		{
			return _reader.GetFieldType(i);
		}

		public decimal GetDecimal(int i)
		{
			return _reader.GetDecimal(i);
		}

		public int GetValues(object[] values)
		{
			return _reader.GetValues( values );
		}

		public string GetName(int i)
		{
			return _reader.GetName(i);
		}

		public int FieldCount
		{
			get { return _reader.FieldCount; }
		}

		public long GetInt64(int i)
		{
			return _reader.GetInt64(i);
		}

		public double GetDouble(int i)
		{
			return _reader.GetDouble(i);
		}

		public bool GetBoolean(int i)
		{
			return _reader.GetBoolean(i);
		}

		public Guid GetGuid(int i)
		{
			return _reader.GetGuid(i);
		}

		public DateTime GetDateTime(int i)
		{
			return _reader.GetDateTime(i);
		}

		public int GetOrdinal(string name)
		{
			return _reader.GetOrdinal(name);
		}

		public string GetDataTypeName(int i)
		{
			return _reader.GetDataTypeName(i);
		}

		public float GetFloat(int i)
		{
			return _reader.GetFloat(i);
		}

		public IDataReader GetData(int i)
		{
			return _reader.GetData(i);
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return _reader.GetChars( i, fieldoffset, buffer, bufferoffset, length );
		}

		public string GetString(int i)
		{
			return _reader.GetString(i);
		}

		public char GetChar(int i)
		{
			return _reader.GetChar(i);
		}

		public short GetInt16(int i)
		{
			return _reader.GetInt16(i);
		}

		#endregion
	}
}
