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

namespace WixBinding.Tests.DialogXmlGeneration
{
	/// <summary>
	/// One list item is removed from the list view.
	/// </summary>
	[TestFixture]
	public class ListViewItemRemovedTestFixture : DialogLoadingTestFixtureBase
	{
		string item1Text;
		int itemCount;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			CreatedComponents.Clear();
			WixDialog wixDialog = doc.GetDialog("WelcomeDialog");
			using (Form dialog = wixDialog.CreateDialog(this)) {

				ListView listView = (ListView)dialog.Controls[0];
				listView.Items.RemoveAt(0);
			
				XmlElement dialogElement = wixDialog.UpdateDialogElement(dialog);
				XmlElement listViewElement = (XmlElement)dialogElement.SelectSingleNode("//w:ListView[@Property='ListViewProperty']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				itemCount = listViewElement.ChildNodes.Count;
				
				XmlElement item1Element = (XmlElement)listViewElement.ChildNodes[0];
				item1Text = item1Element.GetAttribute("Text");
			}
		}

		[Test]
		public void OneListItem()
		{
			Assert.AreEqual(1, itemCount);
		}
		
		[Test]
		public void ListViewItem1Text()
		{
			Assert.AreEqual("second", item1Text);
		}

		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='ListView1' Type='ListView' X='20' Y='187' Width='330' Height='40' Property='ListViewProperty'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t\t<ListView Property='ListViewProperty'>\r\n" +
				"\t\t\t\t<ListItem Text='first'/>\r\n" +
				"\t\t\t\t<ListItem Text='second'/>\r\n" +
				"\t\t\t</ListView>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
