using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSharpCode.Core.WinForms;
using System.Collections;

namespace DemoApp.Gui.Workbench
{
    public partial class DefaultWorkbench : Form, IWorkbench
    {
        readonly static string mainMenuPath = "/SharpDevelop/Workbench/MainMenu";
        IWorkbenchLayout layout = null;

        bool closeAll = false;

        IViewContent activeViewContent;

        IWorkbenchWindow activeWorkbenchWindow;

        object activeContent;

        public DefaultWorkbench()
        {
            InitializeComponent();
        }

        public Form MainForm
        {
            get { return this; }
        }

        public string Title
        {
            get
            {
                return Text;
            }
            set
            {
                Text = value;
            }
        }

        public object ActiveContent
        {
            get
            {
                return activeContent;
            }
            private set
            {
                if (activeContent != value)
                {
                    activeContent = value;
                    if (ActiveContentChanged != null)
                    {
                        ActiveContentChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public bool IsActiveWindow => throw new NotImplementedException();

        public event EventHandler ActiveWorkbenchWindowChanged;
        public event EventHandler ActiveViewContentChanged;
        public event EventHandler ActiveContentChanged;
        public event KeyEventHandler ProcessCommandKey;

        public MenuStrip TopMenu = null;

        public void Initialize()
        {
            //UpdateRenderer();

            //MenuComplete += new EventHandler(SetStandardStatusBar);

            //SetStandardStatusBar(null, null);

            //ProjectService.CurrentProjectChanged += new ProjectEventHandler(SetProjectTitle);

            //FileService.FileRemoved += CheckRemovedOrReplacedFile;
            //FileService.FileReplaced += CheckRemovedOrReplacedFile;
            //FileService.FileRenamed += CheckRenamedFile;

            //FileService.FileRemoved += FileService.RecentOpen.FileRemoved;
            //FileService.FileRenamed += FileService.RecentOpen.FileRenamed;

            //try
            //{
            //    ArrayList contents = AddInTree.GetTreeNode(viewContentPath).BuildChildItems(this);
            //    foreach (PadDescriptor content in contents)
            //    {
            //        if (content != null)
            //        {
            //            ShowPad(content);
            //        }
            //    }
            //}
            //catch (TreePathNotFoundException) { }

            CreateMainMenu();
            //CreateToolBars();

            //toolbarUpdateTimer = new System.Windows.Forms.Timer();
            //toolbarUpdateTimer.Tick += new EventHandler(UpdateMenu);

            //toolbarUpdateTimer.Interval = 500;
            //toolbarUpdateTimer.Start();

            RightToLeftConverter.Convert(this);
        }

        public void CloseAllViews()
        {
            throw new NotImplementedException();
        }

        public void RedrawAllComponents()
        {
            throw new NotImplementedException();
        }

        public void UpdateRenderer()
        {
            throw new NotImplementedException();
        }

        void CreateMainMenu()
        {
            TopMenu = new MenuStrip();
            TopMenu.Items.Clear();
            try
            {
                MenuService.AddItemsToMenu(TopMenu.Items, this, mainMenuPath);
                UpdateMenus();
            }
            catch (TreePathNotFoundException) { }
        }

        void UpdateMenu(object sender, EventArgs e)
        {
            UpdateMenus();
            //UpdateToolbars();
        }

        void UpdateMenus()
        {
            // update menu
            foreach (object o in TopMenu.Items)
            {
                if (o is IStatusUpdate)
                {
                    ((IStatusUpdate)o).UpdateStatus();
                }
            }
        }

        public IWorkbenchLayout WorkbenchLayout
        {
            get
            {
                return layout;
            }
            set
            {
                if (layout != null)
                {
                    layout.ActiveWorkbenchWindowChanged -= OnActiveWindowChanged;
                    layout.Detach();
                }
                value.Attach(this);
                layout = value;
                layout.ActiveWorkbenchWindowChanged += OnActiveWindowChanged;
                OnActiveWindowChanged(null, null);
            }
        }

        void OnActiveWindowChanged(object sender, EventArgs e)
        {
            if (closeAll) return;

            if (layout == null)
            {
                this.ActiveWorkbenchWindow = null;
                this.ActiveContent = null;
            }
            else
            {
                this.ActiveWorkbenchWindow = layout.ActiveWorkbenchWindow;
                this.ActiveContent = layout.ActiveContent;
            }
        }

        public IWorkbenchWindow ActiveWorkbenchWindow
        {
            get
            {
                WorkbenchSingleton.AssertMainThread();
                return activeWorkbenchWindow;
            }
            private set
            {
                if (activeWorkbenchWindow != value)
                {
                    if (activeWorkbenchWindow != null)
                    {
                        activeWorkbenchWindow.ActiveViewContentChanged -= OnWorkbenchActiveViewContentChanged;
                    }
                    activeWorkbenchWindow = value;
                    if (activeWorkbenchWindow != null)
                    {
                        activeWorkbenchWindow.ActiveViewContentChanged += OnWorkbenchActiveViewContentChanged;
                    }

                    if (ActiveWorkbenchWindowChanged != null)
                    {
                        ActiveWorkbenchWindowChanged(this, EventArgs.Empty);
                    }

                    OnWorkbenchActiveViewContentChanged(null, null);
                }
            }
        }

        /// <summary>
		/// The active view content inside the active workbench window.
		/// </summary>
		public IViewContent ActiveViewContent
        {
            get
            {
                WorkbenchSingleton.AssertMainThread();
                return activeViewContent;
            }
            private set
            {
                if (activeViewContent != value)
                {
                    activeViewContent = value;
                    if (ActiveViewContentChanged != null)
                    {
                        ActiveViewContentChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        void OnWorkbenchActiveViewContentChanged(object sender, EventArgs e)
        {
            // update ActiveViewContent
            IWorkbenchWindow window = this.ActiveWorkbenchWindow;
            if (window != null)
                this.ActiveViewContent = window.ActiveViewContent;
            else
                this.ActiveViewContent = null;

            this.ActiveContent = layout.ActiveContent;
        }


    }
}
