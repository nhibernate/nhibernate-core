using System;
using System.Collections;

namespace NHibernate.Expression
{
	// TODO: find where this is used and test/fix it up
	/// <summary>
	/// Represents an strategy for matching strings using "like".
	/// </summary>
	public abstract class MatchMode
	{
		private int _intCode;
		private string _name;
		private static Hashtable Instances = new Hashtable();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="intCode"></param>
		/// <param name="name"></param>
		protected MatchMode(int intCode, string name)
		{
			this._intCode = intCode;
			this._name = name;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _name;
		}

		/// <summary>
		/// convert the pattern, by appending/prepending "%"
		/// </summary>
		public abstract string ToMatchString(string pattern);

		// TODO: need to fix up so serialization/deserialization 
		// preserves the singleton
		//		private Object ReadResolve()
		//		{
		//			return INSTANCES[intCode];
		//		}

		static MatchMode()
		{
			Instances.Add( Exact._intCode, Exact );
			Instances.Add( Start._intCode, Start );
			Instances.Add( End._intCode, End );
			Instances.Add( Anywhere._intCode, Anywhere );
		}

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

		private class ExactMatchMode : MatchMode
		{
			public ExactMatchMode() : base( 0, "EXACT" )
			{
			}

			public override string ToMatchString(string pattern)
			{
				return pattern;
			}
		}

		private class StartMatchMode : MatchMode
		{
			public StartMatchMode() : base( 1, "START" )
			{
			}

			public override string ToMatchString(string pattern)
			{
				return '%' + pattern;
			}
		}

		private class EndMatchMode : MatchMode
		{
			public EndMatchMode() : base( 2, "END" )
			{
			}

			public override string ToMatchString(string pattern)
			{
				return pattern + '%';
			}

		}

		private class AnywhereMatchMode : MatchMode
		{
			public AnywhereMatchMode() : base( 3, "ANYWHERE" )
			{
			}

			public override string ToMatchString(string pattern)
			{
				return '%' + pattern + '%';
			}

		}

	}
}