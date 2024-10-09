using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoApp.Commands
{
    public class MainMenuNewCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            MessageBox.Show("Hello World!", "Caption");
        }
    }
}
