using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Mapping.ByCode
{
	/// <summary>
	/// The possible types of polymorphism for IClassMapper.
	/// </summary>
	public enum PolymorphismType
	{
		/// <summary>
		/// Implicit polymorphism
		/// </summary>
		Implicit,

		/// <summary>
		/// Explicit polymorphism
		/// </summary>
		Explicit
	}
}
