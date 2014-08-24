using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Transform;

namespace NHibernate.Linq
{
	[Serializable]
	public class ResultTransformer : IResultTransformer, IEquatable<ResultTransformer>
	{
		private readonly Func<object[], object> _itemTransformation;
		private readonly Func<IEnumerable<object>, object> _listTransformation;

		public ResultTransformer(Func<object[], object> itemTransformation, Func<IEnumerable<object>, object> listTransformation)
		{
			_itemTransformation = itemTransformation;
			_listTransformation = listTransformation;
		}

		#region IResultTransformer Members

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			return _itemTransformation == null ? tuple : _itemTransformation(tuple);
		}

		public IList TransformList(IList collection)
		{
			if (_listTransformation == null)
			{
				return collection;
			}

			var toTransform = GetToTransform(collection);
			var transformResult = _listTransformation(toTransform);

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