//GraticuleBuilder class for creating tessellations.
//Copyright (C) 2012 Benito M. Zaragozí
//Authors: Benito M. Zaragozí (www.gisandchips.org)
//Send comments and suggestions to benito.zaragozi@ua.es

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;

using NetTopologySuite.IO;
using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;
using GeoAPI.Geometries;


namespace RepeatShapes
{
	public class GraticuleBuilder
	{
		public GraticuleBuilder (IGeometry studyArea,int cellWidth, int cellHeight, ShapeType shp, int angle, int radius)
		{
			_studyArea=studyArea;
			GetBigArea (_studyArea,cellWidth,cellHeight);
			_origin=GetOrigin (_bigArea);
			
			if(shp== ShapeType.rectangle)
			{
				_bigGrid = rectRepeat(cellWidth,cellHeight,_bigGridRows,_bigGridCols);
			}
			else if(shp==ShapeType.triangle)
			{
				_bigGrid = triRepeat(cellWidth,cellHeight,_bigGridRows,_bigGridCols);
			}
			else if(shp==ShapeType.hexagon)
			{
				_bigGrid = hexRepeat(cellWidth,cellHeight,_bigGridRows,_bigGridCols, radius);
			}
			
			_bigGrid = rotateGrid (_bigGrid, _studyArea, angle);
			_filteredGrid=gridFilter(_bigGrid,_studyArea);
		}
		
		
		IGeometry _studyArea;
		IGeometry _bigArea;
		IPoint _origin;
		GeometryCollection _bigGrid;
		GeometryCollection _filteredGrid;
		int _bigGridRows=0;
		int _bigGridCols=0;
		
		
		
		public IGeometry StudyArea {
			get { return _studyArea; }
		}

		public GeometryCollection BigGrid
		{
			get{return _bigGrid;}
		}
		
		public GeometryCollection FilteredGrid
		{
			get{return _filteredGrid;}
		}
		
		
		/// <summary>
		/// Calculates the left-bottom coordinate/point of a geometry.
		/// </summary>
		/// <returns>
		/// The origin point for the grid.
		/// </returns>
		/// <param name='bigAreaEnvelope'>
		/// A big area that encloses the study area.
		/// </param>
		private Point GetOrigin(IGeometry bigAreaEnvelope)
		{	
			double minX=0;
			double minY=0;
			
			foreach(Coordinate c in bigAreaEnvelope.Coordinates)
			{
				minX = c.X;
				minY = c.Y;
				
				if(c.X < minX)
				{
					minX=c.X;
				}
				if(c.Y<minY)
				{
					minY=c.Y;
				}
			}
			
			Point p = new Point(minX,minY);
			return p;
		}
		
		/// <summary>
		/// Create a big rectangle containing the study area.
		/// </summary>
		/// <param name="studyArea"></param>
		/// <param name="widthDivBy"></param>
		/// <param name="heightDivBy"></param>
		private void GetBigArea(IGeometry studyArea, int widthDivBy, int heightDivBy)
		{
			//circumscribed circle for obtaining its envelope and create the bigArea.
			NetTopologySuite.Algorithm.MinimumBoundingCircle mbc = 
				new NetTopologySuite.Algorithm.MinimumBoundingCircle(studyArea);
			
			_bigArea = mbc.GetCircle ().Envelope.Boundary;
			double diameter = mbc.GetRadius ()*4;
						
			//get the total num of cols and rows
			_bigGridCols=(int)diameter/widthDivBy;
			_bigGridRows=(int)diameter/heightDivBy;	
		}
		
		/// <summary>
		/// Rotates the bigGrid.
		/// </summary>
		/// <returns>
		/// The rotated grid.
		/// </returns>
		/// <param name='bigGrid'>
		/// The grid that overlaps the bigArea.
		/// </param>
		/// <param name='studyArea'>
		/// The region of interest.
		/// </param>
		/// <param name='degree'>
		/// A rotation angle in decimal degrees.
		/// </param>
		private GeometryCollection rotateGrid(GeometryCollection bigGrid, IGeometry studyArea, int degree)
		{
			NetTopologySuite.Geometries.Utilities.AffineTransformation trans = 
				NetTopologySuite.Geometries.Utilities.AffineTransformation.RotationInstance(
					Degrees.ToRadians (degree),studyArea.Centroid.X, studyArea.Centroid.Y);
			return (GeometryCollection)trans.Transform (bigGrid);
		}
		
		/// <summary>
		/// Filter the bigGrid for achieving a grid adjusted to the region of interest.
		/// </summary>
		/// <returns>
		/// A grid adjusted to the region of interest.
		/// </returns>
		/// <param name='bigGrid'>
		/// A grid that exceeds the region of interest.
		/// </param>
		/// <param name='studyArea'>
		/// The region of interest.
		/// </param>
		private GeometryCollection gridFilter(GeometryCollection bigGrid, IGeometry studyArea)
		{	
			List<IGeometry>filteredGrid=new List<IGeometry>();
			
			foreach(IPolygon p in bigGrid.Geometries)
			{
				if(p.Intersects(studyArea)==true)
				{
					filteredGrid.Add(p);
				}
			}
			return new GeometryCollection(filteredGrid.ToArray ());
		}
		
