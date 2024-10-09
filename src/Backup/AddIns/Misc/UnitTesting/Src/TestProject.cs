﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a project that has a reference to a unit testing
	/// framework assembly. Currently only NUnit is supported.
	/// </summary>
	public class TestProject
	{
		IProject project;
		IProjectContent projectContent;
		TestClassCollection testClasses;
		List<string> rootNamespaces;
		
		public TestProject(IProject project, IProjectContent projectContent)
		{
			this.project = project;
			this.projectContent = projectContent;
		}
		
		/// <summary>
		/// Returns the underlying project.
		/// </summary>
		public IProject Project {
			get { return project; }
		}
		
		/// <summary>
		/// Determines whether the project is a test project. A project
		/// is considered to be a test project if it contains a reference
		/// to the NUnit.Framework assembly.
		/// </summary>
		public static bool IsTestProject(IProject project)
		{
			if (project != null) {
				foreach (ProjectItem projectItem in project.Items) {
					ReferenceProjectItem referenceProjectItem = projectItem as ReferenceProjectItem;
					if (referenceProjectItem != null) {
						if (IsTestFrameworkReference(referenceProjectItem)) {
							return true;
						}
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// Determines whether the specified reference is a reference to
		/// a test framework. Currently only references to the
		/// NUnit.Framework return true.
		/// </summary>
		public static bool IsTestFrameworkReference(ReferenceProjectItem referenceProjectItem)
		{
			if (referenceProjectItem != null) {
				return string.Equals(referenceProjectItem.ShortName, "NUnit.Framework", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
		
		/// <summary>
		/// Gets the test classes in this project.
		/// </summary>
		public TestClassCollection TestClasses {
			get {
				if (testClasses == null) {
					GetTestClasses();
				}
				return testClasses;
			}
		}
		
		/// <summary>
		/// Gets the test classes that exist in the specified namespace.
		/// </summary>
		public TestClass[] GetTestClasses(string ns)
		{
			return TestClass.GetTestClasses(TestClasses, ns);
		}
		
		/// <summary>
		/// Gets the test classes whose namespaces start with the specified string.
		/// </summary>
		public TestClass[] GetAllTestClasses(string namespaceStartsWith)
		{
			return TestClass.GetAllTestClasses(TestClasses, namespaceStartsWith);
		}
		
		/// <summary>
		/// Gets all the child namespaces with the specified parent
		/// namespace. The parent namespace can be one or more
		/// namespaces separated with a period.
		/// </summary>
		public string[] GetChildNamespaces(string parentNamespace)
		{
			return TestClass.GetChildNamespaces(TestClasses, parentNamespace);
		}
		
		/// <summary>
		/// Gets the project's name.
		/// </summary>
		public string Name {
			get { return project.Name; }
		}
		
		/// <summary>
		/// Gets the distinct root namespaces for all this project.
		/// </summary>
		/// <remarks>
		/// If one of the namespaces is 'ICSharpCode.XmlEditor' then this
		/// method will return 'ICSharpCode' as one of the root namespaces.
		/// </remarks>
		public IList<string> RootNamespaces {
			get {
				if (rootNamespaces == null) {
					GetRootNamespaces();
				}
				return rootNamespaces;
			}
		}
		
		/// <summary>
		/// Updates the test method based on the test result.
		/// </summary>
		public void UpdateTestResult(TestResult testResult)
		{
			TestClasses.UpdateTestResult(testResult);
		}
		
		/// <summary>
		/// Sets all the test results back to none.
		/// </summary>
		public void ResetTestResults()
		{
			TestClasses.ResetTestResults();
		}
		
		/// <summary>
		/// Updates the classes and methods based on the new parse information.
		/// </summary>
		/// <param name="oldUnit">The old compiliation unit
		/// (ParseInformationEventArgs.ParseInformation.BestCompilationUnit as ICompilationUnit)</param>
		/// <param name="newUnit">The new compilation unit
		/// (ParseInformationEventArgs.CompilationUnit).</param>
		public void UpdateParseInfo(ICompilationUnit oldUnit, ICompilationUnit newUnit)
		{
			if (!IsParseInfoForThisProject(oldUnit, newUnit)) {
				return;
			}
			
			RemovedClasses removedClasses = new RemovedClasses();
			
			if (oldUnit != null) {
				removedClasses.Add(oldUnit.Classes);
			}
			if (newUnit != null) {
				foreach (IClass c in newUnit.Classes) {
					UpdateTestClass(c);
					foreach (IClass innerClass in c.InnerClasses) {
						UpdateTestClass(innerClass);
						removedClasses.Remove(innerClass);
					}
					removedClasses.Remove(c);
				}
			}
			
			// Remove missing classes.
			foreach (IClass c in removedClasses.GetMissingClasses()) {
				IClass existingClass = GetExistingTestClassInProject(c);
				if (existingClass != null) {
					UpdateTestClass(existingClass);
				} else {
					TestClasses.Remove(c.DotNetName);
				}
			}
		}
		
		/// <summary>
		/// Determines whether the new parse information is for this test
		/// project.
		/// </summary>
		public bool IsParseInfoForThisProject(ICompilationUnit oldUnit, ICompilationUnit newUnit)
		{
			ICompilationUnit unit = oldUnit;
			if (unit == null) {
				unit = newUnit;
			}
			if (unit != null) {
				return Object.ReferenceEquals(unit.ProjectContent, this.projectContent);
			}
			return false;
		}
		
		/// <summary>
		/// Adds a new class to the test project's classes only if
		/// the class is a test class.
		/// </summary>
		void AddNewTestClass(IClass c)
		{
			if (TestClass.IsTestClass(c)) {
				TestClass testClass = new TestClass(c);
				TestClasses.Add(testClass);
			}
		}
		
		/// <summary>
		/// Updates the test class methods based on the newly parsed class
		/// information.
		/// </summary>
		void UpdateTestClass(IClass c)
		{
			if (TestClasses.Contains(c.DotNetName)) {
				if (TestClass.IsTestClass(c)) {
					TestClass testClass = TestClasses[c.DotNetName];
					testClass.UpdateClass(c);
				} else {
					// TestFixture attribute has been removed so
					// remove the class from the set of TestClasses.
					TestClasses.Remove(c.DotNetName);
				}
			} else {
				// TestFixture attribute may have been recently added to
				// this class so call AddNewTestClass. No need to
				// check if the class is actually a test class since
				// AddNewTestClass does this anyway.
				AddNewTestClass(c);
			}			
		}
		
		void GetTestClasses()
		{
			testClasses = new TestClassCollection();
			foreach (IClass c in projectContent.Classes) {
				if (TestClass.IsTestClass(c)) {
					if (!testClasses.Contains(c.FullyQualifiedName)) {
						testClasses.Add(new TestClass(c));
					}
				}
				foreach (IClass innerClass in c.InnerClasses) {
					if (TestClass.IsTestClass(innerClass)) {
						if (!testClasses.Contains(innerClass.DotNetName)) {
							testClasses.Add(new TestClass(innerClass));
						}
					}
				}
			}
		}
		
		void GetRootNamespaces()
		{
			rootNamespaces = new List<string>();
			foreach (TestClass c in TestClasses) {
				string rootNamespace = c.RootNamespace;
				if (rootNamespace.Length > 0 && !rootNamespaces.Contains(rootNamespace)) {
					rootNamespaces.Add(rootNamespace);
				}
			}
		}	
		
		/// <summary>
		/// Gets an existing test class with the same name in the project. This
		/// method is used to check that we do not remove a class after an existing duplicate class name
		/// is changed.
		/// </summary>
		IClass GetExistingTestClassInProject(IClass c)
		{
			foreach (IClass existingClass in projectContent.Classes) {
				if (TestClass.IsTestClass(existingClass)) {
					if (existingClass.DotNetName == c.DotNetName) {
						return existingClass;
					}
				}
			}
			return null;
		}
	}
}
