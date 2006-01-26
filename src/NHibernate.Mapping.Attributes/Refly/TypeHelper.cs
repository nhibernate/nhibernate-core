/// Refly License
/// 
/// Copyright (c) 2004 Jonathan de Halleux, http://www.dotnetwiki.org
///
/// This software is provided 'as-is', without any express or implied warranty. In no event will the authors be held liable for any damages arising from the use of this software.
/// 
/// Permission is granted to anyone to use this software for any purpose, including commercial applications, and to alter it and redistribute it freely, subject to the following restrictions:
///
/// 1. The origin of this software must not be misrepresented; you must not claim that you wrote the original software. If you use this software in a product, an acknowledgment in the product documentation would be appreciated but is not required.
/// 
/// 2. Altered source versions must be plainly marked as such, and must not be misrepresented as being the original software.
///
///3. This notice may not be removed or altered from any source distribution.

using System;
using System.Reflection;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Refly.CodeDom
{

	/// <summary>
	/// Helper static class for Type related tasks
	/// </summary>
	/// <include file="GUnit.CodeDom.Doc.xml" path="doc/remarkss/remarks[@name='TypeHelper']"/>
	public sealed class TypeHelper
	{
		internal TypeHelper()
		{}

		public static ConstructorInfo GetConstructor(Type t, params Type[] args)
		{
			if (t==null)
				throw new ArgumentNullException("t");

			ConstructorInfo ci = t.GetConstructor(args);
			if (ci==null)
				throw new ArgumentNullException("constructor for " + t.FullName +" not found");
			return ci;
		}	

		public static ConstructorInfo GetDefaultConstructor(Type t)
		{
			if (t==null)
				throw new ArgumentNullException("t");

			ConstructorInfo ci = t.GetConstructor(Type.EmptyTypes);
			if (ci==null)
				throw new ArgumentNullException("no default constructor for " + t.FullName );
			return ci;
		}

		public static bool IsXmlNullable(FieldInfo f)
		{
			if (f.FieldType.IsValueType)
				return false;

			if (!HasCustomAttribute(f,typeof(XmlElementAttribute)))
				return true;

			XmlElementAttribute attr = 
				(XmlElementAttribute)GetFirstCustomAttribute(f,typeof(XmlElementAttribute));
			return attr.IsNullable;
		}

		/// <summary>
		/// Gets a value indicating if the type <paramref name="t"/> is tagged
		/// by a <paramref name="customAttributeType"/> instance.
		/// </summary>
		/// <param name="t">type to test</param>
		/// <param name="customAttributeType">custom attribute type to search</param>
		/// <returns>
		/// true if <param name="t"/> is tagged by a <paramref name="customAttributeType"/>
		/// attribute, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="t"/> or <paramref name="customAttributeType"/>
		/// is a null reference
		/// </exception>
		/// <remarks>
		/// You can use this method to check that a type is tagged by an attribute.
		/// </remarks>
		public static bool HasCustomAttribute(Type t,Type customAttributeType)
		{
			if (t==null)
				throw new ArgumentNullException("t");
			if (customAttributeType==null)
				throw new ArgumentNullException("customAttributeType");

			return t.GetCustomAttributes(customAttributeType,true).Length != 0;
		}

		/// <summary>
		/// Gets a value indicating if the property info <paramref name="t"/> is tagged
		/// by a <paramref name="customAttributeType"/> instance.
		/// </summary>
		/// <param name="t">property to test</param>
		/// <param name="customAttributeType">custom attribute type to search</param>
		/// <returns>
		/// true if <param name="t"/> is tagged by a <paramref name="customAttributeType"/>
		/// attribute, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="t"/> or <paramref name="customAttributeType"/>
		/// is a null reference
		/// </exception>
		/// <remarks>
		/// You can use this property to check that a method is tagged by a
		/// specified attribute.
		/// </remarks>
		public static bool HasCustomAttribute(PropertyInfo t,Type customAttributeType)
		{
			if (t==null)
				throw new ArgumentNullException("t");
			if (customAttributeType==null)
				throw new ArgumentNullException("customAttributeType");

			return t.GetCustomAttributes(customAttributeType,true).Length != 0;
		}

		public static bool HasCustomAttribute(FieldInfo t,Type customAttributeType)
		{
			if (t==null)
				throw new ArgumentNullException("t");
			if (customAttributeType==null)
				throw new ArgumentNullException("customAttributeType");

			return t.GetCustomAttributes(customAttributeType,true).Length != 0;
		}

		/// <summary>
		/// Gets the first instance of <paramref name="customAttributeType"/> 
		/// from the type <paramref name="t"/> custom attributes.
		/// </summary>
		/// <param name="t">type to test</param>
		/// <param name="customAttributeType">custom attribute type to search</param>
		/// <returns>
		/// First instance of <paramref name="customAttributeTyp"/>
		/// from the type <paramref name="t"/> custom attributes.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="t"/> or <paramref name="customAttributeType"/>
		/// is a null reference
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="t"/> is not tagged by an attribute of type
		/// <paramref name="customAttributeType"/>
		/// </exception>
		/// <remarks>
		/// You can use this method to retreive a specified attribute
		/// instance
		/// </remarks>
		public static Object GetFirstCustomAttribute(Type t, Type customAttributeType)
		{
			if (t==null)
				throw new ArgumentNullException("t");
			if (customAttributeType==null)
				throw new ArgumentNullException("customAttributeType");

			Object[] attrs = t.GetCustomAttributes(customAttributeType,true);
			if (attrs.Length==0)
				throw new ArgumentException("type does not have custom attribute");
			return attrs[0];
		}

		/// <summary>
		/// Gets the first instance of <paramref name="customAttributeType"/> 
		/// from the property <paramref name="mi"/> custom attributes.
		/// </summary>
		/// <param name="mi">property to test</param>
		/// <param name="customAttributeType">custom attribute type to search</param>
		/// <returns>
		/// First instance of <paramref name="customAttributeTyp"/>
		/// from the property <paramref name="mi"/> custom attributes.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="mi"/> or <paramref name="customAttributeType"/>
		/// is a null reference
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="mi"/> is not tagged by an attribute of type
		/// <paramref name="customAttributeType"/>
		/// </exception>
		/// <remarks>
		/// You can use this property to retreive a specified attribute
		/// instance of a method.
		/// </remarks>
		public static Object GetFirstCustomAttribute(PropertyInfo mi, Type customAttributeType)
		{
			if (mi==null)
				throw new ArgumentNullException("mi");
			if (customAttributeType==null)
				throw new ArgumentNullException("customAttributeType");

			Object[] attrs = mi.GetCustomAttributes(customAttributeType,true);
			if (attrs.Length==0)
				throw new ArgumentException("type does not have custom attribute");
			return attrs[0];
		}

		public static Object GetFirstCustomAttribute(FieldInfo mi, Type customAttributeType)
		{
			if (mi==null)
				throw new ArgumentNullException("mi");
			if (customAttributeType==null)
				throw new ArgumentNullException("customAttributeType");

			Object[] attrs = mi.GetCustomAttributes(customAttributeType,true);
			if (attrs.Length==0)
				throw new ArgumentException("type does not have custom attribute");
			return attrs[0];
		}	
	}
}
