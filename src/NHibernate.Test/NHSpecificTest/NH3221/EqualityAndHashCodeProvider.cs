namespace NHibernate.Test.NHSpecificTest.NH3221
{
	/// <summary>
	/// This is a trivial class that is used to make sure that Equals and GetHashCode
	/// are properly overloaded with the correct semantics. This is exteremely important
	/// if you are going to deal with objects outside the current Unit of Work.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public abstract class EqualityAndHashCodeProvider<T, TKey>
		where T : EqualityAndHashCodeProvider<T, TKey>
	{
		private int? oldHashCode;

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			T other = obj as T;
			if (other == null)
				return false;
			//to handle the case of comparing two new objects
			bool otherIsTransient = Equals(other.Id, default(TKey));
			bool thisIsTransient = Equals(this.Id, default(TKey));
			if (otherIsTransient && thisIsTransient)
				return ReferenceEquals(other, this);
			return other.Id.Equals(Id);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override int GetHashCode()
		{
			//This is done se we won't change the hash code
			if (oldHashCode.HasValue)
				return oldHashCode.Value;
			bool thisIsTransient = Equals(this.Id, default(TKey));
			//When we are transient, we use the base GetHashCode()
			//and remember it, so an instance can't change its hash code.
			if (thisIsTransient)
			{
				oldHashCode = base.GetHashCode();
				return oldHashCode.Value;
			}
			return Id.GetHashCode();
		}

		/// <summary>
		/// Get or set the Id of this entity
		/// </summary>
		public abstract TKey Id { get; protected set; }

		/// <summary>
		/// Equality operator so we can have == semantics
		/// </summary>
		public static bool operator ==(EqualityAndHashCodeProvider<T, TKey> x, EqualityAndHashCodeProvider<T, TKey> y)
		{
			return Equals(x, y);
		}

		/// <summary>
		/// Inequality operator so we can have != semantics
		/// </summary>
		public static bool operator !=(EqualityAndHashCodeProvider<T, TKey> x, EqualityAndHashCodeProvider<T, TKey> y)
		{
			return !(x == y);
		}
	}
}