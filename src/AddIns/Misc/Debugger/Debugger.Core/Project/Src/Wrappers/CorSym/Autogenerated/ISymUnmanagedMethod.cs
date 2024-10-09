// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

// This file is automatically generated - any changes will be lost

#pragma warning disable 1591

namespace Debugger.Wrappers.CorSym
{
	using System;
	
	
	public partial class ISymUnmanagedMethod
	{
		
		private Debugger.Interop.CorSym.ISymUnmanagedMethod wrappedObject;
		
		internal Debugger.Interop.CorSym.ISymUnmanagedMethod WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ISymUnmanagedMethod(Debugger.Interop.CorSym.ISymUnmanagedMethod wrappedObject)
		{
			this.wrappedObject = wrappedObject;
			ResourceManager.TrackCOMObject(wrappedObject, typeof(ISymUnmanagedMethod));
		}
		
		public static ISymUnmanagedMethod Wrap(Debugger.Interop.CorSym.ISymUnmanagedMethod objectToWrap)
		{
			if ((objectToWrap != null))
			{
				return new ISymUnmanagedMethod(objectToWrap);
			} else
			{
				return null;
			}
		}
		
		~ISymUnmanagedMethod()
		{
			object o = wrappedObject;
			wrappedObject = null;
			ResourceManager.ReleaseCOMObject(o, typeof(ISymUnmanagedMethod));
		}
		
		public bool Is<T>() where T: class
		{
			System.Reflection.ConstructorInfo ctor = typeof(T).GetConstructors()[0];
			System.Type paramType = ctor.GetParameters()[0].ParameterType;
			return paramType.IsInstanceOfType(this.WrappedObject);
		}
		
		public T As<T>() where T: class
		{
			try {
				return CastTo<T>();
			} catch {
				return null;
			}
		}
		
		public T CastTo<T>() where T: class
		{
			return (T)Activator.CreateInstance(typeof(T), this.WrappedObject);
		}
		
		public static bool operator ==(ISymUnmanagedMethod o1, ISymUnmanagedMethod o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ISymUnmanagedMethod o1, ISymUnmanagedMethod o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ISymUnmanagedMethod casted = o as ISymUnmanagedMethod;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public uint Token
		{
			get
			{
				return this.WrappedObject.GetToken();
			}
		}
		
		public uint SequencePointCount
		{
			get
			{
				return this.WrappedObject.GetSequencePointCount();
			}
		}
		
		public ISymUnmanagedScope RootScope
		{
			get
			{
				return ISymUnmanagedScope.Wrap(this.WrappedObject.GetRootScope());
			}
		}
		
		public ISymUnmanagedScope GetScopeFromOffset(uint offset)
		{
			return ISymUnmanagedScope.Wrap(this.WrappedObject.GetScopeFromOffset(offset));
		}
		
		public uint GetOffset(ISymUnmanagedDocument document, uint line, uint column)
		{
			return this.WrappedObject.GetOffset(document.WrappedObject, line, column);
		}
		
		public void GetRanges(ISymUnmanagedDocument document, uint line, uint column, uint cRanges, out uint pcRanges, System.IntPtr ranges)
		{
			this.WrappedObject.GetRanges(document.WrappedObject, line, column, cRanges, out pcRanges, ranges);
		}
		
		public void GetParameters(uint cParams, out uint pcParams, System.IntPtr @params)
		{
			this.WrappedObject.GetParameters(cParams, out pcParams, @params);
		}
		
		public ISymUnmanagedNamespace Namespace
		{
			get
			{
				ISymUnmanagedNamespace pRetVal;
				Debugger.Interop.CorSym.ISymUnmanagedNamespace out_pRetVal;
				this.WrappedObject.GetNamespace(out out_pRetVal);
				pRetVal = ISymUnmanagedNamespace.Wrap(out_pRetVal);
				return pRetVal;
			}
		}
		
		public int GetSourceStartEnd(ISymUnmanagedDocument[] docs, uint[] lines, uint[] columns)
		{
			int pRetVal;
			Debugger.Interop.CorSym.ISymUnmanagedDocument[] array_docs = new Debugger.Interop.CorSym.ISymUnmanagedDocument[docs.Length];
			for (int i = 0; (i < docs.Length); i = (i + 1))
			{
				if ((docs[i] != null))
				{
					array_docs[i] = docs[i].WrappedObject;
				}
			}
			this.WrappedObject.GetSourceStartEnd(array_docs, lines, columns, out pRetVal);
			for (int i = 0; (i < docs.Length); i = (i + 1))
			{
				if ((array_docs[i] != null))
				{
					docs[i] = ISymUnmanagedDocument.Wrap(array_docs[i]);
				} else
				{
					docs[i] = null;
				}
			}
			return pRetVal;
		}
		
		public void GetSequencePoints(uint cPoints, out uint pcPoints, uint[] offsets, ISymUnmanagedDocument[] documents, uint[] lines, uint[] columns, uint[] endLines, uint[] endColumns)
		{
			Debugger.Interop.CorSym.ISymUnmanagedDocument[] array_documents = new Debugger.Interop.CorSym.ISymUnmanagedDocument[documents.Length];
			for (int i = 0; (i < documents.Length); i = (i + 1))
			{
				if ((documents[i] != null))
				{
					array_documents[i] = documents[i].WrappedObject;
				}
			}
			this.WrappedObject.GetSequencePoints(cPoints, out pcPoints, offsets, array_documents, lines, columns, endLines, endColumns);
			for (int i = 0; (i < documents.Length); i = (i + 1))
			{
				if ((array_documents[i] != null))
				{
					documents[i] = ISymUnmanagedDocument.Wrap(array_documents[i]);
				} else
				{
					documents[i] = null;
				}
			}
		}
	}
}

#pragma warning restore 1591
