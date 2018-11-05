using System.Data.Common;
using NHibernate.Driver;
using NHibernate.SqlTypes;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// Supports adjusting a <see cref="DbParameter"/> according to a <see cref="SqlType"/> and
	/// the parameter's value. An <see cref="IDriver"/> may implement this interface.
	/// </summary>
	public interface IParameterAdjuster
	{
		/// <summary>
		/// Adjust the provided parameter according to its <paramref name="sqlType"/> and
		/// <paramref name="value"/>.
		/// </summary>
		/// <param name="parameter">The parameter to adjust.</param>
		/// <param name="sqlType">The parameter's <see cref="SqlType"/>.</param>
		/// <param name="value">The parameter's value.</param>
		void AdjustParameterForValue(DbParameter parameter, SqlType sqlType, object value);
	}
}
