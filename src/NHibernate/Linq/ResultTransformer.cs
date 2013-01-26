using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Transform;

namespace NHibernate.Linq
{
	[Serializable]
	public class ResultTransformer : IResultTransformer
	{
		private readonly Delegate _itemTransformation;
		private readonly Delegate _listTransformation;

		public ResultTransformer(Delegate itemTransformation, Delegate listTransformation)
		{
			_itemTransformation = itemTransformation;
			_listTransformation = listTransformation;
		}

		#region IResultTransformer Members

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			return _itemTransformation == null ? tuple : _itemTransformation.DynamicInvoke(new object[] {tuple});
		}

		public IList TransformList(IList collection)
		{
			if (_listTransformation == null)
			{
				return collection;
			}

			var toTransform = GetToTransform(collection);
			var transformResult = _listTransformation.DynamicInvoke(toTransform);

			var resultList = transformResult as IList;
			return resultList ?? new List<object> { transformResult };
		}

		static IEnumerable<object> GetToTransform(IList collection)
		{
			if (collection.Count > 0)
			{
				var objects = collection[0] as object[];
				if (objects != null && objects.Length == 1)
				{
					return collection.Cast<object[]>().Select(o => o[0]);
				}
			}
			return collection.Cast<object>();
		}

		#endregion

		public bool Equals(ResultTransformer other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other._listTransformation, _listTransformation) && Equals(other._itemTransformation, _itemTransformation);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ResultTransformer);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int lt = (_listTransformation != null ? _listTransformation.GetHashCode() : 0);
				int it = (_itemTransformation != null ? _itemTransformation.GetHashCode() : 0);
				return (lt*397) ^ (it*17);
			}
		}
	}
}