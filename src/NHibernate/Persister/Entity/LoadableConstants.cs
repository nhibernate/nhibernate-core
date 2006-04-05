using System;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// Constants from <see cref="ILoadable" /> interface.
	/// </summary>
	public sealed class LoadableConstants
	{
		private LoadableConstants() { }
		
		public const string RowIdAlias = "rowid_";
	}
}
