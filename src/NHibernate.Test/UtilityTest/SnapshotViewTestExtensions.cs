using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Test.UtilityTest;

internal static class SnapshotViewTestExtensions
{
	extension<TKey, TValue>(IReadOnlyCollection<KeyValuePair<TKey, TValue>> view) where TKey : class
	{
		internal List<TKey> KeyList() =>
			[.. view.Select(kv => kv.Key)];

		internal List<TValue> ValueList() =>
			[.. view.Select(kv => kv.Value)];

		internal bool HasKey(TKey key) =>
			view.Any(kv => ReferenceEquals(kv.Key, key));

		/// <summary>
		/// The value the snapshot froze for <paramref name="key"/>. Throws if the key is absent, so a
		/// mistaken assertion fails loudly instead of silently comparing against a default.
		/// </summary>
		internal TValue ValueFor(TKey key) =>
			view.Single(kv => ReferenceEquals(kv.Key, key)).Value;

		internal bool TryGetValueByIdentity(
			TKey key,
			out TValue value)
		{
			foreach (var kv in view)
			{
				if (ReferenceEquals(kv.Key, key))
				{
					value = kv.Value;
					return true;
				}
			}

			value = default;
			return false;
		}
	}
}
