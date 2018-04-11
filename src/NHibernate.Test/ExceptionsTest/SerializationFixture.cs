using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace NHibernate.Test.ExceptionsTest
{
	[TestFixture]
	public class SerializationFixture
	{
		[Test]
		public void InstantiationExceptionSerialization()
		{
			var formatter = new BinaryFormatter();
			using (var memoryStream = new MemoryStream())
			{
				formatter.Serialize(memoryStream, new InstantiationException("test", GetType()));
				memoryStream.Position = 0;
				var ex = formatter.Deserialize(memoryStream) as InstantiationException;
				Assert.That(ex, Is.Not.Null);
				Assert.That(ex.PersistentType, Is.EqualTo(GetType()));
			}
		}
	}
}
