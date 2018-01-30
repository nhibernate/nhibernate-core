using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;
using NHibernate.Test.NHSpecificTest.NH1274ExportExclude;

namespace NHibernate.Test.Legacy
{
	public sealed class CultureInfoSerializationSurrogate : ISerializationSurrogate
	{
		public static SurrogateSelector Add(SurrogateSelector ss)
		{
			ss.AddSurrogate(typeof(CultureInfo), new StreamingContext(StreamingContextStates.All), new CultureInfoSerializationSurrogate());
			return ss;
		}

		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			var cultureInfo = (CultureInfo) obj;
			info.SetType(typeof(ObjectReference));
			info.AddValue("_cultureinfoname", cultureInfo?.Name);
		}

		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			throw new NotImplementedException("The object reference should be deserializing.");
		}

		[Serializable]
		private sealed class ObjectReference : IObjectReference, ISerializable
		{
			private readonly string _cultureinfoname;

			private ObjectReference(SerializationInfo info, StreamingContext context)
			{
				_cultureinfoname = info.GetString("_cultureinfoname");
			}

			[SecurityCritical]
			public object GetRealObject(StreamingContext context)
			{
				return (_cultureinfoname == null) ? null : CultureInfo.GetCultureInfo(_cultureinfoname);
			}

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new NotImplementedException("This class should not be serialized directly.");
			}
		}
	}
}
