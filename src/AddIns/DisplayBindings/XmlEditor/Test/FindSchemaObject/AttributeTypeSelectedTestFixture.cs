﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml.Schema;
using XmlEditor.Tests.Schema;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.FindSchemaObject
{
	/// <summary>
	/// Tests that a xs:attribute/@type can be located in the schema.
	/// </summary>
	[TestFixture]
	public class AttributeTypeSelectedTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaSimpleType schemaSimpleType;
		
		public override void FixtureInit()
		{
			XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
			schemas.Add(SchemaCompletionData);
			XmlSchemaCompletionData xsdSchemaCompletionData = new XmlSchemaCompletionData(ResourceManager.GetXsdSchema());
			schemas.Add(xsdSchemaCompletionData);
			XmlCompletionDataProvider provider = new XmlCompletionDataProvider(schemas, xsdSchemaCompletionData, String.Empty);

			string xml = GetSchema();
			int index = xml.IndexOf("type=\"dir\"/>");
			index = xml.IndexOf("dir", index);
			schemaSimpleType = (XmlSchemaSimpleType)XmlView.GetSchemaObjectSelected(xml, index, provider, SchemaCompletionData);
		}
		
		[Test]
		public void SimpleTypeName()
		{
			Assert.AreEqual("dir", schemaSimpleType.QualifiedName.Name);
		}		
		
		protected override string GetSchema()
		{
			return "<xs:schema version=\"1.0\" xml:lang=\"en\"\r\n" +
					"    xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"\r\n" +
					"    targetNamespace=\"http://foo/xhtml\"\r\n" +
					"    xmlns=\"http://foo/xhtml\"\r\n" +
					"    elementFormDefault=\"qualified\">\r\n" +
					"  <xs:element name=\"html\">\r\n" +
					"    <xs:complexType>\r\n" +
					"      <xs:sequence>\r\n" +
					"        <xs:element ref=\"head\"/>\r\n" +
					"        <xs:element ref=\"body\"/>\r\n" +
					"      </xs:sequence>\r\n" +
					"      <xs:attributeGroup ref=\"i18n\"/>\r\n" +
					"      <xs:attribute name=\"id\" type=\"dir\"/>\r\n" +
					"    </xs:complexType>\r\n" +
					"  </xs:element>\r\n" +
					"\r\n" +
					"  <xs:element name=\"head\" type=\"xs:string\"/>\r\n" +
					"  <xs:element name=\"body\" type=\"xs:string\"/>\r\n" +
					"\r\n" +
					"      <xs:simpleType name=\"dir\">\r\n" +
					"        <xs:restriction base=\"xs:token\">\r\n" +
					"          <xs:enumeration value=\"ltr\"/>\r\n" +
					"          <xs:enumeration value=\"rtl\"/>\r\n" +
					"        </xs:restriction>\r\n" +
					"      </xs:simpleType>\r\n" +
					"</xs:schema>";
		}
	}
}
