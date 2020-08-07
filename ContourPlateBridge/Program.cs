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
            Model model = new Model();

            if (model.GetConnectionStatus())
            {
                SmartContourPlate contourPlate = new SmartContourPlate(model, 0,0);
                contourPlate.addContourPlate();
            }

            model.CommitChanges();
        }

        
    }
}
