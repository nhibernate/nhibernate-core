namespace NHibernate.Engine
{
	/// <summary>
	/// Information to determine how to run an IDbCommand and what
	/// records to return from the IDataReader.
	/// </summary>
	public sealed class RowSelection
	{
		/// <summary>
		/// Indicates that the no value has been set on the Property.
		/// </summary>
		public static readonly int NoValue = -1;

		// framework defaults value to 0
		private int firstRow;
		private int maxRows = RowSelection.NoValue;
		private int timeout = RowSelection.NoValue;

		/// <summary>
		/// Gets or Sets the Index of the First Row to Select
		/// </summary>
		/// <value>The Index of the First Rows to Select</value>
		/// <remarks>Defaults to 0 unless specifically set.</remarks>
		public int FirstRow
		{
			get { return firstRow; }
			set { firstRow = value; }
		}

		/// <summary>
		/// Gets or Sets the Maximum Number of Rows to Select
		/// </summary>
		/// <value>The Maximum Number of Rows to Select</value>
		/// <remarks>Defaults to NoValue unless specifically set.</remarks>
		public int MaxRows
		{
			get { return maxRows; }
			set { maxRows = value; }
		}

		/// <summary>
		/// Gets or Sets the Timeout of the Query
		/// </summary>
		/// <value>The Query Timeout</value>
		/// <remarks>Defaults to NoValue unless specifically set.</remarks>
		public int Timeout
		{
			get { return timeout; }
			set { timeout = value; }
		}
	}
}