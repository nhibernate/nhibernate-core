using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1831
{
	public class DocumentType
	{
		// Used by reflection
#pragma warning disable CS0169 // The field is never used
		Guid oid;
#pragma warning restore CS0169 // The field is never used
		// Used by reflection
#pragma warning disable CS0414 // The field is assigned but its value is never used
		SystemAction systemAction = 0;
#pragma warning restore CS0414 // The field is assigned but its value is never used
	}

	[Flags]
	[Serializable]
	public enum SystemAction : long
	{
		P10 = 0x01 << 0,
		Denunciation = 0x01 << 1,
		WarnedOfInterest = 0x01 << 2,
		Individual = 0x01 << 3,
	}
}
