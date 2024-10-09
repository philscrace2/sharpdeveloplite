﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Checks that a text node is added by the XmlTreeEditor as
	/// a child of the selected element.
	/// </summary>
	[TestFixture]
	public class AddChildTextNodeTestFixture : XmlTreeViewTestFixtureBase
	{
		XmlElement paragraphElement;
		
		[SetUp]
		public void Init()
		{
			base.InitFixture();
			paragraphElement = (XmlElement)editor.Document.SelectSingleNode("/html/body/p");
			mockXmlTreeView.SelectedElement = paragraphElement;
			editor.AppendChildTextNode();
		}
		
		[Test]
		public void ParagraphElementHasChildNodes()
		{
			Assert.AreEqual(1, paragraphElement.ChildNodes.Count);
		}
		
		[Test]
		public void NewTextNodeAdded()
		{
			XmlNode node = paragraphElement.FirstChild;
			Assert.IsInstanceOf(typeof(XmlText), node);
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void AddChildTextNodeWhenNoElementSelected()
		{
			mockXmlTreeView.SelectedElement = null;
			mockXmlTreeView.IsDirty = false;
			editor.AppendChildTextNode();
			Assert.IsFalse(mockXmlTreeView.IsDirty);
		}
		
		[Test]
		public void TextNodeAddedToView()
		{
			Assert.AreEqual(1, mockXmlTreeView.ChildTextNodesAdded.Count);
		}
		
		/// <summary>
		/// Returns the xhtml strict schema as the default schema.
		/// </summary>
		protected override XmlSchemaCompletionData DefaultSchemaCompletionData {
			get {
				XmlTextReader reader = ResourceManager.GetXhtmlStrictSchema();
				return new XmlSchemaCompletionData(reader);
			}
		}
		
		protected override string GetXml()
		{
			return "<html>\r\n" +
				"\t<head>\r\n" +
				"\t\t<title></title>\r\n" +
				"\t</head>\r\n" +
				"\t<body>\r\n" +
				"\t\t<p/>\r\n" +
				"\t</body>\r\n" +
				"</html>";
		}
	}
}
