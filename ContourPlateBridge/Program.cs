﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

using CsvHelper;
using System.IO;


namespace ContourPlateBridge
{
    class Program
    {
        static void Main(string[] args)
        {
            Model model = new Model();

            if (model.GetConnectionStatus())
            {
                using (var reader = new StreamReader(@"C:\Users\Koshy\source\repos\ContourPlateBridge\COMBINED B81-AND-B80- Bearing Schedules.csv"))
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                {                    
                    var plates = csv.GetRecords<PlateData>();

                    int rowCount = 0;
                    int columnCount = 0;

                    //PlateData plate = plates.First();
                    //SmartContourPlate contourPlate = new SmartContourPlate(model, 0, 0, plate.Profile, plate.T1, plate.T2, plate.T3, plate.T4, plate.DimA, plate.DimB, plate.BearingMark);
                    //contourPlate.addContourPlate();

                    foreach (PlateData plate in plates)
                    {
                        int xInsertionPoint = rowCount * 500;
                        int yInsertionPoint = columnCount * 720;

                        if (rowCount == 20)
                        {
                            columnCount++;
                            rowCount = 0;
                        }
                        else
                        {
                            rowCount++;
                        }                        

                        SmartContourPlate contourPlate = new SmartContourPlate(model, xInsertionPoint, yInsertionPoint, plate.Profile, plate.T1, plate.T2, plate.T3, plate.T4, plate.DimA, plate.DimB, plate.BearingMark);
                        contourPlate.addContourPlate();
                    }
                }
            }

            model.CommitChanges();

            Console.ReadLine();
        }

        
    }
}
