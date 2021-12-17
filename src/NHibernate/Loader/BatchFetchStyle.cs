namespace NHibernate.Loader
{
	/// <summary>
	/// Defines the style that should be used to perform batch loading.
	/// </summary>
	public enum BatchFetchStyle
	{
		/// <summary>
		/// The legacy algorithm where we keep a set of pre-built batch sizes.  Batches are performed
		/// using the next-smaller pre-built batch size from the number of existing batchable identifiers.
		/// <p/>
		/// For example, with a batch-size setting of 32 the pre-built batch sizes would be [32, 16, 10, 9, 8, 7, .., 1].
		/// An attempt to batch load 31 identifiers would result in batches of 16, 10, and 5.
		/// </summary>
		Legacy,

		// /// <summary>
		// /// Still keeps the concept of pre-built batch sizes, but uses the next-bigger batch size and pads the extra
		// /// identifier placeholders.
		// /// <p/>
		// /// Using the same example of a batch-size setting of 32 the pre-built batch sizes would be the same.  However, the
		// /// attempt to batch load 31 identifiers would result just a single batch of size 32.  The identifiers to load would
		// /// be "padded" (aka, repeated) to make up the difference.
		// /// </summary>
		// Padded,

		/// <summary>
		/// Dynamically builds its SQL based on the actual number of available ids.  Does still limit to the batch-size
		/// defined on the entity/collection
		/// </summary>
		Dynamic,
	}
}
