#if !NETFX
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NHibernate.Tool.HbmXsd;

/// <summary>Reads one node of the model which an <see cref="XmlSchemaImporter" /> builds.</summary>
/// <remarks>
/// The model is internal to the assembly that supplies <see cref="XmlSchemaImporter" />, so
/// reflection gives access to it. One class holds the members of all of the model classes.
/// </remarks>
internal class MappingModel
{
	private const BindingFlags Lookup = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	private static readonly MethodInfo Unescape = typeof (XmlSchemas).Assembly
		.GetType("System.Xml.Serialization.Accessor", true)
		.GetMethod("UnescapeName", BindingFlags.Static | Lookup, null, [typeof (string)], null);

	private readonly object target;

	private MappingModel(object target)
	{
		this.target = target;
	}

	// Members of an accessor, which tells how an element, an attribute or a text value is read.
	public string Name => Get<string>();
	public string Namespace => Get<string>();
	public MappingModel Mapping => Of(Read());
	public object Default => Get<object>();
	public bool Any => Get<bool>();
	public bool IsFixed => Get<bool>();
	public bool IsNullable => Get<bool>();
	public bool IsOptional => Get<bool>();
	public XmlSchemaForm Form => Get<XmlSchemaForm>();

	// Members of a mapping, which tells how a type or one member of a type is read.
	public MappingModel Accessor => Of(Read());
	public MappingModel Attribute => Of(Read());
	public MappingModel Text => Of(Read());
	public MappingModel[] Elements => All();
	public MappingModel[] Members => All();
	public MappingModel[] Constants => All();
	public MappingModel BaseMapping => Of(Read());
	public MappingModel DerivedMappings => Of(Read());
	public MappingModel NextDerivedMapping => Of(Read());
	public MappingModel TypeDesc => Of(Read());
	public object Xmlns => Get<object>();
	public string TypeName => Get<string>();
	public string XmlName => Get<string>();
	public bool Ignore => Get<bool>();
	public bool IsFlags => Get<bool>();
	public bool IncludeInSchema => Get<bool>();
	public bool ReferencedByTopLevelElement => Get<bool>();

	/// <summary>Tells if the member needs a companion member which tells if it has a value.</summary>
	public bool CheckSpecified => Get<object>().ToString() != "None";

	// Members of a type description, which tells which CLR type a mapping uses.
	public string FullName => Get<string>();
	public string FormatterName => Get<string>();
	public MappingModel BaseTypeDesc => Of(Read());
	public MappingModel ArrayElementTypeDesc => Of(Read());
	public XmlSchemaType DataType => Get<XmlSchemaType>();
	public bool IsRoot => Get<bool>();
	public bool IsAbstract => Get<bool>();
	public bool IsArrayLike => Get<bool>();
	public bool IsValueType => Get<bool>();
	public bool IsAmbiguousDataType => Get<bool>();
	public bool HasDefaultSupport => Get<bool>();

	public static MappingModel Of(object target)
	{
		return target == null ? null : new MappingModel(target);
	}

	public static string UnescapeName(string name)
	{
		return (string) Unescape.Invoke(null, [name]);
	}

	/// <summary>Tells if the node is of the named model class.</summary>
	public bool Is(string modelClass)
	{
		return target.GetType().Name == modelClass;
	}

	public override bool Equals(object obj)
	{
		return obj is MappingModel other && ReferenceEquals(target, other.target);
	}

	public override int GetHashCode()
	{
		return RuntimeHelpers.GetHashCode(target);
	}

	private T Get<T>([CallerMemberName] string name = null)
	{
		return (T) Read(name);
	}

	private MappingModel[] All([CallerMemberName] string name = null)
	{
		var values = (Array) Read(name);

		if (values == null)
			return [];

		var models = new MappingModel[values.Length];

		for (var i = 0; i < models.Length; i++)
			models[i] = Of(values.GetValue(i));

		return models;
	}

	private object Read([CallerMemberName] string name = null)
	{
		var reader = GetReader(target.GetType(), name);

		return reader is PropertyInfo property ? property.GetValue(target) : ((FieldInfo) reader).GetValue(target);
	}

	private static MemberInfo GetReader(Type type, string name)
	{
		var property = type.GetProperty(name, Lookup);
		// Some properties only have a set method. Their backing field gives the value.
		var reader = property?.GetMethod != null ? property : (MemberInfo) FindBackingField(type, name);

		if (reader == null)
			throw new NotSupportedException($"{type} has no readable {name} member. This tool does not support this version of {type.Assembly.GetName().Name}.");

		return reader;
	}

	private static FieldInfo FindBackingField(Type type, string propertyName)
	{
		for (var declaringType = type; declaringType != null; declaringType = declaringType.BaseType)
		{
			foreach (var field in declaringType.GetFields(Lookup | BindingFlags.DeclaredOnly))
			{
				var name = field.Name.TrimStart('_');

				if (string.Equals(name, propertyName, StringComparison.OrdinalIgnoreCase) ||
					string.Equals("is" + name, propertyName, StringComparison.OrdinalIgnoreCase))
					return field;
			}
		}

		return null;
	}
}
#endif
