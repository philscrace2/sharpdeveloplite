// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Abstract base class for resource resolvers.
	/// </summary>
	public abstract class AbstractResourceResolver : IResourceResolver
	{
		
		/// <summary>
		/// Attempts to resolve a reference to a resource.
		/// </summary>
		/// <param name="fileName">The name of the file that contains the expression to be resolved.</param>
		/// <param name="document">The document that contains the expression to be resolved.</param>
		/// <param name="caretLine">The 0-based line in the file that contains the expression to be resolved.</param>
		/// <param name="caretColumn">The 0-based column position of the expression to be resolved.</param>
		/// <param name="charTyped">The character that has been typed at the caret position but is not yet in the buffer (this is used when invoked from code completion), or <c>null</c>.</param>
		/// <returns>A <see cref="ResourceResolveResult"/> that describes which resource is referenced by the expression at the specified position in the specified file, or <c>null</c> if that expression does not reference a (known) resource or if the specified position is invalid.</returns>
		public ResourceResolveResult Resolve(string fileName, IDocument document, int caretLine, int caretColumn, char? charTyped)
		{
			if (fileName == null || document == null) {
				LoggingService.Debug("ResourceToolkit: "+this.GetType().ToString()+".Resolve called with null fileName or document argument");
				return null;
			}
			if (caretLine < 0 || caretColumn < 0 || caretLine >= document.TotalNumberOfLines || caretColumn >= document.GetLineSegment(caretLine).TotalLength) {
				LoggingService.Debug("ResourceToolkit: "+this.GetType().ToString()+".Resolve called with invalid position arguments");
				return null;
			}
			return this.Resolve(fileName, document, caretLine, caretColumn, document.PositionToOffset(new TextLocation(caretColumn, caretLine)), charTyped);
		}
		
		/// <summary>
		/// Attempts to resolve a reference to a resource.
		/// </summary>
		/// <param name="editor">The text editor for which a resource resolution attempt should be performed.</param>
		/// <param name="charTyped">The character that has been typed at the caret position but is not yet in the buffer (this is used when invoked from code completion), or <c>null</c>.</param>
		/// <returns>A <see cref="ResourceResolveResult"/> that describes which resource is referenced by the expression at the caret in the specified editor, or <c>null</c> if that expression does not reference a (known) resource.</returns>
		public ResourceResolveResult Resolve(TextEditorControl editor, char? charTyped)
		{
			return this.Resolve(editor.FileName, editor.Document, editor.ActiveTextAreaControl.Caret.Line, editor.ActiveTextAreaControl.Caret.Column, charTyped);
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Attempts to resolve a reference to a resource.
		/// </summary>
		/// <param name="fileName">The name of the file that contains the expression to be resolved.</param>
		/// <param name="document">The document that contains the expression to be resolved.</param>
		/// <param name="caretLine">The 0-based line in the file that contains the expression to be resolved.</param>
		/// <param name="caretColumn">The 0-based column position of the expression to be resolved.</param>
		/// <param name="caretOffset">The offset of the position of the expression to be resolved.</param>
		/// <param name="charTyped">The character that has been typed at the caret position but is not yet in the buffer (this is used when invoked from code completion), or <c>null</c>.</param>
		/// <returns>A <see cref="ResourceResolveResult"/> that describes which resource is referenced by the expression at the specified position in the specified file, or <c>null</c> if that expression does not reference a (known) resource.</returns>
		protected abstract ResourceResolveResult Resolve(string fileName, IDocument document, int caretLine, int caretColumn, int caretOffset, char? charTyped);
		
		/// <summary>
		/// Determines whether this resolver supports resolving resources in the given file.
		/// </summary>
		/// <param name="fileName">The name of the file to examine.</param>
		/// <returns><c>true</c>, if this resolver supports resolving resources in the given file, <c>false</c> otherwise.</returns>
		public abstract bool SupportsFile(string fileName);
		
		/// <summary>
		/// Gets a list of patterns that can be searched for in the specified file
		/// to find possible resource references that are supported by this
		/// resolver.
		/// </summary>
		/// <param name="fileName">The name of the file to get a list of possible patterns for.</param>
		public abstract IEnumerable<string> GetPossiblePatternsForFile(string fileName);
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Tries to find a resource file with the given name in the given directory by
		/// trying all known resource file extensions.
		/// </summary>
		protected static string FindResourceFileName(string fileName)
		{
			string f;
			if (File.Exists(f = Path.ChangeExtension(fileName, ".resources"))) {
				return f;
			}
			if (File.Exists(f = Path.ChangeExtension(fileName, ".resx"))) {
				return f;
			}
			return null;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractResourceResolver"/> class.
		/// </summary>
		protected AbstractResourceResolver()
		{
		}
		
	}
}
