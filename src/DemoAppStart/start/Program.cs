using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Drawing;
using System.Resources;
using ICSharpCode.Core;
using DemoApp.Sda;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.WinForms;
using System.Configuration;

namespace DemoAppStart
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Run(args);
        }

        static void Run(string[] args)
        {
            // DO NOT USE LoggingService HERE!
            // LoggingService requires ICSharpCode.Core.dll and log4net.dll
            // When a method containing a call to LoggingService is JITted, the
            // libraries are loaded.
            // We want to show the SplashScreen while those libraries are loading, so
            // don't call LoggingService.
            try
            {
                RunApplication();
            }
            finally
            {

            }
        }

        static void RunApplication()
        {
            // The output encoding differs based on whether SharpDevelop is a console app (debug mode)
            // or Windows app (release mode). Because this flag also affects the default encoding
            // when reading from other processes' standard output, we explicitly set the encoding to get
            // consistent behaviour in debug and release builds of SharpDevelop.

#if DEBUG
            // Console apps use the system's OEM codepage, windows apps the ANSI codepage.
            // We'll always use the Windows (ANSI) codepage.
            try
            {
                Console.OutputEncoding = System.Text.Encoding.Default;
            }
            catch (IOException)
            {
                // can happen if SharpDevelop doesn't have a console
            }
#endif

            LoggingService.Info("Starting SharpDevelop...");
            try
            {
                StartupSettings startup = new StartupSettings();
#if DEBUG
                startup.UseSharpDevelopErrorHandler = !Debugger.IsAttached;
#endif

                Assembly exe = typeof(Program).Assembly;
                startup.ApplicationRootPath = Path.Combine(Path.GetDirectoryName(exe.Location), "..");
                startup.AllowUserAddIns = true;

                string configDirectory = ConfigurationManager.AppSettings["settingsPath"];
                if (String.IsNullOrEmpty(configDirectory))
                {
                    startup.ConfigDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                           "ICSharpCode/SharpDevelop3.0");
                }
                else
                {
                    startup.ConfigDirectory = Path.Combine(Path.GetDirectoryName(exe.Location), configDirectory);
                }

                startup.DomPersistencePath = ConfigurationManager.AppSettings["domPersistencePath"];
                if (string.IsNullOrEmpty(startup.DomPersistencePath))
                {
                    startup.DomPersistencePath = Path.Combine(Path.GetTempPath(), "SharpDevelop" + RevisionClass.MainVersion);
#if DEBUG
                    startup.DomPersistencePath = Path.Combine(startup.DomPersistencePath, "Debug");
#endif
                }
                else if (startup.DomPersistencePath == "none")
                {
                    startup.DomPersistencePath = null;
                }

                startup.AddAddInsFromDirectory(Path.Combine(startup.ApplicationRootPath, "AddIns"));

                // allows testing addins without having to install them
                foreach (string parameter in SplashScreenForm.GetParameterList())
                {
                    if (parameter.StartsWith("addindir:", StringComparison.OrdinalIgnoreCase))
                    {
                        startup.AddAddInsFromDirectory(parameter.Substring(9));
                    }
                }

                Host host = new Host(AppDomain.CurrentDomain, startup);

                //string[] fileList = SplashScreenForm.GetRequestedFileList();
                //if (fileList.Length > 0)
                //{
                //    if (LoadFilesInPreviousInstance(fileList))
                //    {
                //        LoggingService.Info("Aborting startup, arguments will be handled by previous instance");
                //        return;
                //    }
                //}

                //host.BeforeRunWorkbench += delegate {
                //    if (SplashScreenForm.SplashScreen != null)
                //    {
                //        SplashScreenForm.SplashScreen.BeginInvoke(new MethodInvoker(SplashScreenForm.SplashScreen.Dispose));
                //        SplashScreenForm.SplashScreen = null;
                //    }
                //};

                string[] fileList = SplashScreenForm.GetRequestedFileList();

                WorkbenchSettings workbenchSettings = new WorkbenchSettings();
                workbenchSettings.RunOnNewThread = false;
                for (int i = 0; i < fileList.Length; i++)
                {
                    workbenchSettings.InitialFileList.Add(fileList[i]);
                }
                host.RunWorkbench(workbenchSettings);
            }
            finally
            {
                LoggingService.Info("Leaving RunApplication()");
            }
        }
    }
}
