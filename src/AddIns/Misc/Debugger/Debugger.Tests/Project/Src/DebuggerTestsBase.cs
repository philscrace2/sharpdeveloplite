// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

using Debugger;
using Debugger.Interop;
using Microsoft.CSharp;
using NUnit.Framework;
using SRPropertyInfo = System.Reflection.PropertyInfo;

namespace Debugger.Tests
{
	public class DebuggerTestsBase
	{
		protected NDebugger   debugger;
		protected Process     process;
		protected string      log;
		protected string      lastLogMessage;
		protected string      testName;
		protected XmlDocument testDoc;
		protected XmlElement  testNode;
		protected XmlElement  snapshotNode;
		protected int         shapshotID;
		
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			debugger = new NDebugger();
			debugger.MTA2STA.CallMethod = CallMethod.Manual;
		}
		
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			
		}
		
		[SetUp]
		public virtual void SetUp()
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			
			testName = null;
			
			expandProperties = new List<string>();
			
			testDoc = new XmlDocument();
			testDoc.AppendChild(testDoc.CreateXmlDeclaration("1.0","utf-8",null));
			testDoc.AppendChild(testDoc.CreateElement("DebuggerTests"));
			testNode = testDoc.CreateElement("Test");
			testDoc.DocumentElement.AppendChild(testNode);
		}
		
		[TearDown]
		public virtual void TearDown()
		{
			
		}
		
		protected void EndTest()
		{
			if (!process.HasExited) {
				process.AsyncContinue();
				process.WaitForExit();
			}
			CheckXmlOutput();
		}
		
		protected void CheckXmlOutput()
		{
			string startMark = "#if EXPECTED_OUTPUT\r\n";
			string endMark = "#endif // EXPECTED_OUTPUT";
			
			MemoryStream newXmlStream = new MemoryStream();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = Encoding.UTF8;
			settings.Indent = true;
			settings.NewLineOnAttributes = true;
			XmlWriter writer = XmlTextWriter.Create(newXmlStream, settings);
			testDoc.Save(writer);
			newXmlStream.Seek(0, SeekOrigin.Begin);
			string actualXml = new StreamReader(newXmlStream).ReadToEnd() + "\r\n";
			
			string sourceCode = GetResource(testName);
			int startIndex = sourceCode.IndexOf(startMark);
			int endIndex = sourceCode.IndexOf(endMark);
			if (startIndex == -1 || endIndex == -1) {
				Assert.Fail("Test " + testName + " failed.  Expected XML output not found.");
			}
			string expectedXml = sourceCode.Substring(startIndex + startMark.Length, endIndex - (startIndex + startMark.Length));
			
			if (actualXml != expectedXml) {
				// Update the source code file with the new output
				string path = Environment.GetEnvironmentVariable("SD_TESTS_DEBUGGER_XML_OUT");
				if (path != null) {
					string filename = Path.Combine(path, testName);
					string newSourceCode = File.ReadAllText(filename, Encoding.UTF8);
					startIndex = newSourceCode.IndexOf(startMark);
					endIndex = newSourceCode.IndexOf(endMark);
					newSourceCode = 
						newSourceCode.Substring(0, startIndex + startMark.Length) +
						actualXml +
						newSourceCode.Substring(endIndex);
					File.WriteAllText(filename, newSourceCode, Encoding.UTF8);
				}
				
				//Assert.Fail("Test " + testName + " failed.  XML output differs from expected.");
				Assert.AreEqual(expectedXml, actualXml, "Test " + testName + " failed.  XML output differs from expected.");
			}
		}
		
		protected void StartTest(string testName)
		{
			StartTest(testName, true);
		}
		
		protected void StartTest(string testName, bool wait)
		{
			this.testName = testName;
			string exeFilename = CompileTest(testName);
			
			testNode.SetAttribute("name", testName);
			shapshotID = 0;
			
			log = "";
			lastLogMessage = null;
			process = debugger.Start(exeFilename, Path.GetDirectoryName(exeFilename), testName);
			process.LogMessage += delegate(object sender, MessageEventArgs e) {
				log += e.Message;
				lastLogMessage = e.Message;
				LogEvent("LogMessage", e.Message.Replace("\r",@"\r").Replace("\n",@"\n"));
			};
			process.ModuleLoaded += delegate(object sender, ModuleEventArgs e) {
				LogEvent("ModuleLoaded", e.Module.Filename + (e.Module.HasSymbols ? " (Has symbols)" : " (No symbols)"));
			};
			process.Paused += delegate(object sender, ProcessEventArgs e) {
				LogEvent("DebuggingPaused", e.Process.PauseSession.PausedReason.ToString() + " " + e.Process.SelectedStackFrame.NextStatement.ToString());
			};
//			process.DebuggingResumed += delegate(object sender, ProcessEventArgs e) {
//				LogEvent("DebuggingResumed", e.Process.PausedReason.ToString());
//			};
			process.ExceptionThrown += delegate(object sender, ExceptionEventArgs e) {
				StringBuilder msg = new StringBuilder();
				if (process.SelectedThread.InterceptCurrentException()) {
					msg.Append(e.Exception.ToString());
				} else {
					// For example, happens on stack overflow
					msg.Append("Could not intercept: ");
					msg.Append(e.Exception.ToString());
				}
				LogEvent("ExceptionThrown", msg.ToString());
			};
			process.Exited += delegate(object sender, EventArgs e) {
				LogEvent("ProcessExited", null);
			};
			
			LogEvent("ProcessStarted", null);
			
			if (wait) {
				process.WaitForPause();
			}
		}
		
		protected XmlElement LogEvent(string name, string content)
		{
			XmlElement eventNode = testDoc.CreateElement(name);
			if (content != null) {
				eventNode.AppendChild(testDoc.CreateTextNode(content));
			}
			testNode.AppendChild(eventNode);
			return eventNode;
		}
		
		public void ObjectDump(object obj)
		{
			Serialize(testNode, obj, 16, new List<object>());
		}
		
		public void ObjectDump(string name, object obj)
		{
			XmlElement dumpNode = testDoc.CreateElement(XmlConvert.EncodeName(name.Replace(" ", "_")));
			testNode.AppendChild(dumpNode);
			Serialize(dumpNode, obj, 16, new List<object>());
		}
		
		public void ObjectDumpToString(string name, object obj)
		{
			XmlElement dumpNode = testDoc.CreateElement(XmlConvert.EncodeName(name.Replace(" ", "_")));
			testNode.AppendChild(dumpNode);
			if (obj == null) {
				dumpNode.AppendChild(dumpNode.OwnerDocument.CreateTextNode("null"));
			} else {
				dumpNode.AppendChild(dumpNode.OwnerDocument.CreateTextNode(obj.ToString()));
			}
		}
		
		List<string> expandProperties;
		
		protected void ExpandProperties(params string[] props)
		{
			expandProperties = new List<string>(props);
		}
		
		bool ShouldExpandProperty(System.Reflection.PropertyInfo propertyInfo)
		{
			return
				(propertyInfo.GetCustomAttributes(typeof(Debugger.Tests.ExpandAttribute), true).Length > 0) ||
				expandProperties.Contains(propertyInfo.Name) ||
				expandProperties.Contains("*") ||
				expandProperties.Contains(propertyInfo.DeclaringType.Name + "." + propertyInfo.Name) ||
				expandProperties.Contains(propertyInfo.DeclaringType.Name + ".*");
		}
		
		public void Serialize(XmlElement container, object obj, int maxDepth, List<object> parents)
		{
			XmlDocument doc = container.OwnerDocument;
			
			if (obj == null) {
				container.AppendChild(doc.CreateTextNode("null"));
				return;
			}
			if (maxDepth == -1) {
				container.AppendChild(doc.CreateTextNode("{Max depth reached}"));
				return;
			}
			if (parents.Contains(obj)) {
				container.AppendChild(doc.CreateTextNode("{Recusion detected}"));
				return;
			}
			if (obj.GetType().Namespace == "System") {
				container.AppendChild(doc.CreateTextNode(obj.ToString()));
				return;
			}
			
			parents = new List<object>(parents); // Clone
			parents.Add(obj);
			
			Type type = obj.GetType();
			
			if (!(obj is IEnumerable)) {
				XmlElement newContainer = doc.CreateElement(XmlConvert.EncodeName(type.Name));
				container.AppendChild(newContainer);
				container = newContainer;
			}
			
			List<SRPropertyInfo> properties = new List<SRPropertyInfo>();
			properties.AddRange(type.GetProperties());
			properties.Sort(delegate(SRPropertyInfo a, SRPropertyInfo b) { return a.Name.CompareTo(b.Name);});
			
			foreach(SRPropertyInfo property in properties) {
				if (type.BaseType == typeof(Array)) continue;
				if (property.GetGetMethod() == null) continue;
				if (property.GetGetMethod().GetParameters().Length > 0) continue;
				if (property.GetCustomAttributes(typeof(Debugger.Tests.IgnoreAttribute), true).Length > 0) continue;
				
				object val;
				try {
					val = property.GetValue(obj, new object[] {});
				} catch (System.Exception e) {
					while(e.InnerException != null) e = e.InnerException;
					val = "{Exception: " + e.Message + "}";
				}
				if (val == null) val = "null";
				if (val is IEnumerable && !(val is string)) {
					List<string> vals = new List<string>();
					foreach(object o in (IEnumerable)val) {
						vals.Add(o.ToString());
					}
					container.SetAttribute(property.Name, "{" + string.Join(", ", vals.ToArray()) + "}");
				} else {
					container.SetAttribute(property.Name, val.ToString());
				}
				
				
				if (ShouldExpandProperty(property)) {
					XmlElement propertyNode = doc.CreateElement(property.Name);
					container.AppendChild(propertyNode);
					Serialize(propertyNode, val, maxDepth - 1, parents);
				}
			}
			
			// Save all objects of an enumerable object
			if (obj is IEnumerable) {
				int id = 1;
				foreach(object enumObject in (IEnumerable)obj) {
					XmlElement enumRoot = doc.CreateElement("Item");
					container.AppendChild(enumRoot);
					Serialize(enumRoot, enumObject, maxDepth - 1, parents);
					id++;
				}
			}
		}
		
		string GetResource(string filename)
		{
			string resourcePrefix = "Debugger.Tests.Src.TestPrograms.";
			
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePrefix + filename);
			if (stream == null) throw new System.Exception("Resource " + filename + " not found");
			return new StreamReader(stream).ReadToEnd();
		}
		
		string CompileTest(string testName)
		{
			string code = GetResource(testName);
			
			string md5 = ToHexadecimal(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(code)));
			
			string path = Path.GetTempPath();
			path = Path.Combine(path, "SharpDevelop");
			path = Path.Combine(path, "DebuggerTestsX86");
			path = Path.Combine(path, testName + "." + md5);
			Directory.CreateDirectory(path);
			
			string codeFilename = Path.Combine(path, testName);
			string exeFilename = Path.Combine(path, testName.Replace(".cs", ".exe"));
			
			if (File.Exists(exeFilename)) {
				return exeFilename;
			}
			
			StreamWriter file = new StreamWriter(codeFilename);
			file.Write(code);
			file.Close();
			
			CompilerParameters compParams = new CompilerParameters();
			compParams.GenerateExecutable = true;
			compParams.GenerateInMemory = false;
			compParams.TreatWarningsAsErrors = false;
			compParams.IncludeDebugInformation = true;
			compParams.ReferencedAssemblies.Add("System.dll");
			compParams.OutputAssembly = exeFilename;
			compParams.CompilerOptions = "/unsafe /platform:x86";
			compParams.ReferencedAssemblies.Add(typeof(TestFixtureAttribute).Assembly.Location);
			
			CSharpCodeProvider compiler = new CSharpCodeProvider();
			CompilerResults result = compiler.CompileAssemblyFromFile(compParams, codeFilename);
			
			if (result.Errors.Count > 0) {
				throw new System.Exception("There was an error(s) during compilation of test program:\n" + result.Errors[0].ToString());
			}
			
			return exeFilename;
		}
		
		string CopyThisAssembly()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			string md5 = ToHexadecimal(new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(assembly.Location)));
			
			string exeName = Path.GetFileName(assembly.Location);
			string pdbName = Path.GetFileNameWithoutExtension(assembly.Location) + ".pdb";
			
			string oldDir = Path.GetDirectoryName(assembly.Location);
			
			string newDir = Path.GetTempPath();
			newDir = Path.Combine(newDir, "SharpDevelop3.0");
			newDir = Path.Combine(newDir, "DebuggerTests");
			newDir = Path.Combine(newDir, md5);
			Directory.CreateDirectory(newDir);
			
			if (!File.Exists(Path.Combine(newDir, exeName))) {
				File.Copy(Path.Combine(oldDir, exeName), Path.Combine(newDir, exeName));
			}
			
			if (!File.Exists(Path.Combine(newDir, pdbName))) {
				File.Copy(Path.Combine(oldDir, pdbName), Path.Combine(newDir, pdbName));
			}
			
			return Path.Combine(newDir, exeName);
		}
		
		void CopyPdb()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			string dir = Path.GetDirectoryName(assembly.Location);
			string iniFilePath = Path.Combine(dir, "__AssemblyInfo__.ini");
			string iniFileContent = File.ReadAllText(iniFilePath, Encoding.Unicode);
			
			string originalExePath = iniFileContent.Remove(0, iniFileContent.IndexOf("file:///") + "file:///".Length).TrimEnd(' ', '\0');
			string originalDir = Path.GetDirectoryName(originalExePath);
			string originalPdbPath = Path.Combine(originalDir, Path.GetFileNameWithoutExtension(originalExePath) + ".pdb");
			
			string pdbPath = Path.Combine(dir, Path.GetFileNameWithoutExtension(originalExePath) + ".pdb");
			
			if (!File.Exists(pdbPath)) {
				File.Copy(originalPdbPath, pdbPath);
			}
		}
		
		static string ToHexadecimal(byte[] bytes)
		{
			char[] chars = new char[] {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F',};
			string hex = "";
			foreach(byte b in bytes) {
				hex += chars[b >> 4];
				hex += chars[b & 0x0F];
			}
			return hex;
		}
		
		public static void Main(string[] args)
		{
			if (args.Length != 1) throw new System.Exception("Needs test name as argument");
			string testName = args[0];
			Type type = Type.GetType(testName);
			type.GetMethod("Main").Invoke(null, new object[0]);
		}
	}
}