		/// <summary>
		/// Creates a rectangular tessellation.
		/// </summary>
		/// <returns>
		/// A rectangular grid.
		/// </returns>
		/// <param name='cellWidth'>
		/// Cell width.
		/// </param>
		/// <param name='cellHeight'>
		/// Cell height.
		/// </param>
		/// <param name='numRows'>
		/// Number of rows.
		/// </param>
		/// <param name='numColumns'>
		/// Number of columns.
		/// </param>
		private GeometryCollection rectRepeat(int cellWidth, int cellHeight, int numRows, int numColumns)
		{
			List<IGeometry> bigGrid = new List<IGeometry>();
			double x = _origin.X;
			double y = _origin.Y;
			
			GeometricShapeFactory gsf = new GeometricShapeFactory();
			gsf.Height=Convert.ToDouble(cellHeight);
			gsf.Width=Convert.ToDouble(cellWidth);
			gsf.NumPoints=4;
			
			
			for (int i = 1; i <= numColumns; i++)
			{
				for (int j = 1; j <= numRows; j++)
				{
					gsf.Base=new Coordinate(x, y);
					IPolygon newpol = gsf.CreateRectangle();
					
					newpol.UserData=i+" - "+j;
					bigGrid.Add(newpol);
					y += cellHeight;
				}
				if(i!=0)
				{
					x += cellWidth;
				}
				y = _origin.Y;
			}
			return new GeometryCollection(bigGrid.ToArray ());
		}
		
		/// <summary>
		/// Creates a triangular tessellation.
		/// </summary>
		/// <returns>
		/// A triangular tessellation.
		/// </returns>
		/// <param name='cellWidth'>
		/// Cell width.
		/// </param>
		/// <param name='cellHeight'>
		/// Cell height.
		/// </param>
		/// <param name='numRows'>
		/// Number of rows.
		/// </param>
		/// <param name='numColumns'>
		/// Number of columns.
		/// </param>
		private GeometryCollection triRepeat(int cellWidth, int cellHeight, int numRows, int numColumns)
		{
			List<IGeometry> bigGrid = new List<IGeometry>();
			double x = _origin.X-cellWidth;
			double y = _origin.Y;
			
			MyGeometricShapeFactory gsf = new MyGeometricShapeFactory();
			gsf.Height=cellHeight;
			gsf.Width=cellWidth;
			gsf.NumPoints=4;
			
			for (int i = 1; i <= numColumns*2; i++)
			{
				for (int j = 1; j <= numRows; j++)
				{
					if (i % 2 == 0)
					{
						gsf.Centre=new Coordinate(x, y);
						gsf.Envelope=new Envelope(x,x+cellWidth,y,y+cellHeight);
						IPolygon newpol = gsf.CreateTriangle();
						
						newpol.UserData=i+" - "+j;
						bigGrid.Add(newpol);
					}
					else
					{
						gsf.Centre=new Coordinate(x, y);
						gsf.Envelope=new Envelope(x+cellWidth/2,x+cellWidth/2+cellWidth,y,y+cellHeight);
						IPolygon newpol = gsf.CreateInvertedTriangle();
						
						newpol.UserData=i+" - "+j;
						bigGrid.Add(newpol);
					}
					y += cellHeight;
				}
				if(i % 2!=0)
				{
					x += cellWidth;
				}
				y = _origin.Y;
			}
			return new GeometryCollection(bigGrid.ToArray ());
			
		}
		
		/// <summary>
		/// Creates an hexagonal tessellation.
		/// </summary>
		/// <returns>
		/// The hexagonal tessellation
		/// </returns>
		/// <param name='cellWidth'>
		/// Cell width.
		/// </param>
		/// <param name='cellHeight'>
		/// Cell height.
		/// </param>
		/// <param name='numRows'>
		/// Number of rows.
		/// </param>
		/// <param name='numColumns'>
		/// Number of columns.
		/// </param>
		/// <param name='radius'>
		/// A proportion that defines the longitude of the hexagon's base.
		/// A larger radius is relate with a narrower base of the hexagon.
		/// </param>
		private GeometryCollection hexRepeat(int cellWidth, int cellHeight, int numRows, int numColumns, int radius)
		{
			List<IGeometry> bigGrid = new List<IGeometry>();
			double x = _origin.X-cellWidth;
			double y = _origin.Y;
			
			MyGeometricShapeFactory gsf = new MyGeometricShapeFactory();
			gsf.Height=cellHeight;
			gsf.Width=cellWidth;
			gsf.NumPoints=4;
			
			for (int i = 1; i <= numColumns*2; i++)
			{
				for (int j = 1; j <= numRows; j++)
				{
					if (i % 2 == 0)
					{
						gsf.Centre=new Coordinate(x, y);
						gsf.Envelope=new Envelope(x,x+cellWidth,y,y+cellHeight);
						IPolygon newpol = gsf.CreateHexagon(radius);
						
						newpol.UserData=i+" - "+j;
						bigGrid.Add(newpol);
					}
					else
					{
						gsf.Centre=new Coordinate(x, y);
						gsf.Envelope=new Envelope(x+(cellWidth-radius),x+cellWidth+(cellWidth-radius),y+(cellHeight/2),y+cellHeight+(cellHeight/2));
						IPolygon newpol = gsf.CreateHexagon(radius);
						
						newpol.UserData=i+" - "+j;
						bigGrid.Add(newpol);
					}
					y += cellHeight;
				}
				if(i % 2!=0)
				{
					x += cellWidth+(cellWidth-2*radius);
				}
				y = _origin.Y;
			}
			
			return new GeometryCollection(bigGrid.ToArray ());
		}
		
		
		
		
		
	}
}

