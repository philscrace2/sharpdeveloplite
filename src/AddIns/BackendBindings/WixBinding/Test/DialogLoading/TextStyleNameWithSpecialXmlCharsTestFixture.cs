// <file>
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
	[TestFixture]
	public class TextStyleNameWithSpecialXmlCharsTestFixture : DialogLoadingTestFixtureBase
	{
		[Test]
		public void CreateDialog()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.GetDialog("WelcomeDialog");
			using (Form dialog = wixDialog.CreateDialog(this)) {
			}
		}
				
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Property Id='BigFont'>{&amp;BigFont'Style}</Property>\r\n" +
				"\t\t\t<TextStyle Id=\"BigFont'Style\" FaceName='Verdana' Size='13' Bold='yes' />\r\n" +
				"\t\t\t<Property Id='SmallFont'>{\\SmallFontStyle}</Property>\r\n" +
				"\t\t\t<TextStyle Id='SmallFontStyle' FaceName='Arial' Size='10'/>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Title' Type='Text' X='135' Y='20' Width='220' Height='60' Transparent='yes' NoPrefix='yes'>\r\n" +
				"\t\t\t\t\t<Text>[BigFont]Welcome to the [ProductName] installation</Text>\r\n" +
				"\t\t\t\t</Control>\r\n" +
				"\t\t\t\t<Control Id='Description' Type='Text' X='135' Y='20' Width='220' Height='60' Transparent='yes' NoPrefix='yes'>\r\n" +
				"\t\t\t\t\t<Text>[SmallFont]Install text...</Text>\r\n" +
				"\t\t\t\t</Control>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
