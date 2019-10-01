using System;
using System.Threading;

namespace NHibernate.Action
{
	/// <summary>
	/// Acts as a stand-in for an entity identifier which is supposed to be
	/// generated on insert (like an IDENTITY column), when an entity is <c>Persist</c>ed.
	/// <c>Save</c> still performs the insert.
	/// </summary>
	/// <remarks>
	/// The stand-in is only used within the <see cref="NHibernate.Engine.IPersistenceContext"/>
	/// in order to distinguish one instance from another; it is never injected into
	/// the entity instance or returned to the client.
	/// </remarks>
	[Serializable]
	public class DelayedPostInsertIdentifier
	{
		private static long GlobalSequence = 0;
		private readonly long sequence;

		public DelayedPostInsertIdentifier()
		{
			sequence = Interlocked.Increment(ref GlobalSequence);
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(this,obj)) 
				return true;
			return Equals(obj as DelayedPostInsertIdentifier);
		}

		public bool Equals(DelayedPostInsertIdentifier that)
		{
			return that == null ? false : sequence == that.sequence;
		}

		public override int GetHashCode()
		{
			return sequence.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("<delayed:{0}>", sequence);
		}
	}
}
