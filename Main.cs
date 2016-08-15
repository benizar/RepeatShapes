//Console UI for creating tessellations.
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
using System.Collections.Generic;

using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;


namespace RepeatShapes
{
	class MainClass
	{
		
		public static void Main (string[] args)
		{
			Console.Title="Repeat Shapes Tests";
			string _keepWorking="y";
			
			
			while(_keepWorking=="y")
			{
				#region Set wkt file
				Console.WriteLine ("Insert a wkt file:");
				
				Geometry _studyArea=null;
				int _cellWidth=0;
				int _cellHeight=0;
				ShapeType _shp=ShapeType.none;
				int _angle=-9999;
				int _radius=0;
				
				//Obtenemos una geometria compleja desde un fichero wkt.
				WKTReader wktr=new WKTReader();
				while(_studyArea==null) {
					try 
					{
				    	WKTFileReader wktf = new WKTFileReader(Console.ReadLine (),wktr);
						_studyArea = (Geometry)wktf.Read()[0];//lee solamente la primera geometría del fichero
				  	} 
					catch 
					{
						Console.Clear ();
						Console.WriteLine ("Insert a wkt file:");
					}
				}
				#endregion
				
	
				
				#region Set cell width
				while(_cellWidth==0)
				{
					try
					{
						Console.WriteLine ("Insert an integer (and reasonable) cell width:");
						_cellWidth=  Convert.ToInt32(Console.ReadLine ());
					}
					catch
					{
						Console.Clear ();
					}
				}
				#endregion
				
				#region Set cell height
				while(_cellHeight==0)
				{
					try
					{
						Console.WriteLine ("Insert an integer (and reasonable) cell height:");
						_cellHeight=  Convert.ToInt32(Console.ReadLine ());
					}
					catch
					{
						Console.Clear ();
					}
				}
				#endregion
				
				#region Set Shape
				while(_shp==ShapeType.none)
				{
					try
					{
						Console.WriteLine ("Insert Shape Type (rectangle, triangle or hexagon):");
						string shape=Console.ReadLine ();
						
						if(shape=="rectangle")
						{
							_shp=ShapeType.rectangle;
						}
						else if(shape=="triangle")
						{
							_shp=ShapeType.triangle;
						}
						else if(shape=="hexagon")
						{
							_shp=ShapeType.hexagon;
						}
					}
					catch
					{
						Console.Clear ();
					}
				}				
				#endregion
				
				#region Set angle						
				while(_angle==-9999)
					{
						try
						{					
							Console.WriteLine ("Insert an integer rotation angle (0-360º) :");
							_angle =  Convert.ToInt32(Console.ReadLine ());
						}
						catch
						{
							Console.Clear ();
						}
					}
				#endregion
				
				
				#region Set radius						
				if(_shp.ToString ()=="hexagon")
				{
					while(_radius==0)
					{
						try
						{					
							Console.WriteLine ("Insert an integer (and reasonable) radius:");
							_radius =  Convert.ToInt32(Console.ReadLine ());
						}
						catch
						{
							Console.Clear ();
						}
					}
				}
				#endregion
				
				
				#region Calculate Grids
				GraticuleBuilder _grid = new GraticuleBuilder(_studyArea,_cellWidth,_cellHeight, _shp, _angle,_radius);
				Console.WriteLine (_grid.FilteredGrid.AsText ());
				#endregion
				
				
				
				#region Restart Application
				Console.WriteLine ("Do you want to calculate a different grid? (y/n)");
				if(Console.ReadLine ()=="y")
				{
					_keepWorking="y";
					Console.Clear ();
				}
				else if(Console.ReadLine ()=="n")
				{
					_keepWorking="n";
				}
				#endregion
				
			}//End While
			
			
			

			

			
		}
	}
}
