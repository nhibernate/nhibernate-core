#if !NETFX
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NHibernate.Tool.HbmXsd;

/// <summary>Makes a code DOM from the mappings of an <see cref="XmlSchemaImporter" />.</summary>
/// <remarks>
/// This is a port of <c>System.Xml.Serialization.XmlCodeExporter</c>, which the .NET class
/// library does not supply any more. It makes public fields, and it throws for the schema
/// constructs which the NHibernate mapping schema does not use.
/// Ported from the .NET Framework 4.7 reference source, Copyright (c) Microsoft Corporation,
/// MIT license:
/// https://github.com/microsoft/referencesource/blob/4251daa76e0aae7330139978648fc04f5c7b8ccb/System.Xml/System/Xml/Serialization/XmlCodeExporter.cs
/// </remarks>
public class XmlCodeExporter
{
	private const string Remarks = "<remarks/>";

	private readonly CodeNamespace code;

	private readonly Dictionary<MappingModel, CodeTypeDeclaration> exportedTypes = [];

	private CodeAttributeDeclaration generatedCodeAttribute;
	private bool rootExported;

	/// <summary>Takes the same arguments as the class of the .NET Framework, which it replaces.</summary>
	/// <param name="code">The namespace which holds the generated types.</param>
	/// <param name="codeCompileUnit">Not used. The .NET Framework class adds assembly references to it.</param>
	/// <param name="options">Must be <see cref="CodeGenerationOptions.None" />.</param>
	public XmlCodeExporter(CodeNamespace code, CodeCompileUnit codeCompileUnit, CodeGenerationOptions options)
	{
		ArgumentNullException.ThrowIfNull(code);

		if (options != CodeGenerationOptions.None)
			throw new NotSupportedException($"The {options} code generation options are not supported.");

		this.code = code;
	}

	/// <summary>Adds the types of a mapping to the code DOM.</summary>
	/// <param name="mapping">The mapping of a top level element of the schema.</param>
	public void ExportTypeMapping(XmlTypeMapping mapping)
	{
		ArgumentNullException.ThrowIfNull(mapping);

		var element = MappingModel.Of(mapping).Accessor;
		ExportType(element.Mapping, MappingModel.UnescapeName(element.Name), element.Namespace, element, true);
	}

	private void ExportType(MappingModel mapping, string name, string ns, MappingModel rootElement, bool checkReference)
	{
		// A type which a top level element references gets its code when that element is exported.
		if (checkReference && rootElement == null && mapping.Is("StructMapping") && mapping.ReferencedByTopLevelElement)
			return;

		if (!exportedTypes.TryGetValue(mapping, out var codeClass))
		{
			// The mapping is marked before its export, because the schema is recursive.
			exportedTypes.Add(mapping, null);
			codeClass = MakeType(mapping);

			if (codeClass != null)
			{
				codeClass.CustomAttributes.Add(GeneratedCodeAttribute);
				codeClass.CustomAttributes.Add(new CodeAttributeDeclaration(typeof (SerializableAttribute).FullName));

				if (!codeClass.IsEnum)
				{
					codeClass.CustomAttributes.Add(new CodeAttributeDeclaration(typeof (DebuggerStepThroughAttribute).FullName));
					codeClass.CustomAttributes.Add(new CodeAttributeDeclaration(typeof (DesignerCategoryAttribute).FullName,
						new CodeAttributeArgument(new CodePrimitiveExpression("code"))));
				}

				AddTypeMetadata(codeClass.CustomAttributes, mapping);
				exportedTypes[mapping] = codeClass;
			}
		}

		if (codeClass != null && rootElement != null)
			AddRootMetadata(codeClass.CustomAttributes, mapping, name, ns, rootElement);
	}

	private CodeTypeDeclaration MakeType(MappingModel mapping)
	{
		if (mapping.Is("EnumMapping"))
			return ExportEnum(mapping);

		if (mapping.Is("StructMapping"))
			return ExportStruct(mapping);

		if (mapping.Is("ArrayMapping"))
			throw new NotSupportedException("Lists and arrays are not supported.");

		return null;
	}

