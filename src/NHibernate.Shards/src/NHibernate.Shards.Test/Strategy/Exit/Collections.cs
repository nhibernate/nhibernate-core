using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Shards.Test.Strategy.Exit
{
	public class Collections
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="listToRandom"></param>
		/// <param name="rnd"></param>
		/// <returns></returns>
		public static IList<T> RandomList<T>(IEnumerable<T> listToRandom, Random rnd)
		{
			List<T> list = new List<T>(listToRandom);
			
			T[] arr = list.ToArray();

			for(int i = list.Count; i > 1; i--)
			{
				Swap(arr,i-1,rnd.Next(i));
			}
			return new List<T>(arr);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="listToRandom"></param>
		/// <returns></returns>
		public static IList<T> RandomList<T>(IEnumerable<T> listToRandom)
		{
			return RandomList(listToRandom, new Random());
		}

		private static void Swap<T>(T[] arr, int i, int j)
		{
			T tmp = arr[i];
			arr[i] = arr[j];
			arr[j] = tmp;
		}
	}

	[TestFixture]
	public class CollectionsFixture 
	{
		[Test]
		public void Array()
		{
			string[] array = new string[]{"a","b","c","d"};

			IList<string> list = Collections.RandomList(array);
		}

		[Test]
		public void List() 
		{
			List<string> list = new List<string>();
			list.Add("a");
			list.Add("b");
			list.Add("c");
			list.Add("d");
			
			IList<string> list2 = Collections.RandomList(list);
		}
	}
}