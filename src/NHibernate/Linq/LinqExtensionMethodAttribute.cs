using System;

namespace NHibernate.Linq
{
	/// <summary>
	/// Flag a method as being a SQL function call for the linq-to-nhibernate provider. Its
	/// parameters will be used as the function call parameters.
	/// </summary>
	public class LinqExtensionMethodAttribute : LinqExtensionMethodAttributeBase
	{
		/// <summary>
		/// Default constructor. The method call will be translated by the linq provider to
		/// a function call having the same name than the method.
		/// </summary>
		public LinqExtensionMethodAttribute()
			: base(LinqExtensionPreEvaluation.NoEvaluation) { }

		/// <summary>
		/// Constructor specifying a SQL function name.
		/// </summary>
		/// <param name="name">The name of the SQL function.</param>
		public LinqExtensionMethodAttribute(string name)
			: this(name, LinqExtensionPreEvaluation.NoEvaluation) { }

		/// <summary>
		/// Constructor allowing to specify a <see cref="LinqExtensionPreEvaluation"/> for the method.
		/// </summary>
		/// <param name="preEvaluation">Should the method call be pre-evaluated when not depending on
		/// queried data? Default is <see cref="LinqExtensionPreEvaluation.NoEvaluation"/>.</param>
		public LinqExtensionMethodAttribute(LinqExtensionPreEvaluation preEvaluation)
			: base(preEvaluation) { }

		/// <summary>
		/// Constructor for specifying a SQL function name and a <see cref="LinqExtensionPreEvaluation"/>.
		/// </summary>
		/// <param name="name">The name of the SQL function.</param>
		/// <param name="preEvaluation">Should the method call be pre-evaluated when not depending on
		/// queried data? Default is <see cref="LinqExtensionPreEvaluation.NoEvaluation"/>.</param>
		public LinqExtensionMethodAttribute(string name, LinqExtensionPreEvaluation preEvaluation)
			: base(preEvaluation)
		{
			Name = name;
		}

		/// <summary>
		/// The name of the SQL function.
		/// </summary>
		public string Name { get; }
	}

	/// <summary>
	/// Can flag a method as not being callable by the runtime, when used in Linq queries.
	/// If the method is supported by the linq-to-nhibernate provider, it will always be converted
	/// to the corresponding SQL statement.
	/// Otherwise the linq-to-nhibernate provider evaluates method calls when they do not depend on
	/// the queried data.
	/// </summary>
	public class NoPreEvaluationAttribute : LinqExtensionMethodAttributeBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public NoPreEvaluationAttribute()
			: base(LinqExtensionPreEvaluation.NoEvaluation) { }
	}

	/// <summary>
	/// Base class for Linq extension attributes.
	/// </summary>
	public abstract class LinqExtensionMethodAttributeBase : Attribute
	{
		/// <summary>
		/// Should the method call be pre-evaluated when not depending on queried data? If it can,
		/// it would then be evaluated and replaced by the resulting (parameterized) constant expression 
		/// in the resulting SQL query.
		/// </summary>
		public LinqExtensionPreEvaluation PreEvaluation { get; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="preEvaluation">Should the method call be pre-evaluated when not depending on queried data?</param>
		protected LinqExtensionMethodAttributeBase(LinqExtensionPreEvaluation preEvaluation)
		{
			PreEvaluation = preEvaluation;
		}
	}

	/// <summary>
	/// Possible method call behaviors when the linq to NHibernate provider pre-evaluates
	/// expressions before translating them to SQL.
	/// </summary>
	public enum LinqExtensionPreEvaluation
	{
		/// <summary>
		/// The method call will not be evaluated even if its arguments do not depend on queried data.
		/// It will always be translated to the corresponding SQL statement.
		/// </summary>
		NoEvaluation,
		/// <summary>
		/// If the method call does not depend on queried data, the method call will be evaluated and replaced
		/// by the resulting (parameterized) constant expression in the resulting SQL query. A throwing
		/// method implementation will cause the query to throw.
		/// </summary>
		AllowPreEvaluation
	}
}
