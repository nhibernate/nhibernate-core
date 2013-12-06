using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace NHibernate.Driver
{
	/// <summary>
	/// An implementation of <see cref="DbDataReader"/> that will work with either an 
	/// <see cref="DbDataReader"/> returned by Execute or with an <see cref="DbDataReader"/>
	/// whose contents have been read into a <see cref="NDataReader"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This allows NHibernate to use the underlying <see cref="DbDataReader"/> for as long as
	/// possible without the need to read everything into the <see cref="NDataReader"/>.
	/// </para>
	/// <para>
	/// The consumer of the <see cref="DbDataReader"/> returned from <see cref="Engine.IBatcher"/> does
	/// not need to know the underlying reader and can use it the same even if it switches from an
	/// <see cref="DbDataReader"/> to <see cref="NDataReader"/> in the middle of its use.
	/// </para>
	/// </remarks>
	public class NHybridDataReader : DbDataReader
	{
		private readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(NHybridDataReader));

		private DbDataReader _reader;
		private bool _isMidstream;

		public DbDataReader Target { get { return _reader; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="NHybridDataReader"/> class.
		/// </summary>
		/// <param name="reader">The underlying DbDataReader to use.</param>
		public NHybridDataReader(DbDataReader reader) : this(reader, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the NHybridDataReader class.
		/// </summary>
		/// <param name="reader">The underlying DbDataReader to use.</param>
		/// <param name="inMemory"><see langword="true" /> if the contents of the DbDataReader should be read into memory right away.</param>
		public NHybridDataReader(DbDataReader reader, bool inMemory)
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
		/// Reads all of the contents into memory because another <see cref="DbDataReader"/>
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
					log.Debug("Moving DbDataReader into an NDataReader.  It was converted in midstream " + _isMidstream.ToString());
				}
				_reader = new NDataReader(_reader, _isMidstream);
			}
		}

		/// <summary>
		/// Gets if the object is in the middle of reading a Result.
		/// </summary>
		/// <value><see langword="true" /> if NextResult and Read have been called on the <see cref="DbDataReader"/>.</value>
		public bool IsMidstream
		{
			get { return _isMidstream; }
		}

		/// <summary></summary>
		public override int RecordsAffected
		{
			get { return _reader.RecordsAffected; }
		}

		public override bool HasRows
		{
			get { return _reader.HasRows; }
		}

		/// <summary></summary>
		public override bool IsClosed
		{
			get { return _reader.IsClosed; }
		}

		/// <summary></summary>
		public override bool NextResult()
		{
			// we are not in middle of a result
			_isMidstream = false;
			return _reader.NextResult();
		}

		/// <summary></summary>
		public override void Close()
		{
			_reader.Close();
		}

		/// <summary></summary>
		public override bool Read()
		{
			_isMidstream = _reader.Read();
			return _isMidstream;
		}

		/// <summary></summary>
		public override int Depth
		{
			get { return _reader.Depth; }
		}

		/// <summary></summary>
		public override DataTable GetSchemaTable()
		{
			return _reader.GetSchemaTable();
		}

		/// <summary>
		/// A flag to indicate if <c>Disose()</c> has been called.
		/// </summary>
		private bool disposed;

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
		/// <param name="disposing">Indicates if this NHybridDataReader is being Disposed of or Finalized.</param>
		/// <remarks>
		/// If this NHybridDataReader is being Finalized (<c>isDisposing==false</c>) then make sure not
		/// to call any methods that could potentially bring this NHybridDataReader back to life.
		/// </remarks>
		protected override void Dispose(bool disposing)
		{
			if (disposed)
				return;

			if (disposing && _reader != null)
			{
				_reader.Dispose();
				_reader = null;
			}

			disposed = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override int GetInt32(int i)
		{
			return _reader.GetInt32(i);
		}

		/// <summary></summary>
		public override object this[string name]
		{
			get { return _reader[name]; }
		}

		/// <summary></summary>
		public override object this[int i]
		{
			get { return _reader[i]; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override object GetValue(int i)
		{
			return _reader.GetValue(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override bool IsDBNull(int i)
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
		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return _reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override byte GetByte(int i)
		{
			return _reader.GetByte(i);
		}

		public override IEnumerator GetEnumerator()
		{
			return _reader.GetEnumerator();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override System.Type GetFieldType(int i)
		{
			return _reader.GetFieldType(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override decimal GetDecimal(int i)
		{
			return _reader.GetDecimal(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public override int GetValues(object[] values)
		{
			return _reader.GetValues(values);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override string GetName(int i)
		{
			return _reader.GetName(i);
		}

		/// <summary></summary>
		public override int FieldCount
		{
			get { return _reader.FieldCount; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override long GetInt64(int i)
		{
			return _reader.GetInt64(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override double GetDouble(int i)
		{
			return _reader.GetDouble(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override bool GetBoolean(int i)
		{
			return _reader.GetBoolean(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override Guid GetGuid(int i)
		{
			return _reader.GetGuid(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override DateTime GetDateTime(int i)
		{
			return _reader.GetDateTime(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override int GetOrdinal(string name)
		{
			return _reader.GetOrdinal(name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override string GetDataTypeName(int i)
		{
			return _reader.GetDataTypeName(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override float GetFloat(int i)
		{
			return _reader.GetFloat(i);
		}

		protected override DbDataReader GetDbDataReader(int ordinal)
		{
			return _reader.GetData(ordinal);
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
		public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return _reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		/// <summary>
		///  
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override string GetString(int i)
		{
			return _reader.GetString(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override char GetChar(int i)
		{
			return _reader.GetChar(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override short GetInt16(int i)
		{
			return _reader.GetInt16(i);
		}
	}
}