using System;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode
{
	/// <summary>
	/// A mapper for mapping mixed list of columns and formulas.
	/// </summary>
	public interface IColumnsAndFormulasMapper : IColumnsMapper
	{
		/// <summary>
		/// Maps a mixed list of columns and formulas.
		/// </summary>
		/// <param name="columnOrFormulaMapper">The mappers for each column or formula.</param>
		/// <remarks>Replaces any previously mapped column or formula.</remarks>
		void ColumnsAndFormulas(params Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper);

		/// <summary>
		/// Maps a formula.
		/// </summary>
		/// <param name="formula">The formula to map.</param>
		/// <remarks>Replaces any previously mapped column or formula, unless <see langword="null"/>.</remarks>
		void Formula(string formula);

		/// <summary>
		/// Maps many formulas.
		/// </summary>
		/// <param name="formulas">The formulas to map.</param>
		/// <remarks>Replaces any previously mapped column or formula.</remarks>
		void Formulas(params string[] formulas);
	}

	// 6.0 TODO: remove once the extended interfaces inherit IColumnOrFormulaMapper
	public static class ColumnsAndFormulasMapperExtensions
	{
		/// <summary>
		/// Maps a mixed list of columns and formulas.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="columnOrFormulaMapper">The mappers for each column or formula.</param>
		/// <remarks>Replaces any previously mapped column or formula.</remarks>
		public static void ColumnsAndFormulas(
			this IElementMapper mapper,
			params Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper)
		{
			CallColumnsAndFormulas(mapper, columnOrFormulaMapper);
		}

		/// <summary>
		/// Maps a mixed list of columns and formulas.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="columnOrFormulaMapper">The mappers for each column or formula.</param>
		/// <remarks>Replaces any previously mapped column or formula.</remarks>
		public static void ColumnsAndFormulas(
			this IManyToManyMapper mapper,
			params Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper)
		{
			CallColumnsAndFormulas(mapper, columnOrFormulaMapper);
		}

		/// <summary>
		/// Maps a mixed list of columns and formulas.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="columnOrFormulaMapper">The mappers for each column or formula.</param>
		/// <remarks>Replaces any previously mapped column or formula.</remarks>
		public static void ColumnsAndFormulas(
			this IManyToOneMapper mapper,
			params Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper)
		{
			CallColumnsAndFormulas(mapper, columnOrFormulaMapper);
		}

		/// <summary>
		/// Maps a mixed list of columns and formulas.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="columnOrFormulaMapper">The mappers for each column or formula.</param>
		/// <remarks>Replaces any previously mapped column or formula.</remarks>
		public static void ColumnsAndFormulas(
			this IMapKeyManyToManyMapper mapper,
			params Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper)
		{
			CallColumnsAndFormulas(mapper, columnOrFormulaMapper);
		}

		/// <summary>
		/// Maps a mixed list of columns and formulas.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="columnOrFormulaMapper">The mappers for each column or formula.</param>
		/// <remarks>Replaces any previously mapped column or formula.</remarks>
		public static void ColumnsAndFormulas(
			this IMapKeyMapper mapper,
			params Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper)
		{
			CallColumnsAndFormulas(mapper, columnOrFormulaMapper);
		}

		/// <summary>
		/// Maps a mixed list of columns and formulas.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="columnOrFormulaMapper">The mappers for each column or formula.</param>
		/// <remarks>Replaces any previously mapped column or formula.</remarks>
		public static void ColumnsAndFormulas(
			this IPropertyMapper mapper,
			params Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper)
		{
			CallColumnsAndFormulas(mapper, columnOrFormulaMapper);
		}

		private static void CallColumnsAndFormulas(
			object mapper,
			Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper)
		{
			var colsOrForMapper = ReflectHelper.CastOrThrow<IColumnsAndFormulasMapper>(
				mapper,
				nameof(IColumnsAndFormulasMapper.ColumnsAndFormulas));
			colsOrForMapper.ColumnsAndFormulas(columnOrFormulaMapper);
		}

		/// <summary>
		/// Maps many formulas.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="formulas">The formulas to map.</param>
		/// <remarks>Replaces any previously mapped column or formula.</remarks>
		public static void Formulas(this IElementMapper mapper, params string[] formulas)
		{
			CallFormulas(mapper, formulas);
		}

		/// <summary>
		/// Maps many formulas.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="formulas">The formulas to map.</param>
		/// <remarks>Replaces any previously mapped column or formula.</remarks>
		public static void Formulas(this IManyToManyMapper mapper, params string[] formulas)
		{
			CallFormulas(mapper, formulas);
		}

		/// <summary>
		/// Maps many formulas.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="formulas">The formulas to map.</param>
		/// <remarks>Replaces any previously mapped column or formula.</remarks>
		public static void Formulas(this IManyToOneMapper mapper, params string[] formulas)
		{
			CallFormulas(mapper, formulas);
		}

		/// <summary>
		/// Maps many formulas.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="formulas">The formulas to map.</param>
		/// <remarks>Replaces any previously mapped column or formula.</remarks>
		public static void Formulas(this IMapKeyManyToManyMapper mapper, params string[] formulas)
		{
			CallFormulas(mapper, formulas);
		}

		/// <summary>
		/// Maps many formulas.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="formulas">The formulas to map.</param>
		/// <remarks>Replaces any previously mapped column or formula.</remarks>
		public static void Formulas(this IMapKeyMapper mapper, params string[] formulas)
		{
			CallFormulas(mapper, formulas);
		}

		/// <summary>
		/// Maps many formulas.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="formulas">The formulas to map.</param>
		/// <remarks>Replaces any previously mapped column or formula.</remarks>
		public static void Formulas(this IPropertyMapper mapper, params string[] formulas)
		{
			CallFormulas(mapper, formulas);
		}

		private static void CallFormulas(object mapper, string[] formulas)
		{
			var colsOrForMapper = ReflectHelper.CastOrThrow<IColumnsAndFormulasMapper>(
				mapper,
				"Setting many formula");
			colsOrForMapper.Formulas(formulas);
		}
	}
}
