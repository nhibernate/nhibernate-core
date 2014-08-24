using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Param
{
	public class DynamicFilterParameterSpecification : IParameterSpecification
	{
		private const string DynamicFilterParameterIdTemplate = "<dfnh-{0}_span{1}>";

		private readonly IType expectedDefinedType;
		private readonly string filterParameterFullName;
		private IType elementType;

		/// <summary>
		/// Constructs a parameter specification for a particular filter parameter.
		/// </summary>
		/// <param name="filterName">The name of the filter</param>
		/// <param name="parameterName">The name of the parameter</param>
		/// <param name="expectedDefinedType">The paremeter type specified on the filter metadata</param>
		/// <param name="collectionSpan"></param>
		public DynamicFilterParameterSpecification(string filterName, string parameterName, IType expectedDefinedType, int? collectionSpan)
		{
			elementType = expectedDefinedType;
			this.expectedDefinedType = collectionSpan.HasValue ? new CollectionOfValuesType(expectedDefinedType, collectionSpan.Value) : expectedDefinedType;
			filterParameterFullName = filterName + '.' + parameterName;
		}

		#region IParameterSpecification Members

		public void Bind(IDbCommand command, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			Bind(command, sqlQueryParametersList, 0, sqlQueryParametersList, queryParameters, session);
		}

		public void Bind(IDbCommand command, IList<Parameter> multiSqlQueryParametersList, int singleSqlParametersOffset, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			string backTrackId = GetIdsForBackTrack(session.Factory).First(); // just the first because IType suppose the oders in certain sequence

			// The same filterName-parameterName can appear more than once in the whole query
			object value = session.GetFilterParameterValue(filterParameterFullName);
			foreach (int position in multiSqlQueryParametersList.GetEffectiveParameterLocations(backTrackId))
			{
				ExpectedType.NullSafeSet(command, value, position, session);
			}
		}

		public string FilterParameterFullName
		{
			get { return filterParameterFullName; }
		}

		public IType ElementType
		{
			get { return elementType; }
		}

		public IType ExpectedType
		{
			get { return expectedDefinedType; }
			set { throw new InvalidOperationException(); }
		}

		public string RenderDisplayInfo()
		{
			return "dynamic-filter={" + filterParameterFullName + "}";
		}

		public IEnumerable<string> GetIdsForBackTrack(IMapping sessionFactory)
		{
			if (sessionFactory == null)
			{
				throw new ArgumentNullException("sessionFactory");
			}
			int columnSpan = ExpectedType.GetColumnSpan(sessionFactory);
			for (int i = 0; i < columnSpan; i++)
			{
				yield return string.Format(DynamicFilterParameterIdTemplate, filterParameterFullName, i);
			}
		}

		#endregion
		
		public override bool Equals(object obj)
		{
			return Equals(obj as DynamicFilterParameterSpecification);
		}

		[Serializable]
		private class CollectionOfValuesType : IType
		{
			private readonly IType elementType;
			private readonly int valueSpan;

			public CollectionOfValuesType(IType elementType, int valueSpan)
			{
				this.elementType = elementType;
				this.valueSpan = valueSpan;
			}

			public object Disassemble(object value, ISessionImplementor session, object owner)
			{
				throw new InvalidOperationException();
			}

			public object Assemble(object cached, ISessionImplementor session, object owner)
			{
				throw new InvalidOperationException();
			}

			public void BeforeAssemble(object cached, ISessionImplementor session)
			{
			}

			public string Name
			{
				get { return "DynamicFilterCollectionOfValues"; }
			}

			public System.Type ReturnedClass
			{
				get { return typeof(IEnumerable); }
			}

			public bool IsMutable
			{
				get { return false; }
			}

			public bool IsAssociationType
			{
				get { return false; }
			}

			public bool IsXMLElement
			{
				get { return false; }
			}

			public bool IsCollectionType
			{
				get { return false; }
			}

			public bool IsComponentType
			{
				get { return false; }
			}

			public bool IsEntityType
			{
				get { return false; }
			}

			public bool IsAnyType
			{
				get { return false; }
			}

			public SqlType[] SqlTypes(IMapping mapping)
			{
				var sqlTypeSequence = new List<SqlType>(20);
				for (int collectionElement = 0; collectionElement < valueSpan; collectionElement++)
				{
					sqlTypeSequence.AddRange(elementType.SqlTypes(mapping));
				}
				return sqlTypeSequence.ToArray();
			}

			public int GetColumnSpan(IMapping mapping)
			{
				return elementType.GetColumnSpan(mapping) * valueSpan;
			}

			public bool IsDirty(object old, object current, ISessionImplementor session)
			{
				return false;
			}

			public bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
			{
				return false;
			}

			public bool IsModified(object oldHydratedState, object currentState, bool[] checkable, ISessionImplementor session)
			{
				return false;
			}

			public object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner)
			{
				throw new InvalidOperationException();
			}

			public object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner)
			{
				throw new InvalidOperationException();
			}

			public void NullSafeSet(IDbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
			{
				throw new InvalidOperationException();
			}

			public void NullSafeSet(IDbCommand st, object value, int index, ISessionImplementor session)
			{
				var start = index;
				var positions = 0;
				var singleParameterColumnSpan = elementType.GetColumnSpan(session.Factory);

				var collection = (IEnumerable) value;
				foreach (var element in collection)
				{
					elementType.NullSafeSet(st, element, start + positions, session);
					positions += singleParameterColumnSpan;
				}
			}

			public string ToLoggableString(object value, ISessionFactoryImplementor factory)
			{
				throw new InvalidOperationException();
			}

			public object DeepCopy(object val, EntityMode entityMode, ISessionFactoryImplementor factory)
			{
				throw new InvalidOperationException();
			}

			public object Hydrate(IDataReader rs, string[] names, ISessionImplementor session, object owner)
			{
				throw new InvalidOperationException();
			}

			public object ResolveIdentifier(object value, ISessionImplementor session, object owner)
			{
				throw new InvalidOperationException();
			}

			public object SemiResolve(object value, ISessionImplementor session, object owner)
			{
				throw new InvalidOperationException();
			}

			public object Replace(object original, object target, ISessionImplementor session, object owner, IDictionary copiedAlready)
			{
				throw new InvalidOperationException();
			}

			public object Replace(object original, object target, ISessionImplementor session, object owner, IDictionary copyCache, ForeignKeyDirection foreignKeyDirection)
			{
				throw new InvalidOperationException();
			}

			public bool IsSame(object x, object y, EntityMode entityMode)
			{
				return false;
			}

			public bool IsEqual(object x, object y, EntityMode entityMode)
			{
				return false;
			}

			public bool IsEqual(object x, object y, EntityMode entityMode, ISessionFactoryImplementor factory)
			{
				return false;
			}

			public int GetHashCode(object x, EntityMode entityMode)
			{
				return GetHashCode();
			}

			public int GetHashCode(object x, EntityMode entityMode, ISessionFactoryImplementor factory)
			{
				return GetHashCode();
			}

			public int Compare(object x, object y, EntityMode? entityMode)
			{
				return 1;
			}

			public IType GetSemiResolvedType(ISessionFactoryImplementor factory)
			{
				throw new InvalidOperationException();
			}

			public void SetToXMLNode(XmlNode node, object value, ISessionFactoryImplementor factory)
			{
				throw new InvalidOperationException();
			}

			public object FromXMLNode(XmlNode xml, IMapping factory)
			{
				throw new InvalidOperationException();
			}

			public bool[] ToColumnNullness(object value, IMapping mapping)
			{
				throw new InvalidOperationException();
			}
		}

		public bool Equals(DynamicFilterParameterSpecification other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.filterParameterFullName, filterParameterFullName);
		}

		public override int GetHashCode()
		{
			return filterParameterFullName.GetHashCode() ^ 53;
		}
	}
}