	private CodeTypeDeclaration ExportEnum(MappingModel mapping)
	{
		if (mapping.IsFlags)
			throw new NotSupportedException("Enumerations of flags are not supported.");

		var codeClass = new CodeTypeDeclaration(mapping.TypeDesc.Name) {IsEnum = true};
		codeClass.Comments.Add(new CodeCommentStatement(Remarks, true));
		codeClass.TypeAttributes |= TypeAttributes.Public;
		code.Types.Add(codeClass);

		foreach (var constant in mapping.Constants)
		{
			var field = new CodeMemberField(typeof (int).FullName, constant.Name);
			field.Comments.Add(new CodeCommentStatement(Remarks, true));
			codeClass.Members.Add(field);

			if (constant.XmlName != constant.Name)
				field.CustomAttributes.Add(new CodeAttributeDeclaration(typeof (XmlEnumAttribute).FullName,
					new CodeAttributeArgument(new CodePrimitiveExpression(constant.XmlName))));
		}

		return codeClass;
	}

	private CodeTypeDeclaration ExportStruct(MappingModel mapping)
	{
		var typeDesc = mapping.TypeDesc;

		if (typeDesc.IsRoot)
		{
			// xs:anyType gets no code of its own. Only the types which derive from it are written.
			if (!rootExported)
			{
				rootExported = true;
				ExportDerivedStructs(mapping);
			}

			return null;
		}

		var codeClass = new CodeTypeDeclaration(typeDesc.Name) {IsPartial = true};
		codeClass.Comments.Add(new CodeCommentStatement(Remarks, true));
		codeClass.TypeAttributes |= TypeAttributes.Public;
		code.Types.Add(codeClass);

		var constructor = new CodeConstructor();
		constructor.Attributes = (constructor.Attributes & ~MemberAttributes.AccessMask) | MemberAttributes.Public;
		codeClass.Members.Add(constructor);

		var baseTypeDesc = typeDesc.BaseTypeDesc;

		if (baseTypeDesc != null && !baseTypeDesc.IsRoot)
			codeClass.BaseTypes.Add(baseTypeDesc.FullName);

		if (typeDesc.IsAbstract)
		{
			constructor.Attributes |= MemberAttributes.Abstract;
			codeClass.TypeAttributes |= TypeAttributes.Abstract;
		}

		var members = mapping.Members;

		foreach (var member in members)
			ExportMember(codeClass, member, mapping.Namespace, constructor);

		foreach (var member in members)
		{
			foreach (var element in member.Elements)
				EnsureTypeExported(element, mapping.Namespace);

			EnsureTypeExported(member.Attribute, mapping.Namespace);
			EnsureTypeExported(member.Text, mapping.Namespace);
		}

		if (mapping.BaseMapping != null)
			ExportType(mapping.BaseMapping, null, mapping.Namespace, null, false);

		ExportDerivedStructs(mapping);

		if (constructor.Statements.Count == 0)
			codeClass.Members.Remove(constructor);

		return codeClass;
	}

	private void ExportDerivedStructs(MappingModel mapping)
	{
		for (var derived = mapping.DerivedMappings; derived != null; derived = derived.NextDerivedMapping)
			ExportType(derived, null, mapping.Namespace, null, true);
	}

	private void EnsureTypeExported(MappingModel accessor, string ns)
	{
		if (accessor != null)
			ExportType(accessor.Mapping, null, ns, null, false);
	}

	private void ExportMember(CodeTypeDeclaration codeClass, MappingModel member, string ns, CodeConstructor constructor)
	{
		var field = MakeField(member.TypeDesc.FullName, member.Name);
		codeClass.Members.Add(field);
		AddMemberMetadata(field, member, ns, constructor);

		if (!member.CheckSpecified)
			return;

		var specified = MakeField(typeof (bool).FullName, member.Name + "Specified");
		specified.CustomAttributes.Add(new CodeAttributeDeclaration(typeof (XmlIgnoreAttribute).FullName));
		codeClass.Members.Add(specified);
	}

	private static CodeMemberField MakeField(string type, string name)
	{
		var field = new CodeMemberField(type, name);
		field.Attributes = (field.Attributes & ~MemberAttributes.AccessMask) | MemberAttributes.Public;
		field.Comments.Add(new CodeCommentStatement(Remarks, true));

		return field;
	}

