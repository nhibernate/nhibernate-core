namespace NHibernate.Engine
{
	/// <summary>
	/// An enum of the different ways a value might be "included".
	/// </summary>
	/// <remarks>
	/// This is really an expanded true/false notion with Partial being the
	/// expansion. Partial deals with components in the cases where
	/// parts of the referenced component might define inclusion, but the
	/// component overall does not.
	/// </remarks>
	public enum ValueInclusion
	{
		None,
		Partial,
		Full
	}
}
