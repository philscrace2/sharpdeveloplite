// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// IllegalQueryException is thrown when a QueryString contains other statements as
	/// Select. So it should not happen do alter a Database while doing a Query with SharpReport
	/// </summary>
	
	[SerializableAttribute]
	public class IllegalQueryException: System.Exception{
		
		
		public IllegalQueryException():base(){
			
		}
		public IllegalQueryException(string errorMessage) :base (errorMessage){
		}
		
		public IllegalQueryException(string errorMessage,
		                            Exception exception):base (errorMessage,exception){
			
		}
		
		protected IllegalQueryException(SerializationInfo info,
         StreamingContext context) : base(info, context){
         // Implement type-specific serialization constructor logic.
      	}
		
 		
		[SecurityPermissionAttribute(SecurityAction.Demand,
          SerializationFormatter = true)]

		public override void GetObjectData(SerializationInfo info, StreamingContext context){
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
		}
	}
}
