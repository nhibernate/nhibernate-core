using System;

using NUnit.Framework;
using NHibernate.Type;

namespace NHibernate.Test.TypesTest
{
	[TestFixture]
	public class SerializableTypesFixture
	{
		[Test]
		public void AllEmbeddedTypesAreMarkedSerializable()
		{
			NHAssert.InheritedAreMarkedSerializable(typeof(NHibernate.Type.IType));
		}

		[Test]
		public void EachEmbeddedBasicTypeIsSerializable()
		{
			IType ntp = NHibernateUtil.AnsiChar;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.AnsiString;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Binary;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.BinaryBlob;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Boolean;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Byte;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Character;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Class;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.CultureInfo;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Date;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.DateTime;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Decimal;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Double;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Guid;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Int16;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Int32;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Int64;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Object;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.SByte;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Serializable;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Single;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.String;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.StringClob;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Ticks;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Time;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.TimeSpan;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.Timestamp;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.TrueFalse;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.UInt16;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.UInt32;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.UInt64;
			NHAssert.IsSerializable(ntp);
			ntp = NHibernateUtil.YesNo;
			NHAssert.IsSerializable(ntp);
		}
	}
}
