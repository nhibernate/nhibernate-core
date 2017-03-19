using System;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	/// <summary>
	/// Immutable value class. By-value equality.
	/// </summary>
	public class PropertyPath
	{
		private readonly int hashCode;
		private readonly MemberInfo localMember;
		private readonly PropertyPath previousPath;
		private readonly System.Type componentType;

		public PropertyPath(PropertyPath previousPath, MemberInfo localMember)
#if FEATURE_REFLECTEDTYPE
			: this(previousPath, localMember, localMember.ReflectedType)
#else
			: this(previousPath, localMember, localMember.DeclaringType)
#endif
		{
			//if (localMember.DeclaringType != localMember.ReflectedType)
			//{
			//	throw new InvalidOperationException(string.Format("DeclaringType {0} not the same as ReflectedType {1}", localMember.DeclaringType, localMember.ReflectedType));
			//}
		}

		public PropertyPath(PropertyPath previousPath, MemberInfo localMember, System.Type componentType)
		{
			if (localMember == null)
			{
				throw new ArgumentNullException("localMember");
			}
			if (componentType == null)
			{
				throw new ArgumentNullException("componentType");
			}
			if (!localMember.DeclaringType.IsAssignableFrom(componentType))
			{
				throw new ArgumentException("source Member not implemented on passed in type", "componentType");
			}
			//if (componentType != localMember.ReflectedType)
			//{
			//	throw new ArgumentException(string.Format("componentType {0} not the same as ReflectedType {1}", componentType, localMember.ReflectedType), "componentType");
			//}
			this.previousPath = previousPath;
			this.localMember = localMember;
			this.componentType = componentType;

			hashCode = localMember.GetHashCode() ^ (previousPath != null ? previousPath.GetHashCode() : 41);
		}

		public PropertyPath PreviousPath
		{
			get { return previousPath; }
		}

		public MemberInfo LocalMember
		{
			get { return localMember; }
		}

		public System.Type ComponentType
		{
			get { return componentType; }
		}

		public MemberInfo GetRootMember()
		{
			PropertyPath analizing = this;
			while (analizing.previousPath != null)
			{
				analizing = analizing.previousPath;
			}
			return analizing.localMember;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			return Equals(obj as PropertyPath);
		}

		public bool Equals(PropertyPath other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(previousPath, other.previousPath) && localMember.Equals(other.localMember);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		public string ToColumnName()
		{
			return PreviousPath == null ? LocalMember.Name : PreviousPath.ToColumnName() + LocalMember.Name;
		}

		public override string ToString()
		{
			return PreviousPath == null ? LocalMember.Name : PreviousPath + "." + LocalMember.Name;
		}
	}
}
