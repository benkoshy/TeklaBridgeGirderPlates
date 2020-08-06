using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace ContourPlateBridge
{
    class Program
    {
        static void Main(string[] args)
        {
            Model teklaModel = new Model();

            if (teklaModel.GetConnectionStatus())
            {
                SmartContourPlate contourPlate = new SmartContourPlate(0,0);
                contourPlate.addContourPlate();
            }

            teklaModel.CommitChanges();
        }

        
    }
}
