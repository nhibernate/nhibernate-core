using System;
using System.Reflection;
using NHibernate.Engine;

namespace NHibernate.Linq.Functions
{
	public interface IAllowPreEvaluationHqlGenerator
	{
		/// <summary>
		/// Should pre-evaluation be allowed for this property or method?
		/// </summary>
		/// <param name="member">The property or method.</param>
		/// <param name="factory">The session factory.</param>
		/// <returns>
		/// <see langword="true" /> if the property or method should be evaluated before running the query whenever possible,
		/// <see langword="false" /> if it must always be translated to the equivalent HQL call.
		/// </returns>
		/// <remarks>Implementors should return <see langword="true" /> by default. Returning <see langword="false" />
		/// is mainly useful when the HQL translation is a non-deterministic function call like <c>NEWGUID()</c> or
		/// a function which value on server side can differ from the equivalent client value, like
		/// <see cref="DateTime.Now"/>.</remarks>
		bool AllowPreEvaluation(MemberInfo member, ISessionFactoryImplementor factory);

		/// <summary>
		/// Should the instance holding the property or method be ignored?
		/// </summary>
		/// <param name="member">The property or method.</param>
		/// <returns>
		/// <see langword="true" /> if the property or method translation does not depend on the instance to which it
		/// belongs, <see langword="false" /> otherwise.
		/// </returns>
		bool IgnoreInstance(MemberInfo member);
	}
}
