﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Widgets.DesignTimeSupport;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PropertyGrid
{
	/// <summary>
	/// Tests that the DropDownEditor is specified as the editor for a
	/// WixAttributePropertyDescriptor that has possible attribute values.
	/// </summary>
	[TestFixture]
	public class DropDownPropertyDescriptorTestFixture
	{
		WixXmlAttributePropertyDescriptor propertyDescriptor;
		EditorAttribute editorAttribute;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixXmlAttribute attribute = new WixXmlAttribute("Id", WixXmlAttributeType.Text, new string[] {"a", "b"}, null);
			propertyDescriptor = new WixXmlAttributePropertyDescriptor(attribute);
			editorAttribute = WixBindingTestsHelper.GetEditorAttribute(propertyDescriptor.Attributes);
		}
		
		[Test]
		public void EditorAttributeAdded()
		{
			Assert.IsNotNull(editorAttribute);
		}
		
		[Test]
		public void EditorAttributeTypeName()
		{
			Assert.AreEqual(typeof(WixDropDownEditor).AssemblyQualifiedName, editorAttribute.EditorTypeName);
		}
	}
}
