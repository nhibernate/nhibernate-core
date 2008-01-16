using System;

namespace NHibernate.Shards.Util
{
	/// <summary>
	/// Helper methods for checking preconditions.
	/// </summary>
	public class Preconditions
	{
		/// <summary>
		/// TODO: doc
		/// </summary>
		/// <param name="expression"></param>
		public static void CheckArgument(bool expression)
		{
			if (!expression)
			{
				throw new ArgumentException();
			}
		}

		/// <summary>
		/// TODO: documentation
		/// </summary>
		/// <param name="expression"></param>
		public static void CheckState(bool expression)
		{
			if (!expression) 
			{
				//TODO: change this exception by another more apropiated
				throw new Exception("Invalid State Exception");
			}
		}

		/// <summary>
		/// TODO: doc
		/// </summary>
		/// <param name="reference"></param>
		public static void CheckNotNull(object reference)
		{
			if (reference == null)
			{
				throw new ArgumentNullException();
			}
		}
	}
}