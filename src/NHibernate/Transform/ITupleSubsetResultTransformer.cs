
namespace NHibernate.Transform
{
	/// <summary>
	/// A ResultTransformer that operates on "well-defined" and consistent
	/// subset of a tuple's elements.
	/// </summary>
	/// <remarks> "Well-defined" means that:
	/// <ol>
	///    <li>
	///        the indexes of tuple elements accessed by an
	///        ITupleSubsetResultTransformer depends only on the aliases
	///        and the number of elements in the tuple; i.e, it does
	///        not depend on the value of the tuple being transformed;
	///    </li>
	///    <li>
	///        any tuple elements included in the transformed value are
	///        unmodified by the transformation;
	///    </li>
	///    <li>
	///        transforming equivalent tuples with the same aliases multiple
	///        times results in transformed values that are equivalent;
	///    </li>
	///    <li>
	///        the result of transforming the tuple subset (only those
	///        elements accessed by the transformer) using only the
	///        corresponding aliases is equivalent to transforming the
	///        full tuple with the full array of aliases;
	///    </li>
	///    <li>
	///        the result of transforming a tuple with non-accessed tuple
	///        elements and corresponding aliases set to null
	///        is equivalent to transforming the full tuple with the
	///        full array of aliases;
	///    </li>
	/// </ol>
	/// </remarks>
	/// 
	/// @author Gail Badner
	public interface ITupleSubsetResultTransformer : IResultTransformer
	{
		/// <summary>
		/// When a tuple is transformed, is the result a single element of the tuple?
		/// </summary>
		/// <param name="aliases">The aliases that correspond to the tuple.</param>
		/// <param name="tupleLength">The number of elements in the tuple.</param>
		/// <returns>True, if the transformed value is a single element of the tuple;
		///        false, otherwise.</returns>
		bool IsTransformedValueATupleElement(string[] aliases, int tupleLength);


		/// <summary>
		/// Returns an array with the i-th element indicating whether the i-th
		/// element of the tuple is included in the transformed value.
		/// </summary>
		/// <param name="aliases">The aliases that correspond to the tuple.</param>
		/// <param name="tupleLength">The number of elements in the tuple.</param>
		/// <returns>Array with the i-th element indicating whether the i-th
		///        element of the tuple is included in the transformed value.</returns>
		bool[] IncludeInTransform(string[] aliases, int tupleLength);
	}
}
