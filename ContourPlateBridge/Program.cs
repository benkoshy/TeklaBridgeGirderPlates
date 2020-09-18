using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

using CsvHelper;
using System.IO;
using System.Text.RegularExpressions;

namespace ContourPlateBridge
{
    class Program
    {
        static Regex regex = new Regex(@"\d+$");

        static void Main(string[] args)
        {
            Model model = new Model();

            List<ToleranceReport> _tolerances = new List<ToleranceReport>();

            if (model.GetConnectionStatus())
            {
                using (var reader = new StreamReader(@"C:\Users\Koshy\source\repos\ContourPlateBridge\20200918-PLATES TO DETAIL.csv"))
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                {                    
                    var plates = csv.GetRecords<PlateData>().ToList();

                    int rowCount = 0;
                    int columnCount = 0;

                    if (plates.All(p => (new PrefixMaker(p.BearingMark)).IsMatch() ))
                    {
                        foreach (PlateData plate in plates)
                        {
                            int xInsertionPoint = rowCount * 500;
                            int yInsertionPoint = columnCount * 1050;

                            if (rowCount == 20)
                            {
                                columnCount++;
                                rowCount = 0;
                            }
                            else
                            {
                                rowCount++;
                            }

                            SmartContourPlate contourPlate = new SmartContourPlate(model, xInsertionPoint, yInsertionPoint, plate.Profile, plate.T1, plate.T2, plate.T3, plate.T4, plate.DimA, plate.DimB, plate.BearingMark, _tolerances, (new PrefixMaker(plate.BearingMark)), plate.IsM10BoltsRequired);
                            contourPlate.addContourPlate();
                            contourPlate.AddUserDefinedAttributes();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Oops: you have a problem with one of the names of the plates - they do not end with two numbers as required.");
                    }                   
                }
            }

            model.CommitChanges();            

            
            using (var writer = new StreamWriter(@"C:\Users\Koshy\source\repos\ContourPlateBridge\ToleranceReport.csv"))
            using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                    csv.WriteRecords(_tolerances);
            }

            Console.ReadLine();
        }
    }
}
