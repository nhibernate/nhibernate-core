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
		private readonly bool isVirtual;
		private readonly bool embedded;
		private readonly IdentifierValue unsavedValue;
		private readonly IIdentifierGenerator identifierGenerator;
		private readonly bool identifierAssignedByInsert;
		private readonly bool hasIdentifierMapper;


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
			IdentifierValue unsavedValue,
			IIdentifierGenerator identifierGenerator)
			: base(name, node, type)
		{
			isVirtual = false;
			this.embedded = embedded;
			hasIdentifierMapper = false;
			this.unsavedValue = unsavedValue;
			this.identifierGenerator = identifierGenerator;
			identifierAssignedByInsert = identifierGenerator is IPostInsertIdentifierGenerator;
		}

		/// <summary>
		/// Construct a virtual IdentifierProperty. 
		/// </summary>
		/// <param name="type">The Hibernate Type for the identifier property.</param>
		/// <param name="embedded">Is this an embedded identifier.</param>
		/// <param name="unsavedValue">The value which, if found as the value on the identifier
		/// property, represents new (i.e., un-saved) instances of the owning entity.</param>
		/// <param name="identifierGenerator">The generator to use for id value generation.</param>
		/// <param name="hasIdentifierMapper"></param>
		public IdentifierProperty(IType type, bool embedded, bool hasIdentifierMapper, IdentifierValue unsavedValue, IIdentifierGenerator identifierGenerator)
			: base(null, null, type)
		{
			isVirtual = true;
			this.embedded = embedded;
			this.hasIdentifierMapper = hasIdentifierMapper;
			this.unsavedValue = unsavedValue;
			this.identifierGenerator = identifierGenerator;
			identifierAssignedByInsert = identifierGenerator is IPostInsertIdentifierGenerator;
		}

		public bool IsVirtual
		{
			get { return isVirtual; }
		}

		public bool IsEmbedded
		{
			get { return embedded; }
		}

		public IdentifierValue UnsavedValue
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

		public bool HasIdentifierMapper
		{
			get { return hasIdentifierMapper; }
		}
	}
}