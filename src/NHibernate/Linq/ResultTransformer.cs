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
		private readonly Func<object[], object> _itemTransformation; // TODO 6.0: Remove
		private readonly Func<IEnumerable<object>, object> _listTransformation; // TODO 6.0: Remove
		private readonly Func<object[], object[], object> _itemTransformationParams; // TODO 6.0: Rename to _itemTransformation
		private readonly Func<IEnumerable<object>, object[], object> _listTransformationParams; // TODO 6.0: Rename to _listTransformation
		private readonly object[] _parameterValues;

		// Since v5.3
		[Obsolete("Use overload with Func<object[], object[], object> parameter instead.")]
		public ResultTransformer(Func<object[], object> itemTransformation, Func<IEnumerable<object>, object> listTransformation)
		{
			_itemTransformation = itemTransformation;
			_listTransformation = listTransformation;
		}

		public ResultTransformer(
			Func<object[], object[], object> itemTransformation,
			Func<IEnumerable<object>, object[], object> listTransformation)
		{
			_itemTransformationParams = itemTransformation;
			_listTransformationParams = listTransformation;
		}

		private ResultTransformer(
				Func<object[], object[], object> itemTransformation,
				Func<IEnumerable<object>, object[], object> listTransformation,
				object[] parameterValues,
				Func<object[], object> itemTransformationOld, // TODO 6.0: Remove
				Func<IEnumerable<object>, object> listTransformationOld // TODO 6.0: Remove
			)
		{
			_itemTransformationParams = itemTransformation;
			_listTransformationParams = listTransformation;
			_parameterValues = parameterValues;
			_itemTransformation = itemTransformationOld;
			_listTransformation = listTransformationOld;
		}

		internal ResultTransformer WithParameterValues(object[] parameterValues)
		{
			return new ResultTransformer(
				_itemTransformationParams,
				_listTransformationParams,
				parameterValues,
				_itemTransformation,
				_listTransformation);
		}

		#region IResultTransformer Members

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			if (_itemTransformationParams == null && _itemTransformation == null)
			{
				return tuple;
			}

			return _itemTransformationParams != null
				? _itemTransformationParams(tuple, _parameterValues)
				: _itemTransformation(tuple);
		}

		public IList TransformList(IList collection)
		{
			if (_listTransformation == null && _listTransformationParams == null)
			{
				return collection;
			}

			var toTransform = GetToTransform(collection);
			var transformResult = _listTransformationParams != null
				? _listTransformationParams(toTransform, _parameterValues)
				: _listTransformation(toTransform);

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
			return Equals(other._listTransformation, _listTransformation) &&
				Equals(other._itemTransformation, _itemTransformation) &&
				Equals(other._listTransformationParams, _listTransformationParams) &&
				Equals(other._itemTransformationParams, _itemTransformationParams);
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
				int lt2 = (_listTransformationParams != null ? _listTransformationParams.GetHashCode() : 0);
				int it2 = (_itemTransformationParams != null ? _itemTransformationParams.GetHashCode() : 0);
				return (lt*397) ^ (it*17) ^ (lt2 * 397) ^ (it2 * 17);
			}
		}
	}
}
