namespace NHibernate.Event
{
	/// <summary>
	/// Values for listener type property.
	/// </summary>
	public enum ListenerType
	{
		/// <summary>Not allowed in Xml. It represents the default value when an explicit type is assigned.</summary>
		NotValidType,
		/// <summary>Xml value: auto-flush</summary>
		Autoflush,
		/// <summary>Xml value: merge</summary>
		Merge,
		/// <summary>Xml value: create</summary>
		Create,
		/// <summary>Xml value: create-onflush</summary>
		CreateOnFlush,
		/// <summary>Xml value: delete</summary>
		Delete,
		/// <summary>Xml value: dirty-check</summary>
		DirtyCheck,
		/// <summary>Xml value: evict</summary>
		Evict,
		/// <summary>Xml value: flush</summary>
		Flush,
		/// <summary>Xml value: flush-entity</summary>
		FlushEntity,
		/// <summary>Xml value: load</summary>
		Load,
		/// <summary>Xml value: load-collection</summary>
		LoadCollection,
		/// <summary>Xml value: lock</summary>
		Lock,
		/// <summary>Xml value: refresh</summary>
		Refresh,
		/// <summary>Xml value: replicate</summary>
		Replicate,
		/// <summary>Xml value: save-update</summary>
		SaveUpdate,
		/// <summary>Xml value: save</summary>
		Save,
		/// <summary>Xml value: pre-update</summary>
		PreUpdate,
		/// <summary>Xml value: update</summary>
		Update,
		/// <summary>Xml value: pre-load</summary>
		PreLoad,
		/// <summary>Xml value: pre-delete</summary>
		PreDelete,
		/// <summary>Xml value: pre-insert</summary>
		PreInsert,
		/// <summary>Xml value: pre-collection-recreate </summary>
		PreCollectionRecreate,
		/// <summary>Xml value: pre-collection-remove </summary>
		PreCollectionRemove,
		/// <summary>Xml value: pre-collection-update </summary>
		PreCollectionUpdate,
		/// <summary>Xml value: post-load</summary>
		PostLoad,
		/// <summary>Xml value: post-insert</summary>
		PostInsert,
		/// <summary>Xml value: post-update</summary>
		PostUpdate,
		/// <summary>Xml value: post-delete</summary>
		PostDelete,
		/// <summary>Xml value: post-commit-update</summary>
		PostCommitUpdate,
		/// <summary>Xml value: post-commit-insert</summary>
		PostCommitInsert,
		/// <summary>Xml value: post-commit-delete</summary>
		PostCommitDelete,
		/// <summary>Xml value: post-collection-recreate </summary>
		PostCollectionRecreate,
		/// <summary>Xml value: post-collection-remove </summary>
		PostCollectionRemove,
		/// <summary>Xml value: post-collection-update </summary>
		PostCollectionUpdate,
	}
}
