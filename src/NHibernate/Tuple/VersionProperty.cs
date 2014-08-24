using System;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Tuple
{
	/// <summary>
	/// Represents a version property within the Hibernate runtime-metamodel.
	/// </summary>
	/// <remarks>
	/// Author: Steve Ebersole
	/// </remarks>
	[Serializable]
	public class VersionProperty : StandardProperty
	{
		private readonly VersionValue unsavedValue;

		/// <summary>
		/// Constructs VersionProperty instances.
		/// </summary>
		/// <param name="name">The name by which the property can be referenced within
		/// its owner.</param>
		/// <param name="node">The node name to use for XML-based representation of this
		/// property.</param>
		/// <param name="type">The Hibernate Type of this property.</param>
		/// <param name="lazy">Should this property be handled lazily?</param>
		/// <param name="insertable">Is this property an insertable value?</param>
		/// <param name="updateable">Is this property an updateable value?</param>
		/// <param name="insertGenerated">Is this property generated in the database on insert?</param>
		/// <param name="updateGenerated">Is this property generated in the database on update?</param>
		/// <param name="nullable">Is this property a nullable value?</param>
		/// <param name="checkable">Is this property a checkable value?</param>
		/// <param name="versionable">Is this property a versionable value?</param>
		/// <param name="cascadeStyle">The cascade style for this property's value.</param>
		/// <param name="unsavedValue">The value which, if found as the value of
		/// this (i.e., the version) property, represents new (i.e., un-saved)
		/// instances of the owning entity.</param>
		public VersionProperty(
			string name,
			string node,
			IType type,
			bool lazy,
			bool insertable,
			bool updateable,
			bool insertGenerated,
			bool updateGenerated,
			bool nullable,
			bool checkable,
			bool versionable,
			CascadeStyle cascadeStyle,
			VersionValue unsavedValue)
			: base(
				name, node, type, lazy, insertable, updateable, insertGenerated, updateGenerated, nullable, checkable, versionable,
				cascadeStyle, null)
		{
			this.unsavedValue = unsavedValue;
		}

		public VersionValue UnsavedValue
		{
			get { return unsavedValue; }
		}
	}
}