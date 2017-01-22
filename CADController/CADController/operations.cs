﻿//*This file contains controller sketch.*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CADController
{
    using ObjectId = System.UInt32;
    using DocumentId = System.UInt32;
    
    class Operations
    {
        //create point
        public static ObjectId createPoint(IntPtr curSes, DocumentId docID, double X, double Y)
        {
        	IntPtr newNode = CoreWrapper.nodeFactory(X, Y);
            IntPtr newPoint = CoreWrapper.pointFactory(newNode);
        	IntPtr newPointGen = CoreWrapper.genericFactory(newPoint);
        	
        	ObjectId newPointID = CoreWrapper.attachToBase(curSes, docID, newPointGen);
        	CoreWrapper.commit(curSes, docID);
        
        	return newPointID;
        }

        //create line: start point, end point
        public static ObjectId createLine(IntPtr curSes, DocumentId docID, double X1, double Y1, double X2, double Y2)
        {
            IntPtr start = CoreWrapper.nodeFactory(X1, Y1);
            IntPtr end = CoreWrapper.nodeFactory(X2, Y2);

            IntPtr newLine = CoreWrapper.lineFactory(start, end);
            IntPtr newLineGen = CoreWrapper.genericFactory(newLine);

            ObjectId newLineID = CoreWrapper.attachToBase(curSes, docID, newLineGen);
            CoreWrapper.commit(curSes, docID);
        
        	return newLineID;
        }

        //create circle: center point, side point
        public static ObjectId createCircle(IntPtr curSes, DocumentId docID, double X1, double Y1, double X2, double Y2)
        {
        	IntPtr center = CoreWrapper.nodeFactory(X1, Y1);
            IntPtr side = CoreWrapper.nodeFactory(X2, Y2);

            IntPtr newCircle = CoreWrapper.circleFactory(center, side);
            IntPtr newCircleGen = CoreWrapper.genericFactory(newCircle);

            ObjectId newCircleID = CoreWrapper.attachToBase(curSes, docID, newCircleGen);
            CoreWrapper.commit(curSes, docID);
        
        	return newCircleID;
        }

        //create contour by existing edges
        /*
        public static ObjectId createContour(IntPtr curSes, DocumentId docID, IntPtr objects)
        {	
        	std::vector<Edge*> edges;
        	uint currentLayer = CoreWrapper.getGenericLayer(curSes, docID, objects.at(0));
        
        	std::for_each(objects.begin(), objects.end(),
        		[=, &curSes, &edges](ObjectId objID)
        	{
        		IntPtr curEdge = CoreWrapper.getGenericTopology(curSes, docID, objID);
        		edges.push_back(curEdge);
        		CoreWrapper.detachFromBase(curSes, docID, objID);
        	});

            IntPtr newContour = CoreWrapper.contourFactory(edges);
            IntPtr newContourGen = CoreWrapper.genericFactory(newContour, currentLayer);
        
        	ObjectId newContourID = CoreWrapper.attachToBase(curSes, docID, newContourGen);
        	CoreWrapper.commit(curSes, docID);
        
        	return newContourID;
        }
        */
        //delete point by id
        public static void deleteObject(IntPtr curSes, DocumentId docID, ObjectId objID)
        {
            CoreWrapper.detachFromBase(curSes, docID, objID);
            CoreWrapper.commit(curSes, docID);
        }
        
        //destroy contour, all edges will be free
        /*
        public static void destroyContour(IntPtr curSes, DocumentId docID, ObjectId objID)
        {
        	IntPtr temp = CoreWrapper.detachFromBase(curSes, docID, objID);
            uint currentLayer = CoreWrapper.getGenericLayer(temp);
            IntPtr tempCon = CoreWrapper.getGenericTopology(temp);
        
        	IntPtr edges = CoreWrapper.getContourEdges(tempCon);
            std::for_each(edges.begin(), edges.end(),
        		[=, &curSes](Edge* curEdge)
        	{
        		IntPtr newEdgeGen = CoreWrapper.genericFactory(curEdge, currentLayer);
        		CoreWrapper.attachToBase(curSes, docID, newEdgeGen);
        	});
        	
        	CoreWrapper.commit(curSes, docID);
        }
        */
        //undo command
        public static void undo(IntPtr curSes, DocumentId docID)
        {
            CoreWrapper.undo(curSes, docID);
        }
        
        //redo command
        public static void redo(IntPtr curSes, DocumentId docID)
        {
            CoreWrapper.redo(curSes, docID);
        }
        
        //set layers to show
        public static void setLayersToShow(IntPtr curSes, DocumentId docID, IntPtr layersToShow)
        {
            CoreWrapper.setLayers(curSes, docID, layersToShow);
        }

        //set background color
        public static void setBackgroundColor(IntPtr curSes, DocumentId docID, COLOR newColor)
        {
            CoreWrapper.setBackgroundColor(curSes, docID, newColor);
        }

        //show the 2d editior field
        public static void display(IntPtr curSes, DocumentId docID)
        {
            CoreWrapper.toScreen(curSes, docID);
        }

        //test operation - show and/or remove objects from controller
        public static void showAndRemoveFreeGeneric(IntPtr curSes, DocumentId docID)
        {
            IntPtr node = CoreWrapper.nodeFactory(33, 33);
            IntPtr newPoint = CoreWrapper.pointFactory(node);
            IntPtr newPointGen = CoreWrapper.genericFactory(newPoint);
            CoreWrapper.attachToBuffer(curSes, docID, newPointGen);
        }
    }
}