﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Windows.Forms;
using System.Xml;
using WixBinding;

namespace WixBinding.Tests.DialogLoading
{
	/// <summary>
	/// Tests the dialog title is correctly set in the form created by the 
	/// WixDesignerLoader.
	/// </summary>
	[TestFixture]
	public class DialogTitleTestFixture
	{
		string dialogTitle;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.GetDialog("WelcomeDialog");
			using (Form dialog = wixDialog.CreateDialog()) {
				dialogTitle = dialog.Text;
			}
		}
		
		[Test]
		public void Title()
		{
			Assert.AreEqual("Welcome Dialog Title", dialogTitle);
		}
				
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Title='Welcome Dialog Title' Height='100' Width='200'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
