namespace NHibernate.Mapping.ByCode
{
	public interface IColumnOrFormulaMapper : IColumnMapper
	{
		/// <summary>
		/// Maps a formula.
		/// </summary>
		/// <param name="formula">The formula to map.</param>
		/// <remarks>Replaces any previously mapped column attribute.</remarks>
		void Formula(string formula);
	}
}
