using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace ContourPlateBridge
{
    class SmartContourPlate
    {
        private readonly double xOrigin;
        private readonly double yOrigin;

        double aWidth = 470;
        double bLength = 570;
        string name = "B81-P09-BER-01";
        string profile = "PL32";

        public SmartContourPlate(double xOrigin, double yOrigin)
        {
            this.xOrigin = xOrigin;
            this.yOrigin = yOrigin;
        }

        public void addContourPlate()
        {
            //Read in Width and length from CSV file
            // Put 20 plates in 1 row. Use a space of 300 between plates.
            // Name into the name field

            // we're going counter clock wise
            ContourPoint p1 = new ContourPoint(origin(), new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_LINE));
            p1.Chamfer.DZ1 = -12;

            ContourPoint p2 = new ContourPoint(bottomRightT2(), new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_LINE));
            p2.Chamfer.DZ1 = -6;

            ContourPoint p3 = new ContourPoint(topRightT3(), new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_LINE));
            p3.Chamfer.DZ1 = 0;

            ContourPoint p4 = new ContourPoint(topLeftT4(), new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_LINE));
            p4.Chamfer.DZ1 = -6;

            ContourPlate contourPlate = new ContourPlate();
            contourPlate.AddContourPoint(p1);
            contourPlate.AddContourPoint(p2);
            contourPlate.AddContourPoint(p3);
            contourPlate.AddContourPoint(p4);

            contourPlate.Finish = "HDG";
            contourPlate.Profile.ProfileString = profile;
            contourPlate.Material.MaterialString = "250";
            contourPlate.Name = name;
            contourPlate.Position.Depth = Position.DepthEnum.FRONT;
            contourPlate.Insert();

            insertBolt(contourPlate);
        }

        /// <summary>
        /// This will always be the bottom left
        /// </summary>
        /// <returns></returns>
        private Point bottomLeftT1()
        {
            return new Point(xOrigin, yOrigin, 0);
        }

        private Point origin()
        {
            return bottomLeftT1();
        }

        private Point bottomRightT2()
        {
            return new Point(xOrigin + aWidth, yOrigin, 0);
        }

        private Point topRightT3()
        {
            return new Point(xOrigin + aWidth, yOrigin + bLength, 0);
        }

        private Point topLeftT4()
        {
            return new Point(xOrigin, yOrigin + bLength);
        }

        private void insertBolt(ContourPlate contourPlate)
        {
            BoltArray boltArray = new BoltArray();

            boltArray.PartToBeBolted = contourPlate;
            boltArray.PartToBoltTo = contourPlate;

            boltArray.FirstPosition = new Point(0, 250, 0);
            boltArray.SecondPosition = new Point(100, 250, 0);

            boltArray.BoltSize = 16;
            boltArray.Tolerance = 3.00;
            boltArray.BoltStandard = "8.8S";
            boltArray.BoltType = BoltGroup.BoltTypeEnum.BOLT_TYPE_WORKSHOP;
            boltArray.CutLength = 105;

            boltArray.Length = 100;
            boltArray.ExtraLength = 15;
            boltArray.ThreadInMaterial = BoltGroup.BoltThreadInMaterialEnum.THREAD_IN_MATERIAL_NO;

            boltArray.Position.Depth = Position.DepthEnum.MIDDLE;
            boltArray.Position.Plane = Position.PlaneEnum.MIDDLE;
            boltArray.Position.Rotation = Position.RotationEnum.FRONT;

            boltArray.Bolt = true;
            boltArray.Washer1 = true;
            boltArray.Washer2 = true;
            boltArray.Washer3 = true;
            boltArray.Nut1 = true;
            boltArray.Nut2 = true;

            boltArray.Hole1 = true;
            boltArray.Hole2 = true;
            boltArray.Hole3 = true;
            boltArray.Hole4 = true;
            boltArray.Hole5 = true;

            boltArray.AddBoltDistX(100);
            boltArray.AddBoltDistX(90);
            boltArray.AddBoltDistX(80);

            boltArray.AddBoltDistY(70);
            boltArray.AddBoltDistY(60);
            boltArray.AddBoltDistY(50);

            if (!boltArray.Insert())
                Console.WriteLine("BoltArray Insert failed!");
        }
    }    
}
