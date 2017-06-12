using System;
using System.Threading;

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
		private static readonly object _locker = new object();
		private static AsyncLocal<long> _sequence;
		private readonly long _sequenceValue;

		public DelayedPostInsertIdentifier()
		{
			lock (_locker)
			{
				if (_sequence == null)
					_sequence = new AsyncLocal<long> { Value = 0 };
				if (_sequence.Value == long.MaxValue)
				{
					_sequence.Value = 0;
				}
				_sequenceValue = _sequence.Value + 1;
				_sequence.Value = _sequenceValue;
			}
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
				return true;
			return Equals(obj as DelayedPostInsertIdentifier);
		}

		public bool Equals(DelayedPostInsertIdentifier that)
		{
			return that == null ? false : _sequenceValue == that._sequenceValue;
		}

		public override int GetHashCode()
		{
			return _sequenceValue.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("<delayed:{0}>", _sequenceValue);
		}
	}
}