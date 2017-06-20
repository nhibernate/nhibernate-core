using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps a sorted <see cref="ISet{T}"/> collection
	/// to the database.
	/// </summary>
	[Serializable]
	public class GenericOrderedSetType<T> : GenericSetType<T>
	{
		// ReSharper disable once StaticMemberInGenericType
		static readonly Lazy<ConstructorInfo> SetConstructor = new Lazy<ConstructorInfo>(GetSetConstructor);

		/// <summary>
		/// Initializes a new instance of a <see cref="GenericOrderedSetType{T}"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		public GenericOrderedSetType(string role, string propertyRef)
			: base(role, propertyRef)
		{
		}

		static ConstructorInfo GetSetConstructor()
		{
			var type = System.Type.GetType("Iesi.Collections.Generic.LinkedHashSet`1, Iesi.Collections", false);
			if (type == null)
				throw new TypeLoadException(
					@"Could not load Iesi.Collections.Generic.LinkedHashSet`1

Please make sure that you have referenced the Iesi.Collections package.");

			return type
				.MakeGenericType(typeof(T))
				.GetConstructor(System.Type.EmptyTypes);
		}

		public override object Instantiate(int anticipatedSize)
		{
			return SetConstructor.Value.Invoke(null);
		}
	}
}
