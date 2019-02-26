using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using NHibernate.Util;

namespace NHibernate.Transform
{
	/// <summary>
	/// Result transformer that allows to transform a result to
	/// a user specified class which will be populated via setter
	/// methods or fields matching the alias names.
	/// NOTE: This transformer can't be reused by different queries as it caches query aliases on first transformation
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
		private readonly System.Type _resultClass;
		private readonly Dictionary<string, NamedMember<FieldInfo>> _fieldsByNameCaseSensitive;
		private readonly Dictionary<string, NamedMember<FieldInfo>> _fieldsByNameCaseInsensitive;
		private readonly Dictionary<string, NamedMember<PropertyInfo>> _propertiesByNameCaseSensitive;
		private readonly Dictionary<string, NamedMember<PropertyInfo>> _propertiesByNameCaseInsensitive;

		[NonSerialized]
		Func<object[], object> _objectIniter;

		public System.Type ResultClass => _resultClass;

		public AliasToBeanResultTransformer(System.Type resultClass)
		{
			_resultClass = resultClass ?? throw new ArgumentNullException("resultClass");

			var fields = new List<RankedMember<FieldInfo>>();
			var properties = new List<RankedMember<PropertyInfo>>();
			FetchFieldsAndProperties(resultClass, fields, properties);

			_fieldsByNameCaseSensitive = GetMapByName(fields, StringComparer.Ordinal);
			_fieldsByNameCaseInsensitive = GetMapByName(fields, StringComparer.OrdinalIgnoreCase);
			_propertiesByNameCaseSensitive = GetMapByName(properties, StringComparer.Ordinal);
			_propertiesByNameCaseInsensitive = GetMapByName(properties, StringComparer.OrdinalIgnoreCase);
		}

		public override bool IsTransformedValueATupleElement(String[] aliases, int tupleLength)
		{
			return false;
		}

		public override object TransformTuple(object[] tuple, string[] aliases)
		{
			var initer = _objectIniter ?? (_objectIniter = CompileObjectIniter(aliases));
			return initer(tuple);
		}

		public override IList TransformList(IList collection)
		{
			return collection;
		}

		#region Setter resolution

		protected MemberInfo GetMemberInfo(string alias)
		{
			return TryGetMemberInfo(alias, _propertiesByNameCaseSensitive)
				?? TryGetMemberInfo(alias, _fieldsByNameCaseSensitive)
				?? TryGetMemberInfo(alias, _propertiesByNameCaseInsensitive)
				?? (MemberInfo) TryGetMemberInfo(alias, _fieldsByNameCaseInsensitive)
				?? throw new PropertyNotFoundException(_resultClass, alias, "setter");
		}

		private TMemberInfo TryGetMemberInfo<TMemberInfo>(string alias, params Dictionary<string, NamedMember<TMemberInfo>>[] propertiesMaps) where TMemberInfo: MemberInfo
		{
			foreach (var propertiesMap in propertiesMaps)
			{
				if (propertiesMap.TryGetValue(alias, out var property))
				{
					CheckMember(property, alias);
					return property.Member;
				}
			}
			return null;
		}

		private void CheckMember<T>(NamedMember<T> member, string alias) where T : MemberInfo
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

		private Func<object[], object> CompileObjectIniter(string[] aliases)
		{
			if (aliases == null)
			{
				throw new ArgumentNullException("aliases");
			}

			Expression<Func<string, object, Exception>> getException = (name, obj) => new InvalidCastException($"Failed to set property '{name}' with value of type '{obj.GetType()}'");

			var bindings = new List<MemberAssignment>(aliases.Length);
			var tupleParam = Expression.Parameter(typeof(object[]), "tuple");
			for (int i = 0; i < aliases.Length; i++)
			{
				string alias = aliases[i];
				if (string.IsNullOrEmpty(alias))
					continue;

				var memberInfo = GetMemberInfo(alias);
				var valueExpr = Expression.ArrayAccess(tupleParam, Expression.Constant(i));
				bindings.Add(Expression.Bind(memberInfo, GetTyped(memberInfo, valueExpr, getException)));
			}

			Expression initExpr = Expression.MemberInit(GetNewExpression(ResultClass), bindings);
			if (!ResultClass.IsClass)
				initExpr = Expression.Convert(initExpr, typeof(object));

			return (Func<object[], object>) Expression.Lambda(initExpr, tupleParam).Compile();
		}

		private static Expression GetTyped(MemberInfo memberInfo, Expression expr, Expression<Func<string, object, Exception>> getEx)
		{
			var type = GetMemberType(memberInfo);
			if (type == typeof(object))
				return expr;

			var originalValue = expr;
			if (!type.IsClass)
			{
				expr = Expression.Coalesce(expr, Expression.Default(type));
			}
			expr = Expression.Convert(expr, type);

			var tryCatch =
				Expression.TryCatch(
					expr,
					Expression.Catch(
						typeof(InvalidCastException),
						Expression.Throw(Expression.Invoke(getEx, Expression.Constant(memberInfo.Name), originalValue), type)
					));
			if (type.IsClass)
				return expr;
			return Expression.Block(type, tryCatch);
		}

		private static System.Type GetMemberType(MemberInfo memberInfo)
		{
			if (memberInfo is PropertyInfo prop)
				return prop.PropertyType;

			if (memberInfo is FieldInfo field)
				return field.FieldType;

			throw new NotSupportedException($"Member type {memberInfo} is not supported");
		}

		private static NewExpression GetNewExpression(System.Type resultClass)
		{
			if (!resultClass.IsClass)
				return Expression.New(resultClass);

			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var beanConstructor = resultClass.GetConstructor(bindingFlags, null, System.Type.EmptyTypes, null);

			if (beanConstructor == null)
			{
				throw new ArgumentException(
					"The target class of a AliasToBeanResultTransformer need a parameter-less constructor",
					nameof(resultClass));
			}

			return Expression.New(beanConstructor);
		}

		private static void FetchFieldsAndProperties(System.Type resultClass, List<RankedMember<FieldInfo>> fields, List<RankedMember<PropertyInfo>> properties)
		{
			const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
			var currentType = resultClass;
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

		[Serializable]
		private struct NamedMember<T> : ISerializable
			where T : MemberInfo
		{
			public string Name;
			public T Member;
			public T[] AmbiguousMembers;

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

			private NamedMember(SerializationInfo info, StreamingContext context)
			{
				Name = info.GetString("Name");
				Member = info.GetValue<T>("Member");
				AmbiguousMembers = info.GetValueArray<T>("AmbiguousMembers");
			}

			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue("Name", Name);
				info.AddValue("Member", Member);
				info.AddValueArray("AmbiguousMembers", AmbiguousMembers);
			}
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
			if (GetType() != other.GetType())
				return false;
			return Equals(other._resultClass, _resultClass);
		}

		public override int GetHashCode()
		{
			return _resultClass.GetHashCode();
		}

		#endregion
	}
}
