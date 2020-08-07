using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace ContourPlateBridge
{
    class PlateData
    {
        [Name("BEARING MARK")]
        public string BearingMark { get; set; }

        [Name("DIM A")]
        public int DimA { get; set; }

        [Name("DIM B")]
        public int DimB { get; set; }

        [Name("T1")]
        public int T1 { get; set; }

        [Name("T2")]
        public int T2 { get; set; }

        [Name("T3")]
        public int T3 { get; set; }

        [Name("T4")]
        public int T4 { get; set; }

        [Name("PROFILE")]
        public int Profile { get; set; }
    }
}
