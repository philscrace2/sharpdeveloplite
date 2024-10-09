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

namespace WixBinding.Tests.DialogLoading
{
	/// <summary>
	/// Tests the minimize box can be disabled.
	/// </summary>
	[TestFixture]
	public class DialogMinimizeBoxTestFixture
	{
		bool minimizeBox;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.GetDialog("WelcomeDialog");
			using (Form simpleDialog = wixDialog.CreateDialog()) {
				minimizeBox = simpleDialog.MinimizeBox;
			}
		}
		
		[Test]
		public void MinimizeBox()
		{
			Assert.IsFalse(minimizeBox);
		}
				
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370' NoMinimize='yes'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
