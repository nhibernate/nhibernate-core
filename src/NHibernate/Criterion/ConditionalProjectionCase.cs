namespace NHibernate.Criterion
{
	/// <summary>
	/// Defines a pair of <see cref="ConditionalProjectionCase.Criterion"/> and <see cref="ConditionalProjectionCase.Projection"/>.
	/// </summary>
	public class ConditionalProjectionCase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionalProjectionCase"/> class.
		/// </summary>
		/// <param name="criterion">The <see cref="ConditionalProjectionCase.Criterion"/>.</param>
		/// <param name="projection">The <see cref="ConditionalProjectionCase.Projection"/>.</param>
		public ConditionalProjectionCase(ICriterion criterion, IProjection projection)
		{
			Criterion = criterion;
			Projection = projection;
		}

		/// <summary>
		/// Gets the <see cref="ICriterion"/>.
		/// </summary>
		public ICriterion Criterion { get; }

		/// <summary>
		/// Gets the <see cref="IProjection"/>.
		/// </summary>
		public IProjection Projection { get; }
	}
}