	private static void AddMemberMetadata(CodeMemberField field, MappingModel member, string ns, CodeConstructor constructor)
	{
		if (member.Xmlns != null)
			throw new NotSupportedException("Namespace declaration members are not supported.");

		var memberTypeDesc = member.TypeDesc;
		var attribute = member.Attribute;

		if (attribute != null)
		{
			var mapping = attribute.Mapping;
			var name = MappingModel.UnescapeName(attribute.Name);
			var qualified = attribute.Form == XmlSchemaForm.Qualified;

			ExportMetadata(field.CustomAttributes, typeof (XmlAttributeAttribute), name == member.Name ? null : name,
				qualified && attribute.Namespace != ns ? attribute.Namespace : null, mapping.TypeDesc, memberTypeDesc, false,
				qualified ? XmlSchemaForm.Qualified : XmlSchemaForm.None);

			AddDefaultValue(field, attribute, mapping, memberTypeDesc, constructor);
			return;
		}

		if (member.Text != null)
			ExportText(field.CustomAttributes, member.Text.Mapping.TypeDesc, memberTypeDesc);

		var elements = member.Elements;
		// A choice holds the elements of every branch, and each of them needs its own type.
		var isChoice = elements.Length > 1;

		foreach (var element in elements)
		{
			var mapping = element.Mapping;
			var name = MappingModel.UnescapeName(element.Name);
			var unqualified = element.Form == XmlSchemaForm.Unqualified;

			ExportMetadata(field.CustomAttributes, typeof (XmlElementAttribute),
				!isChoice && name == member.Name && !memberTypeDesc.IsArrayLike ? null : name,
				element.Namespace == ns ? null : element.Namespace, mapping.TypeDesc, isChoice ? null : memberTypeDesc,
				element.IsNullable, unqualified ? XmlSchemaForm.Unqualified : XmlSchemaForm.None);

			if (!isChoice)
				AddDefaultValue(field, element, mapping, memberTypeDesc, constructor);
		}

		if (member.Ignore)
			field.CustomAttributes.Add(new CodeAttributeDeclaration(typeof (XmlIgnoreAttribute).FullName));
	}

	private static void ExportText(CodeAttributeDeclarationCollection metadata, MappingModel typeDesc, MappingModel memberTypeDesc)
	{
		var attribute = new CodeAttributeDeclaration(typeof (XmlTextAttribute).FullName);

		if (!IsSameType(typeDesc, memberTypeDesc))
			attribute.Arguments.Add(new CodeAttributeArgument(new CodeTypeOfExpression(typeDesc.FullName)));

		if (typeDesc.IsAmbiguousDataType)
			attribute.Arguments.Add(new CodeAttributeArgument("DataType", new CodePrimitiveExpression(typeDesc.DataType.Name)));

		metadata.Add(attribute);
	}

