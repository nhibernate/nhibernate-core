using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1831
{
	public class DocumentType
	{
		Guid oid;
		SystemAction systemAction = 0;
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
