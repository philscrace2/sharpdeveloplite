// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace Plugins.RegExpTk {

	public class GroupForm : BaseSharpDevelopForm
	{
		public GroupForm(Match match)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.RegExpTkGroupForm.xfrm"));
			
			ListView groupsListView = (ListView)ControlDictionary["GroupsListView"];
			((Button)ControlDictionary["CloseButton"]).Click += new EventHandler(CloseButton_Click);
			foreach(Group group in match.Groups)
			{
				ListViewItem groupItem = groupsListView.Items.Add(group.Value);
				groupItem.SubItems.Add(group.Index.ToString());
				groupItem.SubItems.Add((group.Index + group.Length).ToString());
				groupItem.SubItems.Add(group.Length.ToString());
			}
		}
		
		void CloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	}

}
