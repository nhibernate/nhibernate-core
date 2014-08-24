#region The Apache Software License
/*
 *  Copyright 2001-2004 The Apache Software Foundation
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using System;

namespace NHibernate.Util
{
	/// <summary>
	/// An implementation of a Map which has a maximum size and uses a Least Recently Used
	/// algorithm to remove items from the Map when the maximum size is reached and new items are added.
	/// </summary> 	
	[Serializable]
	public class LRUMap : SequencedHashMap
	{
		private int maximumSize;

		public LRUMap()
			: this(100) {}

		public LRUMap(int capacity):base(capacity)
		{
			maximumSize = capacity;
		}

		public override object this[object key]
		{
			get
			{
				object result = base[key];
				if (result == null) return null;

				Remove(key);
				base.Add(key, result);
				return result;
			}
			set
			{
				Add(key, value);
			}
		}

		public override void Add(object key, object value)
		{
			if (maximumSize == 0)
			{
				return;
			}
			int mapSize = Count;
			if (mapSize >= maximumSize)
			{
				// don't retire LRU if you are just
				// updating an existing key
				if (!ContainsKey(key))
				{
					// lets retire the least recently used item in the cache
					RemoveLRU();
				}
			}

			base[key] = value;
		}

		private void RemoveLRU()
		{
			object key = FirstKey;
			if (ReferenceEquals(null, key))
			{
				return;
			}
			// be sure to call super.get(key), or you're likely to 
			// get infinite promotion recursion
			object value = base[key];

			Remove(key);

			ProcessRemovedLRU(key, value);
		}

		protected void ProcessRemovedLRU(object key, object value)
		{
		}

		public int MaximumSize
		{
			get { return maximumSize; }
			set
			{
				maximumSize = value;
				while (Count > maximumSize)
					RemoveLRU();
			}
		}

	}
}
