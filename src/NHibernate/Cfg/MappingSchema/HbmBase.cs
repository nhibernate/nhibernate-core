using System;

namespace NHibernate.Cfg.MappingSchema
{
	/// <summary>A base class for HBM schema classes that provides helper methods.</summary>
	[Serializable]
	public abstract class HbmBase
	{
		protected static T Find<T>(object[] array)
		{
			return (T) Array.Find(array, delegate(object obj) { return obj is T; });
		}

		protected static T[] FindAll<T>(object[] array)
		{
			object[] objects = Array.FindAll(array, delegate(object obj) { return obj is T; });
			T[] results = new T[objects.Length];

			for (int i = 0; i < results.Length; i++)
				results[i] = (T) objects[i];

			return results;
		}

		protected static string JoinString(string[] text)
		{
			return text.JoinString();
		}
	}
}