﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CADController
{
    using ObjectId = System.UInt32;
    using DocumentId = System.UInt32;

    enum COLOR { BLACK, RED, GREEN, BLUE, YELLOW };
    enum THICKNESS { ONE, TWO, THREE, FOUR, FIVE };
    enum dataType { unsigned, intptr };

    class CoreWrapper
    {
        const ObjectId NOT_FROM_BASE = 0;

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sessionFactory();

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern DocumentId attachDocument(IntPtr pObject, IntPtr doc);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ObjectId attachToBase(IntPtr pObject, DocumentId docID, IntPtr genObj);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr detachFromBase(IntPtr pObject, DocumentId docID, ObjectId objID);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void attachToBuffer(IntPtr pObject, DocumentId docID, IntPtr genObj);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getGenTopology(IntPtr pObject, DocumentId docID, ObjectId objID);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getGenLayer(IntPtr pObject, DocumentId docID, ObjectId objID);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setLayers(IntPtr pObject, DocumentId docID, IntPtr newLayers);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setBackgroundColor(IntPtr pObject, DocumentId docID, COLOR color);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void toScreen(IntPtr pObject, DocumentId docID);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void commit(IntPtr pObject, DocumentId docID);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void undo(IntPtr pObject, DocumentId docID);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void redo(IntPtr pObject, DocumentId docID);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr documentFactory();

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr nodeFactory(double x, double y);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr pointFactory(IntPtr node);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lineFactory(IntPtr start, IntPtr end);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr circleFactory(IntPtr center, IntPtr side);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr contourFactory(IntPtr edges);
        
        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getContourEdges(IntPtr pObject);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr genericFactory(IntPtr primitive, uint layer = 0,
            COLOR color = COLOR.BLACK, THICKNESS thickness = THICKNESS.THREE);
        
        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getGenericLayer(IntPtr pObject);
        
        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getGenericTopology(IntPtr pObject);
    }

    class STLVector
    {
        private IntPtr _pointer;
        private dataType _type;
        
        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr createVector();

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void deleteVector(IntPtr pObject);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void push_back_unsigned(IntPtr pObject, uint value);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void push_back_edge(IntPtr pObject, IntPtr value);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pop_back(IntPtr pObject);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void clear(IntPtr pObject);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint at_unsigned(IntPtr pObject, uint index);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr at_edge(IntPtr pObject, uint index);
        
        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint size(IntPtr pObject);

        public STLVector(dataType type)
        {
            _pointer = createVector();
            _type = type;
        }
        
        public IntPtr getPointer() { return _pointer; }

        public dataType getType() { return _type; }
        
        public void deleteVector()
        {
            deleteVector(_pointer);
            _pointer = IntPtr.Zero;
        }

        public void push_back(uint value) { push_back_unsigned(_pointer, value); }
        public void push_back(IntPtr obj) { push_back_edge(_pointer, obj); }

        public void pop_back() { pop_back(_pointer); }

        public void clear() { clear(_pointer); }

        public uint at1(uint index) { return at_unsigned(_pointer, index); }
        public IntPtr at2(uint index) { return at_edge(_pointer, index); }

        public uint size() { return size(_pointer); }
    }
} 