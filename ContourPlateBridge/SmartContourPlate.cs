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

        int profile;
        string profileString;

        double t1 = -12;
        double t2 = -6;
        double t3 = 0;
        double t4 = -6;

        public SmartContourPlate(double xOrigin, double yOrigin, int profile = 32, int t1 = 20, int t2 = 26, int t3 = 32, int t4 = 26)
        {
            this.xOrigin = xOrigin;
            this.yOrigin = yOrigin;
            this.profile = profile;
            this.profileString = "PL" + profile;

            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
            this.t4 = t4;
        }

        public void addContourPlate()
        {
            //Read in Width and length from CSV file
            // Put 20 plates in 1 row. Use a space of 300 between plates.
            // Name into the name field

            // we're going counter clock wise
            ContourPoint p1 = new ContourPoint(origin(), new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_LINE));
            p1.Chamfer.DZ1 =  modt1Negative();

            ContourPoint p2 = new ContourPoint(bottomRightT2(), new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_LINE));
            p2.Chamfer.DZ1 = modt2Negative();

            ContourPoint p3 = new ContourPoint(topRightT3(), new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_LINE));
            p3.Chamfer.DZ1 = modt3Negative();

            ContourPoint p4 = new ContourPoint(topLeftT4(), new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_LINE));
            p4.Chamfer.DZ1 = modt4Negative();

            ContourPlate contourPlate = new ContourPlate();
            contourPlate.AddContourPoint(p1);
            contourPlate.AddContourPoint(p2);
            contourPlate.AddContourPoint(p3);
            contourPlate.AddContourPoint(p4);

            contourPlate.Finish = "HDG";
            contourPlate.Profile.ProfileString = profileString;
            contourPlate.Material.MaterialString = "250";
            contourPlate.Name = name;
            contourPlate.Position.Depth = Position.DepthEnum.FRONT;
            contourPlate.Insert();

            insertHorizontalBolts(contourPlate, horizontalBottomBoltOrigin());
            insertHorizontalBolts(contourPlate, horizontalTopBoltOrigin());

            insertVerticalBolts(contourPlate, verticalLeftBoltOrigin());
            insertVerticalBolts(contourPlate, verticalRighBoltOrigin());

            // insertColumn(origin());
            InsertColumnOnPlane();
        }

        private double modt1Negative()
        {
            return -1 * (profile - t1);
        }

        private double modt2Negative()
        {
            return -1 * (profile - t2);
        }

        private double modt3Negative()
        {
            return -1 * (profile - t3);
        }

        private double modt4Negative()
        {
            return -1 * (profile - t4);
        }

        private void InsertColumnOnPlane()
        {
            CoordinateSystem cs = new CoordinateSystem();
            cs.Origin = t1Point();
            cs.AxisX = getVector(t2Point(), t1Point());
            cs.AxisY = getVector(t4Point(), t1Point());            
            
            Matrix transformationMatrix = MatrixFactory.ToCoordinateSystem(cs);

            Point topOrigin = transformationMatrix.Transform(origin());

            insertColumn(topOrigin);
        }

        private Vector getVector(Point finalPoint, Point startingPoing)
        {
            return new Vector(finalPoint.X - startingPoing.X, finalPoint.Y - startingPoing.Y, finalPoint.Z - startingPoing.Z);
        }

        private void insertColumn(Point basePoint)
        {
            double height = 50;

            Point start = new Point(basePoint.X, basePoint.Y, basePoint.Z);
            Point end = new Point(basePoint.X, basePoint.Y, basePoint.Z + height);

            // Create a beam instance
            Beam column = new Beam(start, end);
            column.Profile.ProfileString = "D10";

            // Insert the beam in the model
            column.Insert();
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

        private Point flatPlateCentre()
        {
            double halfWidth = aWidth / 2;
            double halfLength = bLength / 2;
            return new Point(origin().X + halfWidth, origin().Y + halfLength, 0);
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

        private void insertVerticalBolts(ContourPlate contourPlate, Point bottomLeftBoltOrigin)
        {
            BoltArray boltArray = new BoltArray();

            boltArray.PartToBeBolted = contourPlate;
            boltArray.PartToBoltTo = contourPlate;

            boltArray.FirstPosition = bottomLeftBoltOrigin;
            boltArray.SecondPosition = bottomLeftBoltOrigin + new Vector(0, 100, 0);

            boltArray.BoltSize = 16;
            boltArray.Tolerance = 2.00;
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

            boltArray.AddBoltDistX(125);
            boltArray.AddBoltDistX(125);

            boltArray.AddBoltDistY(0);

            if (!boltArray.Insert())
                Console.WriteLine("BoltArray Insert failed!");
        }

        private void insertHorizontalBolts(ContourPlate contourPlate, Point bottomLeftBoltOrigin)
        {
            BoltArray boltArray = new BoltArray();

            boltArray.PartToBeBolted = contourPlate;
            boltArray.PartToBoltTo = contourPlate;

            boltArray.FirstPosition = bottomLeftBoltOrigin;
            boltArray.SecondPosition = bottomLeftBoltOrigin + new Vector(100, 0, 0);

            boltArray.BoltSize = 16;
            boltArray.Tolerance = 2.00;
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

            boltArray.AddBoltDistX(85);
            boltArray.AddBoltDistX(85);


            boltArray.AddBoltDistY(0);

            if (!boltArray.Insert())
                Console.WriteLine("BoltArray Insert failed!");
        }

        private Point verticalLeftBoltOrigin()
        {
            double xBoltOrigin = (origin().X + 32);
            double yboltOrigin = origin().Y + (bLength / 2) - 125;
            return new Point(xBoltOrigin, yboltOrigin, 0);
        }

        private Point verticalRighBoltOrigin()
        {
            double xBoltOrigin = (origin().X + aWidth - 32);
            double yboltOrigin = origin().Y + (bLength / 2) - 125;
            return new Point(xBoltOrigin, yboltOrigin, 0);
        }

        private Point horizontalBottomBoltOrigin()
        {
            double xBoltOrigin = (origin().X + aWidth / 2) - 85;
            double yboltOrigin = origin().Y + 32;

            return new Point(xBoltOrigin, yboltOrigin, 0);
        }

        private Point horizontalTopBoltOrigin()
        {
            double xBoltOrigin = (origin().X + aWidth / 2) - 85;
            double yboltOrigin = (origin().Y + bLength - 32);
            return new Point(xBoltOrigin, yboltOrigin, 0);
        }


        private Point t1Point()
        {
            return new Point(bottomLeftT1().X, bottomLeftT1().Y, t1);
        }

        private Point t2Point()
        {
            return new Point(bottomRightT2().X, bottomRightT2().Y, t2);
        }

        private Point t3Point()
        {
            return new Point(topRightT3().X, topRightT3().Y, t3);
        }

        private Point t4Point()
        {
            return new Point(topLeftT4().X, topLeftT4().Y, t4);
        }
    }
}

