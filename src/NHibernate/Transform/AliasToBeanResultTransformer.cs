using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NHibernate.Util;

namespace NHibernate.Transform
{
	/// <summary>
	/// Result transformer that allows to transform a result to
	/// a user specified class which will be populated via setter
	/// methods or fields matching the alias names.
	/// </summary>
	/// <example>
	/// <code>
	/// IList resultWithAliasedBean = s.CreateCriteria(typeof(Enrollment))
	/// 			.CreateAlias("Student", "st")
	/// 			.CreateAlias("Course", "co")
	/// 			.SetProjection( Projections.ProjectionList()
	/// 					.Add( Projections.Property("co.Description"), "CourseDescription")
	/// 			)
	/// 			.SetResultTransformer( new AliasToBeanResultTransformer(typeof(StudentDTO)))
	/// 			.List();
	/// 
	/// StudentDTO dto = (StudentDTO)resultWithAliasedBean[0];
	/// </code>
	/// </example>
	/// <remarks>
	/// Resolves setter for an alias with a heuristic: search among properties then fields for matching name and case, then,
	/// if no matching property or field was found, retry with a case insensitive match. For members having the same name, it
	/// sorts them by inheritance depth then by visibility from public to private, and takes those ranking first.
	/// </remarks>
	[Serializable]
	public class AliasToBeanResultTransformer : AliasedTupleSubsetResultTransformer, IEquatable<AliasToBeanResultTransformer>
	{
		[NonSerialized]
		private System.Type _resultClass;
		private SerializableSystemType _serializableResultClass;
		[NonSerialized]
		private ConstructorInfo _beanConstructor;
		[NonSerialized]
		private Dictionary<string, NamedMember<FieldInfo>> _fieldsByNameCaseSensitive;
		[NonSerialized]
		private Dictionary<string, NamedMember<FieldInfo>> _fieldsByNameCaseInsensitive;
		[NonSerialized]
		private Dictionary<string, NamedMember<PropertyInfo>> _propertiesByNameCaseSensitive;
		[NonSerialized]
		private Dictionary<string, NamedMember<PropertyInfo>> _propertiesByNameCaseInsensitive;

		public AliasToBeanResultTransformer(System.Type resultClass)
		{
			_resultClass = resultClass ?? throw new ArgumentNullException(nameof(resultClass));

			InitializeTransformer();
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			_serializableResultClass = SerializableSystemType.Wrap(_resultClass);
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			_resultClass = _serializableResultClass?.GetSystemType();
			InitializeTransformer();
		}

		private void InitializeTransformer()
		{
			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			_beanConstructor = _resultClass.GetConstructor(bindingFlags, null, System.Type.EmptyTypes, null);

			// if resultClass is a ValueType (struct), GetConstructor will return null... 
			// in that case, we'll use Activator.CreateInstance instead of the ConstructorInfo to create instances
			if (_beanConstructor == null && _resultClass.IsClass)
			{
				throw new InvalidOperationException(
					"The target class of a AliasToBeanResultTransformer need a parameter-less constructor");
			}

			var fields = new List<RankedMember<FieldInfo>>();
			var properties = new List<RankedMember<PropertyInfo>>();
			FetchFieldsAndProperties(_resultClass, fields, properties);

			_fieldsByNameCaseSensitive = GetMapByName(fields, StringComparer.Ordinal);
			_fieldsByNameCaseInsensitive = GetMapByName(fields, StringComparer.OrdinalIgnoreCase);
			_propertiesByNameCaseSensitive = GetMapByName(properties, StringComparer.Ordinal);
			_propertiesByNameCaseInsensitive = GetMapByName(properties, StringComparer.OrdinalIgnoreCase);
		}

		public override bool IsTransformedValueATupleElement(String[] aliases, int tupleLength)
		{
			return false;
		}

		public override object TransformTuple(object[] tuple, String[] aliases)
		{
			if (aliases == null)
			{
				throw new ArgumentNullException(nameof(aliases));
			}
			object result;

			try
			{
				result = _resultClass.IsClass
							? _beanConstructor.Invoke(null)
							: Cfg.Environment.BytecodeProvider.ObjectsFactory.CreateInstance(_resultClass, true);

				for (int i = 0; i < aliases.Length; i++)
				{
					SetProperty(aliases[i], tuple[i], result);
				}
			}
			catch (InstantiationException e)
			{
				throw new HibernateException("Could not instantiate result class: " + _resultClass.FullName, e);
			}
			catch (MethodAccessException e)
			{
				throw new HibernateException("Could not instantiate result class: " + _resultClass.FullName, e);
			}

			return result;
		}

		public override IList TransformList(IList collection)
		{
			return collection;
		}

		#region Setter resolution

		/// <summary>
		/// Set the value of a property or field matching an alias.
		/// </summary>
		/// <param name="alias">The alias for which resolving the property or field.</param>
		/// <param name="value">The value to which the property or field should be set.</param>
		/// <param name="resultObj">The object on which to set the property or field. It must be of the type for which
		/// this instance has been built.</param>
		/// <exception cref="PropertyNotFoundException">Thrown if no matching property or field can be found.</exception>
		/// <exception cref="AmbiguousMatchException">Thrown if many matching properties or fields are found, having the
		/// same visibility and inheritance depth.</exception>
		private void SetProperty(string alias, object value, object resultObj)
		{
			if (alias == null)
				// Grouping properties in criteria are selected without alias, just ignore them.
				return;

			if (TrySet(alias, value, resultObj, _propertiesByNameCaseSensitive))
				return;
			if (TrySet(alias, value, resultObj, _fieldsByNameCaseSensitive))
				return;
			if (TrySet(alias, value, resultObj, _propertiesByNameCaseInsensitive))
				return;
			if (TrySet(alias, value, resultObj, _fieldsByNameCaseInsensitive))
				return;

			throw new PropertyNotFoundException(resultObj.GetType(), alias, "setter");
		}

