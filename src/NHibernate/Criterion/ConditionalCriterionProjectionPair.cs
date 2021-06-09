namespace NHibernate.Criterion
{
	/// <summary>
	/// Defines a pair of <see cref="ConditionalCriterionProjectionPair.Criterion"/> and <see cref="ConditionalCriterionProjectionPair.Projection"/>.
	/// </summary>
	public class ConditionalCriterionProjectionPair
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionalCriterionProjectionPair"/> class.
		/// </summary>
		/// <param name="criterion">The <see cref="ConditionalCriterionProjectionPair.Criterion"/>.</param>
		/// <param name="projection">The <see cref="ConditionalCriterionProjectionPair.Projection"/>.</param>
		public ConditionalCriterionProjectionPair(ICriterion criterion, IProjection projection)
		{
			this.Criterion = criterion;
			this.Projection = projection;
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
