using System;
using System.Collections;

namespace NHibernate.Expression
{
	/// <summary>
	/// Represents an strategy for matching strings using "like".
	/// </summary>
	public abstract class MatchMode
	{
		private int intCode;
		private string name;
		private static Hashtable INSTANCES = new Hashtable();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="intCode"></param>
		/// <param name="name"></param>
		protected MatchMode(int intCode, string name)
		{
			this.intCode = intCode;
			this.name = name;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return name;
		}

		/// <summary>
		/// convert the pattern, by appending/prepending "%"
		/// </summary>
		public abstract string ToMatchString(string pattern);

		private Object ReadResolve()
		{
			return INSTANCES[intCode];
		}

		static MatchMode()
		{
			INSTANCES.Add(EXACT.intCode, EXACT);
			INSTANCES.Add(START.intCode, START);
			INSTANCES.Add(END.intCode, END);
			INSTANCES.Add(ANYWHERE.intCode, ANYWHERE);
		}

		/// <summary>
		/// Match the entire string to the pattern
		/// </summary>
		public static readonly MatchMode EXACT = new ExactMatchMode();
		/// <summary>
		/// Match the start of the string to the pattern
		/// </summary>
		public static readonly MatchMode START = new StartMatchMode();
		/// <summary>
		/// Match the end of the string to the pattern
		/// </summary>
		public static readonly MatchMode END = new EndMatchMode();
		/// <summary>
		/// Match the pattern anywhere in the string
		/// </summary>
		public static readonly MatchMode ANYWHERE = new AnywhereMatchMode();

		private class ExactMatchMode : MatchMode
		{
			public ExactMatchMode():base(0, "EXACT")
			{
				
			}

			public override string ToMatchString(string pattern)
			{
				return pattern;
			}
		}

		private class StartMatchMode : MatchMode
		{
			public StartMatchMode() : base(1, "START")
			{
				
			}		

			public override string ToMatchString(string pattern)
			{
				return '%' + pattern;
			}
		}

		private class EndMatchMode : MatchMode
		{
			public EndMatchMode() : base(2, "END")
			{
			}

			public override string ToMatchString(string pattern)
			{
				return pattern + '%';
			}

		}

		private class AnywhereMatchMode : MatchMode
		{
			public AnywhereMatchMode() : base(3, "ANYWHERE")
			{		
			}

			public override string ToMatchString(string pattern)
			{
				return '%' + pattern + '%';
			}

		}

	}
}
