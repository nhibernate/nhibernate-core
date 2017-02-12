using System.Collections.Generic;
using System.Collections.ObjectModel;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// A list of <see cref="Parameter"/> that maintains a cache of backtrace positions for performance purpose.
	/// See https://nhibernate.jira.com/browse/NH-3489.
	/// </summary>
	internal class BackTrackCacheParameterList : Collection<Parameter>
	{
		private Dictionary<string, SortedSet<int>> _indexesByBackTrace;

		private void AddIndex(Parameter parameter, int index)
		{
			var backTrack = parameter.BackTrack as string;
			if (backTrack == null)
				return;

			SortedSet<int> indexes;
			if (!_indexesByBackTrace.TryGetValue(backTrack, out indexes))
			{
				indexes = new SortedSet<int>();
				_indexesByBackTrace.Add(backTrack, indexes);
			}
			indexes.Add(index);
		}

		private void RemoveIndexes(Parameter parameter)
		{
			var backTrack = parameter.BackTrack as string;
			if (backTrack != null)
				_indexesByBackTrace.Remove(backTrack);
		}

		private Dictionary<string, SortedSet<int>> BuildBackTrackCache()
		{
			var indexesByBackTrace = new Dictionary<string, SortedSet<int>>();
			IList<Parameter> parameters = Items;
			for (int i = 0; i < parameters.Count; i++)
			{
				var backTrace = parameters[i].BackTrack as string;
				if (backTrace != null)
				{
					SortedSet<int> locations;
					if (!indexesByBackTrace.TryGetValue(backTrace, out locations))
					{
						locations = new SortedSet<int>();
						indexesByBackTrace.Add(backTrace, locations);
					}
					locations.Add(i);
				}
			}
			return indexesByBackTrace;
		}

		protected override void InsertItem(int index, Parameter item)
		{
			base.InsertItem(index, item);
			if (_indexesByBackTrace != null)
				AddIndex(item, index);
		}

		protected override void RemoveItem(int index)
		{
			Parameter oldItem = Items[index];
			base.RemoveItem(index);
			if (_indexesByBackTrace != null)
				RemoveIndexes(oldItem);
		}

		protected override void SetItem(int index, Parameter item)
		{
			Parameter oldItem = Items[index];
			base.SetItem(index, item);
			if (_indexesByBackTrace != null)
			{
				RemoveIndexes(oldItem);
				AddIndex(item, index);
			}
		}

		protected override void ClearItems()
		{
			base.ClearItems();
			if (_indexesByBackTrace != null)
				_indexesByBackTrace.Clear();
		}

		public IEnumerable<int> GetEffectiveParameterLocations(string backTrace)
		{
			if (backTrace != null)
			{
				if (_indexesByBackTrace == null)
					_indexesByBackTrace = BuildBackTrackCache();
				SortedSet<int> indexes;
				if (_indexesByBackTrace.TryGetValue(backTrace, out indexes))
					return indexes;
			}
			return ArrayHelper.EmptyIntArray;
		}
	}
}