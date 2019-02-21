using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Transform
{
	/// <summary>
	/// Result transformer that allows to transform a result to
	/// a user specified class which will be populated via setter
	/// methods or fields matching the alias names.
	/// "Compiled" version of AliasToBean transformer. Performs better if you have many aliases and/or load many records.
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
	/// 			.SetResultTransformer(Transformers.AliasToBeanCompiled(typeof(StudentDTO))
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
	public sealed class AliasToBeanCompiledResultTransformer : AliasToBeanResultTransformer
	{
		[NonSerialized]
		Func<object[], object> _objectIniter;

		public AliasToBeanCompiledResultTransformer(System.Type resultClass) : base(resultClass)
		{
		}

		public override object TransformTuple(object[] tuple, string[] aliases)
		{
			var initer = _objectIniter ?? (_objectIniter = CompileObjectIniter(aliases));
			return initer(tuple);
		}

		private Func<object[], object> CompileObjectIniter(string[] aliases)
		{
			if (aliases == null)
			{
				throw new ArgumentNullException("aliases");
			}

			var bindings = new MemberAssignment[aliases.Length];
			var tupleParam = Expression.Parameter(typeof(object[]), "tuple");
			for (int i = 0; i < aliases.Length; i++)
			{
				string alias = aliases[i];
				if (string.IsNullOrEmpty(alias))
					continue;

				var memberInfo = GetMemberInfo(alias);
				var valueExpr = Expression.ArrayAccess(tupleParam, Expression.Constant(i));
				bindings[i] = Expression.Bind(memberInfo, GetTyped(memberInfo, valueExpr));
			}

			Expression initExpr = Expression.MemberInit(GetNewExpression(ResultClass), bindings);
			if (!ResultClass.IsClass)
				initExpr = Expression.Convert(initExpr, typeof(object));

			return (Func<object[], object>) Expression.Lambda(initExpr, tupleParam).Compile();
		}

		private static Expression GetTyped(MemberInfo memberInfo, Expression expr)
		{
			var type = GetMemberType(memberInfo);
			if (type == typeof(object))
				return expr;
			return Expression.Convert(expr, type);
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
					"The target class of a AliasToBeanCompiledResultTransformer need a parameter-less constructor",
					nameof(resultClass));
			}

			return Expression.New(beanConstructor);
		}
	}
}
