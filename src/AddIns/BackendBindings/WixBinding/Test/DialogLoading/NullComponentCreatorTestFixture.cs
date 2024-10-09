﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using WixBinding;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.DialogLoading
{
	/// <summary>
	/// Tests that an argument exception is thrown if we pass in null for the
	/// component creator into the WixDialog.CreateDialog method.
	/// </summary>
	[TestFixture]
	public class NullComponentCreatorTestFixture
	{
		[Test]
		public void CreateDialog()
		{
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			WixDocument doc = new WixDocument(project);
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.GetDialog("WelcomeDialog");
			try {
				wixDialog.CreateDialog(null);
				Assert.Fail("Expected an ArgumentException");
			} catch (ArgumentException ex) {
				Assert.AreEqual("componentCreator", ex.ParamName);
			}
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
