using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Util {
	
	[TestFixture]
	public class SequencedHashMapTest {
		private SequencedHashMap shm;
		private IList keys;
		private IList values;

		[SetUp]
		public void SetUp() {
			shm = new SequencedHashMap();

			keys = new ArrayList();
			keys.Add("test1"); keys.Add("test2"); keys.Add("test3");
			values = new ArrayList();
			values.Add(1); values.Add("2"); values.Add(true);

			for (int i=0; i<keys.Count; i++) {
				shm[keys[i]] = values[i];
			}
		}

		[Test]
		public void TestBasic() {
			
			Assertion.AssertEquals(1, shm["test1"]);
			Assertion.AssertEquals("2", shm["test2"]);
			Assertion.AssertEquals(true, shm["test3"]);

			Assertion.AssertEquals(3, shm.Count);

			Assertion.Assert( shm.Contains("test1") );
			Assertion.Assert( shm.ContainsKey("test1") );
			Assertion.Assert( shm.ContainsValue("2") );
			Assertion.Assert( shm.ContainsValue(true) );
		}

		[Test]
		public void TestKeyEnumerator() {
			int i=0;
			foreach(object obj in shm.Keys) {
				i++;
				Assertion.Assert( keys.Contains(obj) );
			}

			Assertion.AssertEquals(3, i);

			SequencedHashMap empty = new SequencedHashMap();
			foreach(object obj in empty.Keys) {
				Assertion.Fail("should not be a key");
			}
		}

		[Test]
		public void TestValueEnumerator() {
			int i=0;
			foreach(object obj in shm.Values) {
				i++;
				Assertion.Assert( values.Contains(obj) );
			}

			Assertion.AssertEquals(3, i);

			SequencedHashMap empty = new SequencedHashMap();
			foreach(object obj in empty.Values) {
				Assertion.Fail("should not be a value");
			}
		}
		

	}
}
