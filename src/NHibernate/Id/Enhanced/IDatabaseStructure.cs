using NHibernate.Engine;

namespace NHibernate.Id.Enhanced
{
	/// <summary> 
	/// Encapsulates definition of the underlying data structure backing a sequence-style generator. 
	/// </summary>
	public interface IDatabaseStructure
	{
		/// <summary> The name of the database structure (table or sequence).</summary>
		string Name { get; }

		/// <summary> How many times has this structure been accessed through this reference?</summary>
		int TimesAccessed { get; }

		/// <summary> The configured increment size</summary>
		int IncrementSize { get; }

		/// <summary> 
		/// A callback to be able to get the next value from the underlying
		/// structure as needed.
		///  </summary>
		/// <param name="session">The session. </param>
		/// <returns> The next value. </returns>
		IAccessCallback BuildCallback(ISessionImplementor session);

		/// <summary> 
		/// Prepare this structure for use.  Called sometime after instantiation,
		/// but before first use. 
		/// </summary>
		/// <param name="optimizer">The optimizer being applied to the generator. </param>
		void Prepare(IOptimizer optimizer);

		/// <summary> Commands needed to create the underlying structures.</summary>
		/// <param name="dialect">The database dialect being used. </param>
		/// <returns> The creation commands. </returns>
		string[] SqlCreateStrings(Dialect.Dialect dialect);

		/// <summary> Commands needed to drop the underlying structures.</summary>
		/// <param name="dialect">The database dialect being used. </param>
		/// <returns> The drop commands. </returns>
		string[] SqlDropStrings(Dialect.Dialect dialect);
	}
}