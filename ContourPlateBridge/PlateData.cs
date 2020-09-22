using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace ContourPlateBridge
{
    class PlateDataMap : ClassMap<PlateData>
    {
        public PlateDataMap()
        {
            Map(m => m.BearingMark).Name("BEARING MARK").TypeConverter<TrimConverter>();
            Map(m => m.M).Name("M");
            Map(m => m.DimA).Name("DIM A");
            Map(m => m.DimB).Name("DIM B");
            Map(m => m.T1).Name("T1");
            Map(m => m.T2).Name("T2"); 
            Map(m => m.T3).Name("T3"); 
            Map(m => m.T4).Name("T4"); 
            Map(m => m.Profile).Name("PROFILE");
            Map(m => m.IsM10BoltsRequired).Name("M10").TypeConverterOption.BooleanValues(true, true, "YES", "Yes", "Y").TypeConverterOption.BooleanValues(false, false, "NO", "No", "N") ;
        }
    }


    public class TrimConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return text.Trim();
        }
    }

    class PlateData
    {   
        public string BearingMark { get; set; }       
        
        public double M { get; set; }
        
        public int DimA { get; set; }
                
        public int DimB { get; set; }
                
        public double T1 { get; set; }

        public double T2 { get; set; }

        public double T3 { get; set; }

        public double T4 { get; set; }
                
        public int Profile { get; set; }

        public bool IsM10BoltsRequired { get; set; }
    }
}


