using System;
using System.Collections;

namespace NHibernate.Criterion
{
	/// <summary>
	/// Represents an strategy for matching strings using "like".
	/// </summary>
	public abstract class MatchMode
	{
		private int _intCode;
		private string _name;
		private static Hashtable Instances = new Hashtable();

		static MatchMode()
		{
			Instances.Add(Exact._intCode, Exact);
			Instances.Add(Start._intCode, Start);
			Instances.Add(End._intCode, End);
			Instances.Add(Anywhere._intCode, Anywhere);
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="MatchMode" /> class.
		/// </summary>
		/// <param name="intCode">The code that identifies the match mode.</param>
		/// <param name="name">The friendly name of the match mode.</param>
		/// <remarks>
		/// The parameter <c>intCode</c> is used as the key of <see cref="IDictionary"/>
		/// to store instances and to ensure only instance of a particular <see cref="MatchMode"/>
		/// is created.
		/// </remarks>
		protected MatchMode(int intCode, string name)
		{
			_intCode = intCode;
			_name = name;
		}

		#region System.Object Members

		/// <summary>
		/// The string representation of the <see cref="MatchMode"/>.
		/// </summary>
		/// <returns>The friendly name used to describe the <see cref="MatchMode"/>.</returns>
		public override string ToString()
		{
			return _name;
		}

		#endregion

		/// <summary>
		/// Convert the pattern, by appending/prepending "%"
		/// </summary>
		/// <param name="pattern">The string to convert to the appropriate match pattern.</param>
		/// <returns>
		/// A <see cref="String"/> that contains a "%" in the appropriate place
		/// for the Match Strategy.
		/// </returns>
		public abstract string ToMatchString(string pattern);

		// TODO: need to fix up so serialization/deserialization 
		// preserves the singleton
		//		private Object ReadResolve()
		//		{
		//			return INSTANCES[intCode];
		//		}

		/// <summary>
		/// Match the entire string to the pattern
		/// </summary>
		public static readonly MatchMode Exact = new ExactMatchMode();

		/// <summary>
		/// Match the start of the string to the pattern
		/// </summary>
		public static readonly MatchMode Start = new StartMatchMode();

		/// <summary>
		/// Match the end of the string to the pattern
		/// </summary>
		public static readonly MatchMode End = new EndMatchMode();

		/// <summary>
		/// Match the pattern anywhere in the string
		/// </summary>
		public static readonly MatchMode Anywhere = new AnywhereMatchMode();

		/// <summary>
		/// The <see cref="MatchMode"/> that matches the entire string to the pattern.
		/// </summary>
		private class ExactMatchMode : MatchMode
		{
			/// <summary>
			/// Initialize a new instance of the <see cref="ExactMatchMode" /> class.
			/// </summary>
			public ExactMatchMode() : base(0, "EXACT")
			{
			}

			/// <summary>
			/// Converts the string to the Exact MatchMode.
			/// </summary>
			/// <param name="pattern">The string to convert to the appropriate match pattern.</param>
			/// <returns>The <c>pattern</c> exactly the same as it was passed in.</returns>
			public override string ToMatchString(string pattern)
			{
				return pattern;
			}
		}

		/// <summary>
		/// The <see cref="MatchMode"/> that matches the start of the string to the pattern.
		/// </summary>
		private class StartMatchMode : MatchMode
		{
			/// <summary>
			/// Initialize a new instance of the <see cref="StartMatchMode" /> class.
			/// </summary>
			public StartMatchMode() : base(1, "START")
			{
			}

			/// <summary>
			/// Converts the string to the Start MatchMode.
			/// </summary>
			/// <param name="pattern">The string to convert to the appropriate match pattern.</param>
			/// <returns>The <c>pattern</c> with a "<c>%</c>" appended at the end.</returns>
			public override string ToMatchString(string pattern)
			{
				return pattern + '%';
			}
		}

		/// <summary>
		/// The <see cref="MatchMode"/> that matches the end of the string to the pattern.
		/// </summary>
		private class EndMatchMode : MatchMode
		{
			/// <summary>
			/// Initialize a new instance of the <see cref="EndMatchMode" /> class.
			/// </summary>
			public EndMatchMode() : base(2, "END")
			{
			}

			/// <summary>
			/// Converts the string to the End MatchMode.
			/// </summary>
			/// <param name="pattern">The string to convert to the appropriate match pattern.</param>
			/// <returns>The <c>pattern</c> with a "<c>%</c>" appended at the beginning.</returns>
			public override string ToMatchString(string pattern)
			{
				return '%' + pattern;
			}
		}

		/// <summary>
		/// The <see cref="MatchMode"/> that exactly matches the string
		/// by appending "<c>%</c>" to the beginning and end.
		/// </summary>
		private class AnywhereMatchMode : MatchMode
		{
			/// <summary>
			/// Initialize a new instance of the <see cref="AnywhereMatchMode" /> class.
			/// </summary>
			public AnywhereMatchMode() : base(3, "ANYWHERE")
			{
			}

			/// <summary>
			/// Converts the string to the Exact MatchMode.
			/// </summary>
			/// <param name="pattern">The string to convert to the appropriate match pattern.</param>
			/// <returns>The <c>pattern</c> with a "<c>%</c>" appended at the beginning and the end.</returns>
			public override string ToMatchString(string pattern)
			{
				return '%' + pattern + '%';
			}
		}
	}
}