// <file>
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

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// The collection of schemas should provide completion data for the
	/// namespaces it holds.
	/// </summary>
	[TestFixture]
	public class NamespaceCompletionTestFixture
	{
		ICompletionData[] namespaceCompletionData;
		string firstNamespace = "http://foo.com/foo.xsd";
		string secondNamespace = "http://bar.com/bar.xsd";
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			XmlSchemaCompletionDataCollection items = new XmlSchemaCompletionDataCollection();
			
			StringReader reader = new StringReader(GetSchema(firstNamespace));
			XmlSchemaCompletionData schema = new XmlSchemaCompletionData(reader);
			items.Add(schema);
			
			reader = new StringReader(GetSchema(secondNamespace));
			schema = new XmlSchemaCompletionData(reader);
			items.Add(schema);
			namespaceCompletionData = items.GetNamespaceCompletionData();
		}
		
		[Test]
		public void NamespaceCount()
		{
			Assert.AreEqual(2, namespaceCompletionData.Length,
			                "Should be 2 namespaces.");
		}
		
		[Test]
		public void ContainsFirstNamespace()
		{
			Assert.IsTrue(SchemaTestFixtureBase.Contains(namespaceCompletionData, firstNamespace));
		}
		
		[Test]
		public void ContainsSecondNamespace()
		{
			Assert.IsTrue(SchemaTestFixtureBase.Contains(namespaceCompletionData, secondNamespace));
		}		
		
		string GetSchema(string namespaceURI)
		{
			return "<?xml version=\"1.0\"?>\r\n" +
				"<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"\r\n" +
				"targetNamespace=\"" + namespaceURI + "\"\r\n" +
				"xmlns=\"" + namespaceURI + "\"\r\n" +
				"elementFormDefault=\"qualified\">\r\n" +
				"<xs:element name=\"note\">\r\n" +
				"</xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
