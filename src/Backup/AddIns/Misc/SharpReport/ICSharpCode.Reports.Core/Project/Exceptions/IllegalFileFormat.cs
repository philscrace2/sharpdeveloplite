// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

	/// <summary>
	/// This exception is throw'n when something is wrong with the File Format
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 25.04.2005 14:29:20
	/// </remarks>
namespace ICSharpCode.Reports.Core {	
	[Serializable()]
	public class IllegalFileFormatException : System.Exception {
		
		public IllegalFileFormatException():base ()
		{
		}
		
		public IllegalFileFormatException(string errorMessage) :base (errorMessage)
		{	
		}
		
		public IllegalFileFormatException(string errorMessage,
		                            Exception exception):base (errorMessage,exception)
		{	
		}

		
		protected IllegalFileFormatException(SerializationInfo info, 
         StreamingContext context) : base(info, context)
		{
         // Implement type-specific serialization constructor logic.
      	}

			
 		[SecurityPermissionAttribute(SecurityAction.Demand, 
          SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		base.GetObjectData(info, context);
		}
	}
}
