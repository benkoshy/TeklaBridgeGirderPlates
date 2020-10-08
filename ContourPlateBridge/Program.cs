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
using CsvHelper.Configuration;

namespace ContourPlateBridge
{
    class Program
    {
        static Regex regex = new Regex(@"\d+$");

        static void Main(string[] args)
        {
            Model model = new Model();

            List<ToleranceReport> _tolerances = new List<ToleranceReport>();

            List<MReport> inconsistentMValues = new List<MReport>();

            if (model.GetConnectionStatus())
            {
                using (var reader = new StreamReader(@"C:\Users\Koshy\source\repos\ContourPlateBridge\20201008-BRIDGE-80-PLATES TO DETAIL.csv"))
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                {                    
                    csv.Configuration.RegisterClassMap<PlateDataMap>();
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

                            AddToMReportIfRequired(plate, inconsistentMValues);

                            SmartContourPlate contourPlate = new SmartContourPlate(model, xInsertionPoint, yInsertionPoint, plate.Profile, plate.T1, plate.T2, plate.T3, plate.T4, plate.DimA, plate.DimB, plate.BearingMark, _tolerances, (new PrefixMaker(plate.BearingMark)), plate.IsM10BoltsRequired, plate.M);
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

            if (_tolerances.Count > 0)
            {
                using (var writer = new StreamWriter(@"C:\Users\Koshy\source\repos\ContourPlateBridge\ToleranceReport.csv"))
                using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(_tolerances);
                }
                Console.WriteLine("********* There are some tolerance problems - check the report.");
            }
            else
            {
                Console.WriteLine("No notable tolerances to report");
            }


            if (inconsistentMValues.Count > 0)
            {
                using (var writer = new StreamWriter(@"C:\Users\Koshy\source\repos\ContourPlateBridge\InconsistentMValues.csv"))
                using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(inconsistentMValues);
                }

                Console.WriteLine("********There are some inconsistent M values - please view the report generated.");
            }
            else
            {
                Console.WriteLine("No inconsistent values to report");
            }


            Console.WriteLine("Color code: Astronomical tolerance errors, color code: 5; HUGE tolerance errors, color code: 3. M values which are zero are color coded: 1, m values which are -10 color code: 2; The tolerance color codes override all others.");

            Console.ReadLine();
        }

        private static void AddToMReportIfRequired(PlateData plate, List<MReport> inconsistentMValues)
        {
            double tolerancesSummed = (plate.T4 + plate.T3) - (plate.T1 + plate.T2);
            if (tolerancesSummed > 0 )
            {
                if (plate.M > 0)
                {
                    // we are ok
                }
                else
                {
                    inconsistentMValues.Add(new MReport() { MReportValue = String.Format("{0}, (plate.T4 + plate.T3) - (plate.T1 + plate.T2) is {1} but m-value is going the other way: m-value: {2}  ", plate.BearingMark, tolerancesSummed, plate.M) });
                }
            }
            else
            {
                if (plate.M < 0)
                {
                    // we are ok
                }
                else
                {
                    inconsistentMValues.Add(new MReport() { MReportValue = String.Format("{0}, (plate.T4 + plate.T3) - (plate.T1 + plate.T2) is {1} but m-value is going the other way: m-value: {2}  ", plate.BearingMark, tolerancesSummed, plate.M) });
                }
            }
        }
    }
}
