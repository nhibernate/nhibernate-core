using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Param
{
	class CollectionFilterKeyParameterSpecification : IParameterSpecification
	{
		private readonly string _collectionRole;
		private readonly IType _keyType;
		private int _queryParameterPosition;

		/// <summary>
		/// Creates a specialized collection-filter collection-key parameter spec.
		/// </summary>
		/// <param name="collectionRole">The collection role being filtered.</param>
		/// <param name="keyType">The mapped collection-key type.</param>
		/// <param name="queryParameterPosition">The position within QueryParameters where we can find the appropriate param value to bind.</param>
		public CollectionFilterKeyParameterSpecification(string collectionRole, IType keyType, int queryParameterPosition)
		{
			_collectionRole = collectionRole;
			_keyType = keyType;
			_queryParameterPosition = queryParameterPosition;
		}

		public int Bind(
			IDbCommand statement,
			QueryParameters qp,
			ISessionImplementor session,
			int position)
		{
			object value = qp.PositionalParameterValues[_queryParameterPosition];
			_keyType.NullSafeSet(statement, value, position, session);
			return _keyType.GetColumnSpan(session.Factory);
		}

		public IType ExpectedType
		{
			get { return _keyType; }
			set { throw new InvalidOperationException(); }
		}

		public string RenderDisplayInfo()
		{
			return "collection-filter-key=" + _collectionRole;
		}
	}
}