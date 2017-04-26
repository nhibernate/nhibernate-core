namespace NHibernate.Id.Enhanced
{
	/// <summary>
	/// Performs optimization on an optimizable identifier generator.  Typically
	/// this optimization takes the form of trying to ensure we do not have to
	/// hit the database on each and every request to get an identifier value.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Optimizers work on constructor injection.  They should provide
	/// a constructor with the following arguments.
	/// </para>
	/// - <see cref="System.Type"/> The return type for the generated values.
	/// - <langword>int</langword> The increment size.
	/// </remarks>
	public interface IOptimizer
	{
		/// <summary>
		/// A common means to access the last value obtained from the underlying
		/// source.  This is intended for testing purposes, since accessing the
		/// underlying database source directly is much more difficult.
		///  </summary>
		/// <value>
		/// The last value we obtained from the underlying source; -1 indicates we have not yet consulted with the source.
		/// </value>
		long LastSourceValue { get; }

		/// <summary>
		/// Defined increment size. 
		/// </summary>
		/// <value> The increment size.</value>
		int IncrementSize { get; }

		/// <summary>
		/// Generate an identifier value accounting for this specific optimization. 
		/// </summary>
		/// <param name="callback">Callback to access the underlying value source. </param>
		/// <returns>The generated identifier value.</returns>
		object Generate(IAccessCallback callback);

		/// <summary> 
		/// Are increments to be applied to the values stored in the underlying
		/// value source?
		/// </summary>
		/// <returns>
		/// True if the values in the source are to be incremented
		/// according to the defined increment size; false otherwise, in which
		/// case the increment is totally an in memory construct.
		/// </returns>
		bool ApplyIncrementSizeToSourceValues { get; }
	}
}