	/// <summary>Adds the attribute which tells how one member is read and written.</summary>
	/// <param name="memberTypeDesc">
	/// The type of the member, or <see langword="null" /> to always write the type of the value.
	/// </param>
	private static void ExportMetadata(CodeAttributeDeclarationCollection metadata, Type attributeType, string name, string ns,
		MappingModel typeDesc, MappingModel memberTypeDesc, bool isNullable, XmlSchemaForm form)
	{
		var attribute = new CodeAttributeDeclaration(attributeType.FullName);

		if (name != null)
			attribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(name)));

		if (memberTypeDesc == null || !IsSameType(typeDesc, memberTypeDesc))
			attribute.Arguments.Add(new CodeAttributeArgument(new CodeTypeOfExpression(isNullable && typeDesc.IsValueType
				? "System.Nullable`1[" + typeDesc.FullName + "]"
				: typeDesc.FullName)));

		if (form != XmlSchemaForm.None)
			attribute.Arguments.Add(new CodeAttributeArgument("Form",
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof (XmlSchemaForm).FullName), form.ToString())));

		if (ns != null)
			attribute.Arguments.Add(new CodeAttributeArgument("Namespace", new CodePrimitiveExpression(ns)));

		if (typeDesc.IsAmbiguousDataType)
			attribute.Arguments.Add(new CodeAttributeArgument("DataType", new CodePrimitiveExpression(typeDesc.DataType.Name)));

		if (isNullable && !typeDesc.IsValueType)
			attribute.Arguments.Add(new CodeAttributeArgument("IsNullable", new CodePrimitiveExpression(true)));

		// An element attribute which says nothing has no use.
		if (attribute.Arguments.Count > 0 || attributeType != typeof (XmlElementAttribute))
			metadata.Add(attribute);
	}

	private static void AddTypeMetadata(CodeAttributeDeclarationCollection metadata, MappingModel mapping)
	{
		var attribute = new CodeAttributeDeclaration(typeof (XmlTypeAttribute).FullName);
		var name = MappingModel.UnescapeName(mapping.TypeName);

		if (string.IsNullOrEmpty(name))
			attribute.Arguments.Add(new CodeAttributeArgument("AnonymousType", new CodePrimitiveExpression(true)));
		else if (mapping.TypeDesc.Name != name)
			attribute.Arguments.Add(new CodeAttributeArgument("TypeName", new CodePrimitiveExpression(name)));

		if (!string.IsNullOrEmpty(mapping.Namespace))
			attribute.Arguments.Add(new CodeAttributeArgument("Namespace", new CodePrimitiveExpression(mapping.Namespace)));

		if (!mapping.IncludeInSchema)
			attribute.Arguments.Add(new CodeAttributeArgument("IncludeInSchema", new CodePrimitiveExpression(false)));

		if (attribute.Arguments.Count > 0)
			metadata.Add(attribute);
	}

	private static void AddRootMetadata(CodeAttributeDeclarationCollection metadata, MappingModel mapping, string name, string ns,
		MappingModel rootElement)
	{
		var rootAttributeName = typeof (XmlRootAttribute).FullName;

		// Only one root attribute is allowed.
		foreach (CodeAttributeDeclaration declaration in metadata)
			if (declaration.Name == rootAttributeName)
				return;

		var attribute = new CodeAttributeDeclaration(rootAttributeName);

		if (mapping.TypeDesc.Name != name)
			attribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(name)));

		if (ns != null)
			attribute.Arguments.Add(new CodeAttributeArgument("Namespace", new CodePrimitiveExpression(ns)));

		if (mapping.TypeDesc.IsAmbiguousDataType)
			attribute.Arguments.Add(new CodeAttributeArgument("DataType", new CodePrimitiveExpression(mapping.TypeDesc.DataType.Name)));

		attribute.Arguments.Add(new CodeAttributeArgument("IsNullable", new CodePrimitiveExpression(rootElement.IsNullable)));
		metadata.Add(attribute);
	}

	private static void AddDefaultValue(CodeMemberField field, MappingModel accessor, MappingModel mapping, MappingModel memberTypeDesc,
		CodeConstructor constructor)
	{
		var defaultValue = (string) accessor.Default;

		if (defaultValue == null || !memberTypeDesc.HasDefaultSupport || memberTypeDesc.IsArrayLike)
			return;

		var value = MakeValueExpression(mapping, defaultValue);

		constructor.Statements.Add(new CodeAssignStatement(
			new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name), value));

		if (accessor.IsOptional && !accessor.IsFixed)
			field.CustomAttributes.Add(new CodeAttributeDeclaration(typeof (DefaultValueAttribute).FullName,
				new CodeAttributeArgument(value)));
	}

	private static CodeExpression MakeValueExpression(MappingModel mapping, string defaultValue)
	{
		var typeDesc = mapping.TypeDesc;

		if (mapping.Is("EnumMapping"))
		{
			foreach (var constant in mapping.Constants)
				if (constant.XmlName == defaultValue)
					return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeDesc.FullName), constant.Name);

			throw new InvalidOperationException($"'{defaultValue}' is not a value of the {typeDesc.FullName} type.");
		}

		// Only the types which a default value attribute can hold without a cast are supported.
		return typeDesc.FormatterName switch
		{
			"String" => new CodePrimitiveExpression(defaultValue),
			"Boolean" => new CodePrimitiveExpression(XmlConvert.ToBoolean(defaultValue)),
			"Int32" => new CodePrimitiveExpression(XmlConvert.ToInt32(defaultValue)),
			"Double" => new CodePrimitiveExpression(XmlConvert.ToDouble(defaultValue)),
			_ => throw new NotSupportedException($"A default value of the {typeDesc.FullName} type is not supported."),
		};
	}

	private static bool IsSameType(MappingModel typeDesc, MappingModel memberTypeDesc)
	{
		return typeDesc.Equals(memberTypeDesc) ||
			memberTypeDesc.IsArrayLike && typeDesc.Equals(memberTypeDesc.ArrayElementTypeDesc);
	}

	private CodeAttributeDeclaration GeneratedCodeAttribute
	{
		get
		{
			if (generatedCodeAttribute == null)
			{
				var assembly = Assembly.GetEntryAssembly() ?? typeof (XmlCodeExporter).Assembly;
				var name = assembly.GetName();
				var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

				generatedCodeAttribute = new CodeAttributeDeclaration(typeof (GeneratedCodeAttribute).FullName,
					new CodeAttributeArgument(new CodePrimitiveExpression(name.Name)),
					new CodeAttributeArgument(new CodePrimitiveExpression(version?.InformationalVersion ?? name.Version.ToString())));
			}

			return generatedCodeAttribute;
		}
	}
}
#endif
