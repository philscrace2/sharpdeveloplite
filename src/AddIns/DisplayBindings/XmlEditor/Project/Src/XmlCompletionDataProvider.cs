// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Provides the autocomplete (intellisense) data for an
	/// xml document that specifies a known schema.
	/// </summary>
	public class XmlCompletionDataProvider : AbstractCompletionDataProvider
	{
		XmlSchemaCompletionDataCollection schemaCompletionDataItems;
		XmlSchemaCompletionData defaultSchemaCompletionData;
		string defaultNamespacePrefix = String.Empty;
		
		public XmlCompletionDataProvider(XmlSchemaCompletionDataCollection schemaCompletionDataItems, XmlSchemaCompletionData defaultSchemaCompletionData, string defaultNamespacePrefix)
		{
			this.schemaCompletionDataItems = schemaCompletionDataItems;
			this.defaultSchemaCompletionData = defaultSchemaCompletionData;
			this.defaultNamespacePrefix = defaultNamespacePrefix;
			DefaultIndex = 0;
		}
		
		public override ImageList ImageList {
			get {
				return XmlCompletionDataImageList.GetImageList();
			}
		}

		/// <summary>
		/// Overrides the default behaviour and allows special xml
		/// characters such as '.' and ':' to be used as completion data.
		/// </summary>
		public override CompletionDataProviderKeyResult ProcessKey(char key)
		{
			if (key == '\r' || key == '\t') {
				return CompletionDataProviderKeyResult.InsertionKey;
			}
			return CompletionDataProviderKeyResult.NormalKey;
		}
		
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			preSelection = null;
			string text = String.Concat(textArea.Document.GetText(0, textArea.Caret.Offset), charTyped);
			
			switch (charTyped) {
				case '=':
					// Namespace intellisense.
					if (XmlParser.IsNamespaceDeclaration(text, text.Length)) {
						return schemaCompletionDataItems.GetNamespaceCompletionData();;
					}
					break;
				case '<':
					// Child element intellisense.
					XmlElementPath parentPath = XmlParser.GetParentElementPath(text);
					if (parentPath.Elements.Count > 0) {
						return GetChildElementCompletionData(parentPath);
					} else if (defaultSchemaCompletionData != null) {
						return defaultSchemaCompletionData.GetElementCompletionData(defaultNamespacePrefix);
					}
					break;
					
				case ' ':
					// Attribute intellisense.
					if (!XmlParser.IsInsideAttributeValue(text, text.Length)) {
						XmlElementPath path = XmlParser.GetActiveElementStartPath(text, text.Length);
						if (path.Elements.Count > 0) {
							return GetAttributeCompletionData(path);
						}
					}
					break;
					
				default:
					
					// Attribute value intellisense.
					if (XmlParser.IsAttributeValueChar(charTyped)) {
						string attributeName = XmlParser.GetAttributeName(text, text.Length);
						if (attributeName.Length > 0) {
							XmlElementPath elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
							if (elementPath.Elements.Count > 0) {
								preSelection = charTyped.ToString();
								return GetAttributeValueCompletionData(elementPath, attributeName);
							}
						}
					}
					break;
			}
			
			return null;
		}
		
		/// <summary>
		/// Finds the schema given the xml element path.
		/// </summary>
		public XmlSchemaCompletionData FindSchema(XmlElementPath path)
		{
			if (path.Elements.Count > 0) {
				string namespaceUri = path.Elements[0].Namespace;
				if (namespaceUri.Length > 0) {
					return schemaCompletionDataItems[namespaceUri];
				} else if (defaultSchemaCompletionData != null) {
					
					// Use the default schema namespace if none
					// specified in a xml element path, otherwise
					// we will not find any attribute or element matches
					// later.
					foreach (QualifiedName name in path.Elements) {
						if (name.Namespace.Length == 0) {
							name.Namespace = defaultSchemaCompletionData.NamespaceUri;
						}
					}
					return defaultSchemaCompletionData;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Finds the schema given a namespace URI.
		/// </summary>
		public XmlSchemaCompletionData FindSchema(string namespaceUri)
		{
			return schemaCompletionDataItems[namespaceUri];
		}
		
		/// <summary>
		/// Gets the schema completion data that was created from the specified 
		/// schema filename.
		/// </summary>
		public XmlSchemaCompletionData FindSchemaFromFileName(string fileName)
		{
			return schemaCompletionDataItems.GetSchemaFromFileName(fileName);
		}
		
		ICompletionData[] GetChildElementCompletionData(XmlElementPath path)
		{
			ICompletionData[] completionData = null;
			
			XmlSchemaCompletionData schema = FindSchema(path);
			if (schema != null) {
				completionData = schema.GetChildElementCompletionData(path);
			}
			
			return completionData;
		}
		
		ICompletionData[] GetAttributeCompletionData(XmlElementPath path)
		{
			ICompletionData[] completionData = null;
			
			XmlSchemaCompletionData schema = FindSchema(path);
			if (schema != null) {
				completionData = schema.GetAttributeCompletionData(path);
			}
			
			return completionData;
		}
		
		ICompletionData[] GetAttributeValueCompletionData(XmlElementPath path, string name)
		{
			ICompletionData[] completionData = null;
			
			XmlSchemaCompletionData schema = FindSchema(path);
			if (schema != null) {
				completionData = schema.GetAttributeValueCompletionData(path, name);
			}
			
			return completionData;
		}
	}
}
