using NHibernate.Type;

namespace NHibernate.Metadata
{
	/// <summary>
	/// Exposes collection metadata to the application
	/// </summary>
	public interface ICollectionMetadata
	{
		/// <summary>
		/// The collection key type
		/// </summary>
		IType KeyType { get; }

		/// <summary>
		/// The collection element type
		/// </summary>
		IType ElementType { get; }

		/// <summary>
		/// The collection index type (or null if the collection has no index)
		/// </summary>
		IType IndexType { get; }

		/// <summary>
		/// Is the collection indexed?
		/// </summary>
		bool HasIndex { get; }

		/// <summary>
		/// The name of this collection role
		/// </summary>
		string Role { get; }

		/// <summary>
		/// Is the collection an array?
		/// </summary>
		bool IsArray { get; }

		/// <summary>
		/// Is the collection a primitive array?
		/// </summary>
		bool IsPrimitiveArray { get; }

		/// <summary>
		/// Is the collection lazily initialized?
		/// </summary>
		bool IsLazy { get; }
	}
}