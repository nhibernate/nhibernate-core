using System;
using NHibernate.Type;

namespace NHibernate {
	/// <summary>
	/// A result iterator that allows moving around within the results by arbitrary increments.
	/// </summary>
	/// <remarks>
	/// Columns of results are numbered from zero
	/// </remarks>
	public interface IScrollableResults {
		
		/// <summary>
		/// Advance to the next result
		/// </summary>
		/// <returns><c>true</c> if there is another result</returns>
		bool Next();

		/// <summary>
		/// Retreat to the previous result
		/// </summary>
		/// <returns><c>true</c> if there is a previous result</returns>
		bool Previous();

		/// <summary>
		/// Scroll an arbitrary number of locations
		/// </summary>
		/// <param name="i">a positive (forward) or negative (backward) number of rows</param>
		/// <returns><c>true</c> if there is a result at the new location</returns>
		bool Scroll(int i);

		/// <summary>
		/// Go to the last result
		/// </summary>
		/// <returns><c>true</c> if there are any results</returns>
		bool Last();

		/// <summary>
		/// Go to the first result
		/// </summary>
		/// <returns><c>true</c> if there are any results</returns>
		bool First();

		/// <summary>
		/// Go to a location just before first result (this is the initial location)
		/// </summary>
		void BeforeFirst();

		/// <summary>
		/// Go to a location just after the last result
		/// </summary>
		void AfterLast();

		/// <summary>
		/// Is this the first result
		/// </summary>
		bool IsFirst { get; }

		/// <summary>
		/// Is this the last result
		/// </summary>
		bool IsLast { get; }

		/// <summary>
		/// Release resources immediately
		/// </summary>
		void Close();

		/// <summary>
		/// Get the current row of results
		/// </summary>
		/// <returns>an object or array</returns>
		object[] Get();

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
		object Get(int i);

		/// <summary>
		/// Get the type of the <c>i</c>th column of results
		/// </summary>
		/// <param name="i">the column, numbered from zero</param>
		/// <returns>the Hibernate type</returns>
		HibernateType GetType(int i);

		/// <summary>
		/// Convenience method to read an <c>int</c>
		/// </summary>
		int GetInteger(int col);

		/// <summary>
		/// Convenience method to read a <c>long</c>
		/// </summary>
		long GetLong(int col);

		/// <summary>
		/// Convenience method to read a <c>float</c>
		/// </summary>
		float GetFloat(int col);

		/// <summary>
		/// Convenience method to read a <c>bool</c>
		/// </summary>
		bool GetBoolean(int col);

		/// <summary>
		/// Convenience method to read a <c>double</c>
		/// </summary>
		double GetDouble(int col);

		/// <summary>
		/// Convenience method to read an <c>short</c>
		/// </summary>
		short GetShort(int col);

		/// <summary>
		/// Convenience method to read a <c>byte</c>
		/// </summary>
		byte GetByte(int col);

		/// <summary>
		/// Convenience method to read a <c>char</c>
		/// </summary>
		char GetCharacter(int col);

		/// <summary>
		/// Convenience method to read a <c>byte[]</c>
		/// </summary>
		byte[] GetBinary(int col);

		/// <summary>
		/// Convenience method to read a <c>string</c>
		/// </summary>
		string GetString(int col);

		/// <summary>
		/// Convenience method to read a <c>date</c>
		/// </summary>
		DateTime GetDate(int col);
	}
}
