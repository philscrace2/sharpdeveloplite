// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace HexEditor.Util
{
	/// <summary>
	/// Used to save data about changes to undo/redo.
	/// </summary>
	public class UndoStep
	{
		byte[] bytes, oldBytes;
		int start;
		long timeStamp;
		UndoAction action;
		
		public UndoStep(byte[] bytes, byte[] oldBytes, int start, UndoAction action)
		{
			this.bytes = bytes;
			this.oldBytes = oldBytes;
			this.start = start;
			this.action = action;
			this.timeStamp = DateTime.Now.Ticks;
		}
		
		public long TimeStamp {
			get { return timeStamp; }
		}
		
		public byte[] GetBytes() {
			return bytes;
		}
		
		public byte[] GetOldBytes() {
			return oldBytes;
		}
		
		public int Start {
			get { return start; }
		}
		
		public UndoAction Action {
			get { return action; }
		}
		
		public override string ToString()
		{
			return "{ Bytes: " + System.Text.Encoding.Default.GetString(GetBytes()) + ", OldBytes: " + System.Text.Encoding.Default.GetString(GetOldBytes()) + ", Start: " + start.ToString() + ", Action: " + action.ToString() + " }";
		}
	}
}