		private static bool TrySet(string alias, object value, object resultObj, Dictionary<string, NamedMember<FieldInfo>> fieldsMap)
		{
			if (fieldsMap.TryGetValue(alias, out var field))
			{
				CheckMember(field, alias);
				field.Member.SetValue(resultObj, value);
				return true;
			}
			return false;
		}

		private static bool TrySet(string alias, object value, object resultObj, Dictionary<string, NamedMember<PropertyInfo>> propertiesMap)
		{
			if (propertiesMap.TryGetValue(alias, out var property))
			{
				CheckMember(property, alias);
				property.Member.SetValue(resultObj, value);
				return true;
			}
			return false;
		}

		private static void CheckMember<T>(NamedMember<T> member, string alias) where T : MemberInfo
		{
			if (member.Member != null)
				return;

			if (member.AmbiguousMembers == null || member.AmbiguousMembers.Length < 2)
			{
				// Should never happen, check NamedMember instanciations.
				throw new InvalidOperationException(
					$"{nameof(NamedMember<T>.Member)} missing and {nameof(NamedMember<T>.AmbiguousMembers)} invalid.");
			}

			throw new AmbiguousMatchException(
				$"Unable to find adequate property or field to set on '{member.AmbiguousMembers[0].DeclaringType.Name}' for alias '{alias}', " +
					$"many {(member.AmbiguousMembers[0] is PropertyInfo ? "properties" : "fields")} matches: " +
					$"{string.Join(", ", member.AmbiguousMembers.Select(m => m.Name))}");
		}

		private static void FetchFieldsAndProperties(System.Type currentType, List<RankedMember<FieldInfo>> fields, List<RankedMember<PropertyInfo>> properties)
		{
			const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
			var rank = 1;
			// For grasping private members, we need to manually walk the hierarchy.
			while (currentType != null && currentType != typeof(object))
			{
				fields.AddRange(
					currentType
						.GetFields(bindingFlags)
						.Select(f => new RankedMember<FieldInfo> { Member = f, VisibilityRank = GetFieldVisibilityRank(f), HierarchyRank = rank }));
				properties.AddRange(
					currentType
						.GetProperties(bindingFlags)
						.Where(p => p.CanWrite && p.GetIndexParameters().Length == 0)
						.Select(p => new RankedMember<PropertyInfo> { Member = p, VisibilityRank = GetPropertyVisibilityRank(p), HierarchyRank = rank }));
				currentType = currentType.BaseType;
				rank++;
			}
		}

		private static int GetFieldVisibilityRank(FieldInfo field)
		{
			if (field.IsPublic)
				return 1;
			if (field.IsFamilyOrAssembly)
				return 2;
			if (field.IsFamily)
				return 3;
			if (field.IsPrivate)
				return 4;
			return 5;
		}

		private static int GetPropertyVisibilityRank(PropertyInfo property)
		{
			var setter = property.SetMethod;
			if (setter.IsPublic)
				return 1;
			if (setter.IsFamilyOrAssembly)
				return 2;
			if (setter.IsFamily)
				return 3;
			if (setter.IsPrivate)
				return 4;
			return 5;
		}

		private static Dictionary<string, NamedMember<T>> GetMapByName<T>(IEnumerable<RankedMember<T>> members, StringComparer comparer) where T : MemberInfo
		{
			return members
				.GroupBy(m => m.Member.Name,
					(k, g) =>
						new NamedMember<T>(k,
							g
								.GroupBy(m => new { m.HierarchyRank, m.VisibilityRank })
								.OrderBy(subg => subg.Key.HierarchyRank).ThenBy(subg => subg.Key.VisibilityRank)
								.First()
								.Select(m => m.Member)
								.ToArray()),
					comparer)
				.ToDictionary(f => f.Name, comparer);
		}

		private struct RankedMember<T> where T : MemberInfo
		{
			public T Member;
			public int HierarchyRank;
			public int VisibilityRank;
		}

		private struct NamedMember<T> where T : MemberInfo
		{
			public NamedMember(string name, T[] members)
			{
				if (members == null)
					throw new ArgumentNullException(nameof(members));
				Name = name;
				if (members.Length == 1)
				{
					Member = members[0];
					AmbiguousMembers = null;
				}
				else
				{
					Member = null;
					AmbiguousMembers = members;
				}
			}

			public string Name;
			public T Member;
			public T[] AmbiguousMembers;
		}

		#endregion

		#region Equality & hash-code

		public override bool Equals(object obj)
		{
			return Equals(obj as AliasToBeanResultTransformer);
		}

		public bool Equals(AliasToBeanResultTransformer other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other._resultClass, _resultClass);
		}

		public override int GetHashCode()
		{
			return _resultClass.GetHashCode();
		}

		#endregion
	}
}
