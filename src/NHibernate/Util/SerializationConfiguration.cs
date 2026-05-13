using System;

namespace NHibernate.Util
{
	/// <summary>
	/// Global serialization configuration used by NHibernate serialization features.
	/// </summary>
	public static class SerializationConfiguration
	{
		private static ISerializationStrategy _strategy = new ThrowingSerializationStrategy();

		/// <summary>
		/// Gets or sets the active serialization strategy used by NHibernate.
		/// </summary>
		public static ISerializationStrategy Strategy
		{
			get => _strategy;
			set
			{
				if (value == null)
					throw new ArgumentNullException(nameof(value));

				_strategy = value;
			}
		}
	}
}
