using System;

namespace NHibernate.Action
{
	/// <summary>
	/// Acts as a stand-in for an entity identifier which is supposed to be
	/// generated on insert (like an IDENTITY column) where the insert needed to
	/// be delayed because we were outside a transaction when the persist
	/// occurred (save currently still performs the insert).
	/// 
	/// The stand-in is only used within the see cref="NHibernate.Engine.PersistenceContext"
	/// in order to distinguish one instance from another; it is never injected into
	/// the entity instance or returned to the client...
	/// </summary>
	[Serializable]
	public class DelayedPostInsertIdentifier
	{
		[ThreadStatic]
		private static long _Sequence = 0;
		private readonly long sequence;

		public DelayedPostInsertIdentifier()
		{
			lock (typeof(DelayedPostInsertIdentifier))
			{
				if (_Sequence == long.MaxValue)
				{
					_Sequence = 0;
				}
				sequence = _Sequence++;
			}
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