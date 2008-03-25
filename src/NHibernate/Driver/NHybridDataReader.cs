using System;
using System.Data;
using log4net;

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
		private ILog log = LogManager.GetLogger(typeof(NHybridDataReader));

		private IDataReader _reader;
		private bool _isMidstream = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="NHybridDataReader"/> class.
		/// </summary>
		/// <param name="reader">The underlying IDataReader to use.</param>
		public NHybridDataReader(IDataReader reader) : this(reader, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the NHybridDataReader class.
		/// </summary>
		/// <param name="reader">The underlying IDataReader to use.</param>
		/// <param name="inMemory"><see langword="true" /> if the contents of the IDataReader should be read into memory right away.</param>
		public NHybridDataReader(IDataReader reader, bool inMemory)
		{
			if (inMemory)
			{
				_reader = new NDataReader(reader, false);
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
			if (_reader.IsClosed == false && _reader.GetType() != typeof(NDataReader))
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("Moving IDataReader into an NDataReader.  It was converted in midstream " + _isMidstream.ToString());
				}
				_reader = new NDataReader(_reader, _isMidstream);
			}
		}

		/// <summary>
		/// Gets if the object is in the middle of reading a Result.
		/// </summary>
		/// <value><see langword="true" /> if NextResult and Read have been called on the <see cref="IDataReader"/>.</value>
		public bool IsMidstream
		{
			get { return _isMidstream; }
		}

		#region IDataReader Members

		/// <summary></summary>
		public int RecordsAffected
		{
			get { return _reader.RecordsAffected; }
		}

		/// <summary></summary>
		public bool IsClosed
		{
			get { return _reader.IsClosed; }
		}

		/// <summary></summary>
		public bool NextResult()
		{
			// we are not in middle of a result
			_isMidstream = false;
			return _reader.NextResult();
		}

		/// <summary></summary>
		public void Close()
		{
			_reader.Close();
		}

		/// <summary></summary>
		public bool Read()
		{
			_isMidstream = _reader.Read();
			return _isMidstream;
		}

		/// <summary></summary>
		public int Depth
		{
			get { return _reader.Depth; }
		}

		/// <summary></summary>
		public DataTable GetSchemaTable()
		{
			return _reader.GetSchemaTable();
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// A flag to indicate if <c>Disose()</c> has been called.
		/// </summary>
		private bool _isAlreadyDisposed;

		/// <summary>
		/// Finalizer that ensures the object is correctly disposed of.
		/// </summary>
		~NHybridDataReader()
		{
			Dispose(false);
		}

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		public void Dispose()
		{
			log.Debug("running NHybridDataReader.Dispose()");
			Dispose(true);
		}

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		/// <param name="isDisposing">Indicates if this NHybridDataReader is being Disposed of or Finalized.</param>
		/// <remarks>
		/// If this NHybridDataReader is being Finalized (<c>isDisposing==false</c>) then make sure not
		/// to call any methods that could potentially bring this NHybridDataReader back to life.
		/// </remarks>
		protected virtual void Dispose(bool isDisposing)
		{
			if (_isAlreadyDisposed)
			{
				// don't dispose of multiple times.
				return;
			}

			// free managed resources that are being managed by the NHybridDataReader if we
			// know this call came through Dispose()
			if (isDisposing)
			{
				_reader.Dispose();
			}

			// free unmanaged resources here

			_isAlreadyDisposed = true;
			// nothing for Finalizer to do - so tell the GC to ignore it
			GC.SuppressFinalize(this);
		}

		#endregion

		#region IDataRecord Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public int GetInt32(int i)
		{
			return _reader.GetInt32(i);
		}

		/// <summary></summary>
		public object this[string name]
		{
			get { return _reader[name]; }
		}

		/// <summary></summary>
		object IDataRecord.this[int i]
		{
			get { return _reader[i]; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public object GetValue(int i)
		{
			return _reader.GetValue(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public bool IsDBNull(int i)
		{
			return _reader.IsDBNull(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="fieldOffset"></param>
		/// <param name="buffer"></param>
		/// <param name="bufferoffset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return _reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public byte GetByte(int i)
		{
			return _reader.GetByte(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public System.Type GetFieldType(int i)
		{
			return _reader.GetFieldType(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public decimal GetDecimal(int i)
		{
			return _reader.GetDecimal(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public int GetValues(object[] values)
		{
			return _reader.GetValues(values);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public string GetName(int i)
		{
			return _reader.GetName(i);
		}

		/// <summary></summary>
		public int FieldCount
		{
			get { return _reader.FieldCount; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public long GetInt64(int i)
		{
			return _reader.GetInt64(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public double GetDouble(int i)
		{
			return _reader.GetDouble(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public bool GetBoolean(int i)
		{
			return _reader.GetBoolean(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public Guid GetGuid(int i)
		{
			return _reader.GetGuid(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public DateTime GetDateTime(int i)
		{
			return _reader.GetDateTime(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public int GetOrdinal(string name)
		{
			return _reader.GetOrdinal(name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public string GetDataTypeName(int i)
		{
			return _reader.GetDataTypeName(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public float GetFloat(int i)
		{
			return _reader.GetFloat(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public IDataReader GetData(int i)
		{
			return _reader.GetData(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="fieldoffset"></param>
		/// <param name="buffer"></param>
		/// <param name="bufferoffset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return _reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		/// <summary>
		///  
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public string GetString(int i)
		{
			return _reader.GetString(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public char GetChar(int i)
		{
			return _reader.GetChar(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public short GetInt16(int i)
		{
			return _reader.GetInt16(i);
		}

		#endregion
	}
}