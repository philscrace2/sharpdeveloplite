﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that the WixDocument.GetProperty returns the inner text of the 
	/// Wix Property element.
	/// </summary>
	[TestFixture]
	public class GetDocumentPropertyTestFixture
	{
		WixDocument doc;
		
		[SetUp]
		public void SetUpFixture()
		{
			doc = new WixDocument();
			doc.LoadXml(GetWixXml());
		}
		
		[Test]
		public void ButtonCancelProperty()
		{
			Assert.AreEqual("Cancel", doc.GetProperty("Button_Cancel"));
		}
		
		[Test]
		public void MissingProperty()
		{
			Assert.AreEqual(String.Empty, doc.GetProperty("MissingProperty"));
		}		
		
		[Test]
		public void PropertyWithSingleQuoteInName()
		{
			Assert.AreEqual(String.Empty, doc.GetProperty("Test'Property"));
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t\t<Property Id='Button_Cancel'>Cancel</Property>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
