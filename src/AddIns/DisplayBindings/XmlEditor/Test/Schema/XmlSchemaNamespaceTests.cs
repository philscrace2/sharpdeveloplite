﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests that the standard W3C namespace for XSD files is recognised.
	/// </summary>
	[TestFixture]
	public class XmlSchemaNamespaceTests
	{
		[Test]
		public void IsXmlSchemaNamespace()
		{
			Assert.IsTrue(XmlSchemaManager.IsXmlSchemaNamespace("http://www.w3.org/2001/XMLSchema"));
		}
		
		[Test]
		public void IsNotXmlSchemaNamespace()
		{
			Assert.IsFalse(XmlSchemaManager.IsXmlSchemaNamespace("http://foo.com"));
		}
		
		[Test]
		public void EmptyString()
		{
			Assert.IsFalse(XmlSchemaManager.IsXmlSchemaNamespace(String.Empty));
		}
	}
}
