using System.Runtime.Serialization;

namespace NHibernate.Util
{
	/// <summary>
	/// Secure-by-default serialization strategy that requires applications to opt in explicitly.
	/// </summary>
	public sealed class ThrowingSerializationStrategy : ISerializationStrategy
	{
		private static readonly string Message =
			$"Serialization is not configured. Configure NHibernate.Util.{nameof(SerializationConfiguration)}.{nameof(SerializationConfiguration.Strategy)} " +
			$"or set NHibernate property {Cfg.Environment.SerializationStrategy}.";

		/// <summary>
		/// Always throws because no serialization strategy is configured.
		/// </summary>
		/// <param name="value">Ignored.</param>
		/// <returns>Never returns.</returns>
		public byte[] Serialize(object value)
		{
			throw new SerializationException(Message);
		}

		/// <summary>
		/// Always throws because no serialization strategy is configured.
		/// </summary>
		/// <param name="data">Ignored.</param>
		/// <returns>Never returns.</returns>
		public object Deserialize(byte[] data)
		{
			throw new SerializationException(Message);
		}
	}
}
