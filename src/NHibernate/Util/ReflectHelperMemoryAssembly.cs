using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Util
{
	public class ReflectHelperMemoryAssembly
    {
		private static readonly ReflectHelperMemoryAssembly instance = new ReflectHelperMemoryAssembly();
		private Dictionary<string, Assembly> _assemblyList;

		public static ReflectHelperMemoryAssembly Instance
		{
			get
			{
				return instance;
			}
		}

		/// <summary>
		///  Remove all stored assembly in instance.
		/// </summary>
		public void Reset()
		{
			_assemblyList = new Dictionary<string, Assembly>();
		}

		public ReflectHelperMemoryAssembly()
		{
			Reset();
		}

		/// <summary>
		///  Add new assembly to store.
		/// </summary>
		/// <param name="assembly">Assembly object.</param>
		/// <returns>True if assembly was stored</returns>
		public bool AddMemoryAssembly(Assembly assembly)
		{
			string name = assembly.FullName;
			if (name.Length > 0 && !_assemblyList.ContainsKey(name))
			{ 
				_assemblyList.Add(name, assembly);
				return true;
			}
			return false;
		}

		/// <summary>
		///  Get number of stored assembly.
		/// </summary>
		public int GetAssemblyCount()
		{
			return _assemblyList.Count;
		}

		/// <summary>
		///  Get type from specific assembly.
		/// </summary>
		/// <param name="type">Specify name class to get.</param>
		/// /// <param name="assemblyName">Specify assembly name.</param>
		public System.Type GetType(string type, string assemblyName)
		{
			return _assemblyList.ContainsKey(assemblyName)
				?
					_assemblyList[assemblyName].GetType(type)
				:
					_assemblyList.First(s => GetAssemblyName(s.Key) == assemblyName).Value.GetType(type)
				;
		}

		/// <summary>
		///  Get first detected type of class in all assembly.
		/// </summary>
		/// <param name="type">Specify name class to get.</param>
		public System.Type GetType(string type)
		{
			System.Type getType = null;
			foreach (var assembly in _assemblyList)
			{
				getType = assembly.Value.GetType(type);
				if(getType != null)
				{
					break;
				}
			}
			return getType;
		}

		/// <summary>
		///  Remove specify stored assembly.
		/// </summary>
		/// <param name="name">Assembly fullname.</param>
		/// <returns>True if assembly was removed</returns>
		public bool RemoveAssembly(string name)
		{
			return _assemblyList.Remove(name);
		}

		private string GetAssemblyName(Assembly assembly)
		{
			return GetAssemblyName(assembly.FullName);
		}

		private string GetAssemblyName(string name)
		{
			return name.Split(',')[0] ?? null;
		}
	}
}
