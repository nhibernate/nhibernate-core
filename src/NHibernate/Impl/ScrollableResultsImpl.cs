using System;
using System.Data;
using NHibernate;
using NHibernate.Type;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	/// <summary>
	/// Summary description for ScrollableResultsImpl.
	/// </summary>
	public class ScrollableResultsImpl : IScrollableResults
	{
		public ScrollableResultsImpl(IDataReader rs, ISessionImplementor sess, IType[] types)
		{
		}
		/// <summary>
		/// Advance to the next result
		/// </summary>
		/// <returns><c>true</c> if there is another result</returns>
		public bool Next()
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Retreat to the previous result
		/// </summary>
		/// <returns><c>true</c> if there is a previous result</returns>
		public bool Previous()
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Scroll an arbitrary number of locations
		/// </summary>
		/// <param name="i">a positive (forward) or negative (backward) number of rows</param>
		/// <returns><c>true</c> if there is a result at the new location</returns>
		public bool Scroll(int i)
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Go to the last result
		/// </summary>
		/// <returns><c>true</c> if there are any results</returns>
		public bool Last()
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Go to the first result
		/// </summary>
		/// <returns><c>true</c> if there are any results</returns>
		public bool First()
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Go to a location just before first result (this is the initial location)
		/// </summary>
		public void BeforeFirst()
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Go to a location just after the last result
		/// </summary>
		public void AfterLast()
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Is this the first result
		/// </summary>
		public bool IsFirst
		{ 
			get { throw new NotImplementedException("Not ported yet"); }
		}

		/// <summary>
		/// Is this the last result
		/// </summary>
		public bool IsLast 
		{ 
			get { throw new NotImplementedException("Not ported yet"); }
		}

		/// <summary>
		/// Release resources immediately
		/// </summary>
		public void Close()
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Get the current row of results
		/// </summary>
		/// <returns>an object or array</returns>
		public object[] Get()
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Get the <c>i</c>th object in the current row of results, without initializing any
		/// other results in the row
		/// </summary>
		/// <remarks>
		/// This method may be used safely, regardless of the type of the column (ie. even for scalar
		/// results)
		/// </remarks>
		/// <param name="i">the column, numbered from zero</param>
		/// <returns>an object of any Hibernate type or <c>null</c></returns>
		public object Get(int i)
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Get the type of the <c>i</c>th column of results
		/// </summary>
		/// <param name="i">the column, numbered from zero</param>
		/// <returns>the Hibernate type</returns>
		public IType GetType(int i)
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Convenience method to read an <c>int</c>
		/// </summary>
		public int GetInteger(int col)
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Convenience method to read a <c>long</c>
		/// </summary>
		public long GetLong(int col)
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Convenience method to read a <c>float</c>
		/// </summary>
		public float GetFloat(int col)
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Convenience method to read a <c>bool</c>
		/// </summary>
		public bool GetBoolean(int col)
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Convenience method to read a <c>double</c>
		/// </summary>
		public double GetDouble(int col)
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Convenience method to read an <c>short</c>
		/// </summary>
		public short GetShort(int col)
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Convenience method to read a <c>byte</c>
		/// </summary>
		public byte GetByte(int col)
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Convenience method to read a <c>char</c>
		/// </summary>
		public char GetCharacter(int col)
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Convenience method to read a <c>byte[]</c>
		/// </summary>
		public byte[] GetBinary(int col)
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Convenience method to read a <c>string</c>
		/// </summary>
		public string GetString(int col)
		{
			throw new NotImplementedException("Not ported yet");
		}

		/// <summary>
		/// Convenience method to read a <c>date</c>
		/// </summary>
		public DateTime GetDate(int col)
		{
			throw new NotImplementedException("Not ported yet");
		}

	}
}
