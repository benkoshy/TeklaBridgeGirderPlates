using System;
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

                    foreach (PlateData plate in plates)
                    {
                        SmartContourPlate contourPlate = new SmartContourPlate(model, 0, 0, plate.Profile, plate.T1, plate.T2, plate.T3, plate.T4, plate.DimA, plate.DimB, plate.BearingMark);
                        contourPlate.addContourPlate();
                    }
                }
            }

            model.CommitChanges();
        }

        
    }
}
