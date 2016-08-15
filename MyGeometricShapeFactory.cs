//MyGeometricShapeFactory class for creating tessellations.
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

using NetTopologySuite.Utilities;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;


namespace RepeatShapes
{
	/// <summary>
	/// This class inherites from NTS's GeometricShapeFactory.
	/// See that rectangles are created using the parent class.
	/// </summary>
	public class MyGeometricShapeFactory:GeometricShapeFactory
	{
		public MyGeometricShapeFactory ()
		{
		}
		
		private readonly Dimensions _dim = new Dimensions();
        private int _nPts = 100;
		

		/// <summary>
		/// Creates a triangular polygon.
		/// </summary>
		/// <returns>
		/// The triangle.
		/// </returns>
        public IPolygon CreateTriangle()
        {
            int i;
            int ipt = 0;
            int nSide = _nPts / 100;
            if (nSide < 1) nSide = 1;
            double XsegLen = this.Envelope.Width/nSide;
            double YsegLen = this.Envelope.Width/nSide;

            Coordinate[] pts = new Coordinate[3 * nSide + 1];
            Envelope env = (Envelope)this.Envelope;

            for (i = 0; i < nSide; i++) 
            {
                double x = env.MinX + i * XsegLen;
                double y = env.MinY;
                pts[ipt++] = (Coordinate)CreateCoord(x, y);
            }
            for (i = 0; i < nSide; i++) 
            {
                double x = env.MaxX;
                double y = env.MinY + i * YsegLen;
                pts[ipt++] = (Coordinate)CreateCoord(x, y);
            }
            for (i = 0; i < nSide; i++) 
            {
            	double x = env.MaxX - XsegLen/2;
                double y = env.MaxY;
                pts[ipt++] = (Coordinate)CreateCoord(x, y);
            }
//            for (i = 0; i < nSide; i++) 
//            {
//                double x = env.MinX;
//                double y = env.MaxY - i * YsegLen;
//                pts[ipt++] = (Coordinate)CreateCoord(x, y);
//            }
            pts[ipt++] = new Coordinate(pts[0]);

            ILinearRing ring = GeomFact.CreateLinearRing(pts);
            IPolygon poly = GeomFact.CreatePolygon(ring, null);
            return poly;
        }
        
        
		/// <summary>
		/// Creates an inverted triangle (up-down).
		/// </summary>
		/// <returns>
		/// The inverted triangle.
		/// </returns>
        public IPolygon CreateInvertedTriangle()
        {
            int i;
            int ipt = 0;
            int nSide = _nPts / 100;
            if (nSide < 1) nSide = 1;
            double XsegLen = this.Envelope.Width/nSide;
            double YsegLen = this.Envelope.Width/nSide;

            Coordinate[] pts = new Coordinate[3 * nSide + 1];
            Envelope env = (Envelope)this.Envelope;

            for (i = 0; i < nSide; i++) 
            {
                double x = env.MinX + i * XsegLen;
                double y = env.MaxY;
                pts[ipt++] = (Coordinate)CreateCoord(x, y);
            }
            for (i = 0; i < nSide; i++) 
            {
            	double x = env.MinX + XsegLen/2;
                double y = env.MinY;
                pts[ipt++] = (Coordinate)CreateCoord(x, y);
            }
            for (i = 0; i < nSide; i++) 
            {
                double x = env.MaxX;
                double y = env.MaxY + i * YsegLen;
                pts[ipt++] = (Coordinate)CreateCoord(x, y);
            }

          
            pts[ipt++] = new Coordinate(pts[0]);

            ILinearRing ring = GeomFact.CreateLinearRing(pts);
            IPolygon poly = GeomFact.CreatePolygon(ring, null);
            return poly;
        }
		
        
        /// <summary>
        /// Creates an hexagon adjusted by a radius parameter.
        /// </summary>
        /// <returns>
        /// The hexagon.
        /// </returns>
        /// <param name='radius'>
        /// A proportion that defines the longitude of the hexagon's base.
		/// A larger radius is relate with a narrower base of the hexagon.
        /// </param>
        public IPolygon CreateHexagon(int radius)
        {
            int i;
            int ipt = 0;
            int nSide = _nPts / 100;
            if (nSide < 1) nSide = 1;
            double XsegLen = this.Envelope.Width/nSide;
            double YsegLen = this.Envelope.Height/nSide;

            Coordinate[] pts = new Coordinate[6 * nSide + 1];
            Envelope env = (Envelope)this.Envelope;

             for (i = 0; i < nSide; i++) 
            {
                double x = env.MinX + radius + i * XsegLen;
                double y = env.MinY;
                pts[ipt++] = (Coordinate)CreateCoord(x, y);
            }
            for (i = 0; i < nSide; i++) 
            {
                double x = env.MaxX - radius;
                double y = env.MinY + i * YsegLen;
                pts[ipt++] = (Coordinate)CreateCoord(x, y);
            }
            for (i = 0; i < nSide; i++) 
            {
                double x = env.MaxX;
                double y = env.MinY+(env.Height/2) + i * YsegLen;
                pts[ipt++] = (Coordinate)CreateCoord(x, y);
            }
            for (i = 0; i < nSide; i++) 
            {
                double x = env.MaxX - radius;
                double y = env.MaxY + i * YsegLen;
                pts[ipt++] = (Coordinate)CreateCoord(x, y);
            }
            for (i = 0; i < nSide; i++) 
            {
                double x = env.MinX + radius + i * XsegLen;
                double y = env.MaxY;
                pts[ipt++] = (Coordinate)CreateCoord(x, y);
            }
            for (i = 0; i < nSide; i++) 
            {
                double x = env.MinX;
                double y = env.MinY+(env.Height/2) + i * YsegLen;
                pts[ipt++] = (Coordinate)CreateCoord(x, y);
            }

            pts[ipt++] = new Coordinate(pts[0]);

            ILinearRing ring = GeomFact.CreateLinearRing(pts);
            IPolygon poly = GeomFact.CreatePolygon(ring, null);
            return poly;
        }
		
		
		
	}
}

