using System;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Type;

namespace NHibernate.Tuple
{
	/// <summary>
	/// Represents a defined entity identifier property within the Hibernate
	/// runtime-metamodel.
	/// </summary>
	/// <remarks>
	/// Author: Steve Ebersole
	/// </remarks>
	[Serializable]
	public class IdentifierProperty : Property
	{
		private bool isVirtual;
		private bool embedded;
		private Cascades.IdentifierValue unsavedValue;
		private IIdentifierGenerator identifierGenerator;
		private bool identifierAssignedByInsert;

		/// <summary>
		/// Construct a non-virtual identifier property. 
		/// </summary>
		/// <param name="name">The name of the property representing the identifier within
		/// its owning entity.</param>
		/// <param name="node">The node name to use for XML-based representation of this
		/// property.</param>
		/// <param name="type">The Hibernate Type for the identifier property.</param>
		/// <param name="embedded">Is this an embedded identifier.</param>
		/// <param name="unsavedValue">The value which, if found as the value on the identifier
		/// property, represents new (i.e., un-saved) instances of the owning entity.</param>
		/// <param name="identifierGenerator">The generator to use for id value generation.</param>
		public IdentifierProperty(
			String name,
			String node,
			IType type,
			bool embedded,
			Cascades.IdentifierValue unsavedValue,
			IIdentifierGenerator identifierGenerator)
			: base(name, node, type)
		{
			this.isVirtual = false;
			this.embedded = embedded;
			this.unsavedValue = unsavedValue;
			this.identifierGenerator = identifierGenerator;
			this.identifierAssignedByInsert = identifierGenerator is IdentityGenerator;
			// TODO H3: identifierGenerator is PostInsertIdentifierGenerator;
		}

		/// <summary>
		/// Construct a virtual IdentifierProperty. 
		/// </summary>
		/// <param name="type">The Hibernate Type for the identifier property.</param>
		/// <param name="embedded">Is this an embedded identifier.</param>
		/// <param name="unsavedValue">The value which, if found as the value on the identifier
		/// property, represents new (i.e., un-saved) instances of the owning entity.</param>
		/// <param name="identifierGenerator">The generator to use for id value generation.</param>
		public IdentifierProperty(
			IType type,
			bool embedded,
			Cascades.IdentifierValue unsavedValue,
			IIdentifierGenerator identifierGenerator)
			: base(null, null, type)
		{
			this.isVirtual = true;
			this.embedded = embedded;
			this.unsavedValue = unsavedValue;
			this.identifierGenerator = identifierGenerator;
			this.identifierAssignedByInsert = identifierGenerator is IdentityGenerator;
			// TODO H3: identifierGenerator is PostInsertIdentifierGenerator;
		}

		public bool IsVirtual
		{
			get { return isVirtual; }
		}

		public bool IsEmbedded
		{
			get { return embedded; }
		}

		public Cascades.IdentifierValue UnsavedValue
		{
			get { return unsavedValue; }
		}

		public IIdentifierGenerator IdentifierGenerator
		{
			get { return identifierGenerator; }
		}

		public bool IsIdentifierAssignedByInsert
		{
			get { return identifierAssignedByInsert; }
		}
	}
}