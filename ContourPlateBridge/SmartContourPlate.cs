using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace ContourPlateBridge
{
    class SmartContourPlate
    {
        private readonly Model model;
        private readonly double xOrigin;
        private readonly double yOrigin;

        double aWidth; 
        double bLength;
        string name;
        
        int profile;
        string profileString;

        double t1;
        double t2;
        double t3;
        double t4;

        ContourPlate contourPlate;

        private List<ToleranceReport> _tolerances;

        public SmartContourPlate(Model model, double xOrigin, double yOrigin, int profile, int t1, int t2, int t3, int t4, double aWidth, double bLength, string bearingMark, List<ToleranceReport> _tolerances)
        {
            this.model = model;
            this.xOrigin = xOrigin;
            this.yOrigin = yOrigin;
            this.profile = profile;
            this.profileString = "PL" + profile;

            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
            this.t4 = t4;
            this.aWidth = aWidth;
            this.bLength = bLength;
            this.name = bearingMark;

            this._tolerances = _tolerances;
            
            this.contourPlate = new ContourPlate();
        }       

        public void addContourPlate()
        {
            //Read in Width and length from CSV file
            // Put 20 plates in 1 row. Use a space of 300 between plates.
            // Name into the name field

            // we're going counter clock wise
            ContourPoint p4Origin = new ContourPoint(origin(), new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_LINE));
            p4Origin.Chamfer.DZ1 = modt4Negative();

            ContourPoint p3 = new ContourPoint(bottomRightB3(), new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_LINE));
            p3.Chamfer.DZ1 = modt3Negative();

            ContourPoint p2 = new ContourPoint(topRightB2(), new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_LINE));
            p2.Chamfer.DZ1 = modt2Negative();

            ContourPoint p1 = new ContourPoint(topLeftB1(), new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_LINE));
            p1.Chamfer.DZ1 = modt1Negative();
                        
            contourPlate.AddContourPoint(p4Origin);
            contourPlate.AddContourPoint(p3);
            contourPlate.AddContourPoint(p2);
            contourPlate.AddContourPoint(p1);

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
            
            insertBoltsOnTaperedPlane();
        }

        public void AddUserDefinedAttributes()
        {
            contourPlate.SetUserProperty("USER_FIELD_1", String.Format("T1: {0}, T2: {1}, T3: {2}, T4: {3}", t1, t2, t3, t4));
            
            contourPlate.Modify();
        }

        private double modt1Negative()
        {
            return -1 * (profile - t1);
        }

        private double modt2Negative()
        {
            return -1 * (profile - t2PointCalculated().Z);
        }

        private double modt3Negative()
        {
            return -1 * (profile - t3);
        }

        private double modt4Negative()
        {
            return -1 * (profile - t4);
        }

        private void insertBoltsOnTaperedPlane()
        {
            Point t1_and_t3_midpoint = get_t3_t1_diagonal_point();

            Vector txAxis = getTXAxis();
            Vector tYAxis = getTYAxis();

            Vector zTVector = getTZAxis();
            Vector negativeZVector = zTVector * -1;

            Line line = new Line(t1_and_t3_midpoint, negativeZVector);
            GeometricPlane plane = new GeometricPlane(new Point(0, 0, 0), new Vector(0, 0, 1));

            Point intersectionPoint = Intersection.LineToPlane(line, plane);

            insertFinalBoltArray(intersectionPoint);

            checkIfPlanarAndColorizeAccordingly();
        }

        private void insertFinalBoltArray(Point inclinedOrigin)
        {
            TransformationPlane currentPlane =  model.GetWorkPlaneHandler().GetCurrentTransformationPlane();

            // get the new transformation plane
            TransformationPlane newPlane = new TransformationPlane(inclinedOrigin, getTXAxis(), getTYAxis());
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(newPlane);
            
            BoltArray boltArray = new BoltArray();

            boltArray.PartToBeBolted = contourPlate;
            boltArray.PartToBoltTo = contourPlate;

            boltArray.FirstPosition =  new Point(0,0,0);
            boltArray.SecondPosition = new Point(0, bLength / 2, 0);

            boltArray.BoltSize = 16;
            boltArray.Tolerance = 2;
            boltArray.BoltStandard = "8.8S";
            boltArray.BoltType = BoltGroup.BoltTypeEnum.BOLT_TYPE_WORKSHOP;
            boltArray.CutLength = -20;

            boltArray.Length = 100;
            boltArray.ExtraLength = 15;
            boltArray.ThreadInMaterial = BoltGroup.BoltThreadInMaterialEnum.THREAD_IN_MATERIAL_NO;

            boltArray.Position.Depth = Position.DepthEnum.MIDDLE;
            boltArray.Position.Plane = Position.PlaneEnum.MIDDLE;
            boltArray.Position.Rotation = Position.RotationEnum.BACK;

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

            boltArray.StartPointOffset.Dx = -150;

            boltArray.AddBoltDistX(300);
            
            boltArray.AddBoltDistY(200);

            if (!boltArray.Insert())
                Console.WriteLine("BoltArray Insert failed!");

            // revert back to the current plane
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlane);
        }

        private void checkIfPlanarAndColorizeAccordingly()
        {
            Vector txAxis = getTXAxis();
            Vector tYAxis = getTYAxis();
            Vector zTVector = getTZAxis();

            Line line = new Line(excelT2Point(), zTVector);
            GeometricPlane plane = new GeometricPlane(t1Point(), txAxis, tYAxis);

            Point intersectionPoint = Intersection.LineToPlane(line, plane);

            // get the distance between the intersection point and t4
            double distanceBetweenPoints = Distance.PointToPoint(intersectionPoint, excelT2Point());

            if (Math.Abs(distanceBetweenPoints) > 0.05)
            {
                GraphicsDrawer drawer = new GraphicsDrawer();
                drawer.DrawText(excelT2Point(), " t2 point is not planar", new Color(1.0, 0.5, 0.0));

                contourPlate.Class = "3";

                Console.WriteLine("\n " + name + " T2 is out by: " + distanceBetweenPoints);

                _tolerances.Add(new ToleranceReport() { ErrorString = name + " - T2 is out by: " + distanceBetweenPoints });
            }
            else
            {
                Console.WriteLine("\n " + name + " - T2 is out by: " + distanceBetweenPoints);
            }            
         }

        private Vector getTZAxis()
        {
            return getTXAxis().Cross(getTYAxis());               
                
        }

        // updated
        private Vector getTYAxis()
        {
            return getVector(t4Point(), t1Point());
        }

        // updated
        private Vector getTXAxis()
        {
            return getVector(t3Point(), t4Point());
        }

        // updated
        private Point get_t3_t1_diagonal_point()
        {
            Vector diagonal = getVector(t1Point(), t3Point());            
            Vector halfDiagonal = diagonal * 0.5;

            Point midpoint = new Point(t3Point().X + halfDiagonal.X, t3Point().Y + halfDiagonal.Y, t3Point().Z + halfDiagonal.Z);
            return midpoint;
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

        // updated
        private Point bottomLeftB4()
        {
            return new Point(xOrigin, yOrigin, 0);
        }

        // updated
        private Point origin()
        {
            return bottomLeftB4();
        }

        private Point flatPlateCentre()
        {
            double halfWidth = aWidth / 2;
            double halfLength = bLength / 2;
            return new Point(origin().X + halfWidth, origin().Y + halfLength, 0);
        }
        
        // updated
        private Point bottomRightB3()
        {
            return new Point(xOrigin + aWidth, yOrigin, 0);
        }

        // updated
        private Point topRightB2()
        {
            return new Point(xOrigin + aWidth, yOrigin + bLength, 0);
        }

        // updated
        private Point topLeftB1()
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

            boltArray.BoltSize = 12;
            boltArray.Tolerance = -3.5;
            boltArray.BoltStandard = "8.8S";
            boltArray.BoltType = BoltGroup.BoltTypeEnum.BOLT_TYPE_WORKSHOP;
            boltArray.CutLength = -70;

            boltArray.Length = 100;
            boltArray.ExtraLength = 15;
            boltArray.ThreadInMaterial = BoltGroup.BoltThreadInMaterialEnum.THREAD_IN_MATERIAL_NO;

            boltArray.Position.Depth = Position.DepthEnum.MIDDLE;
            boltArray.Position.Plane = Position.PlaneEnum.MIDDLE;
            boltArray.Position.Rotation = Position.RotationEnum.BACK;

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

            boltArray.BoltSize = 12;
            boltArray.Tolerance = -3.5;
            boltArray.BoltStandard = "8.8S";
            boltArray.BoltType = BoltGroup.BoltTypeEnum.BOLT_TYPE_WORKSHOP;
            boltArray.CutLength = -70;

            boltArray.Length = 100;
            boltArray.ExtraLength = 15;
            boltArray.ThreadInMaterial = BoltGroup.BoltThreadInMaterialEnum.THREAD_IN_MATERIAL_NO;

            boltArray.Position.Depth = Position.DepthEnum.MIDDLE;
            boltArray.Position.Plane = Position.PlaneEnum.MIDDLE;
            boltArray.Position.Rotation = Position.RotationEnum.BACK;

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

        // updated
        private Point verticalLeftBoltOrigin()
        {
            double xBoltOrigin = (origin().X + 32);
            double yboltOrigin = origin().Y + (bLength / 2) - 125;
            return new Point(xBoltOrigin, yboltOrigin, 0);
        }

        // updated
        private Point verticalRighBoltOrigin()
        {
            double xBoltOrigin = (origin().X + aWidth - 32);
            double yboltOrigin = origin().Y + (bLength / 2) - 125;
            return new Point(xBoltOrigin, yboltOrigin, 0);
        }

        // updated
        private Point horizontalBottomBoltOrigin()
        {
            double xBoltOrigin = (origin().X + aWidth / 2) - 85;
            double yboltOrigin = origin().Y + 32;

            return new Point(xBoltOrigin, yboltOrigin, 0);
        }

        // updated
        private Point horizontalTopBoltOrigin()
        {
            double xBoltOrigin = (origin().X + aWidth / 2) - 85;
            double yboltOrigin = (origin().Y + bLength - 32);
            return new Point(xBoltOrigin, yboltOrigin, 0);
        }
        

        // updated
        private Point t1Point()
        {
            return new Point(topLeftB1().X, topLeftB1().Y, t1);
        }  
        
        private Point t2PointCalculated()
        {
            Vector txAxis = getTXAxis();
            Vector tYAxis = getTYAxis();
            Vector zTVector = getTZAxis();

            Line line = new Line(excelT2Point(), zTVector);
            GeometricPlane plane = new GeometricPlane(t1Point(), txAxis, tYAxis);

            Point intersectionPoint = Intersection.LineToPlane(line, plane);

            return intersectionPoint;
        }

        private Point excelT2Point()
        {
            return new Point(topRightB2().X, topRightB2().Y, t2);
        }

        // updated
        private Point t3Point()
        {
            return new Point(bottomRightB3().X, bottomRightB3().Y, t3);
        }

        // updated
        private Point t4Point()
        {
            return new Point(bottomLeftB4().X, bottomLeftB4().Y, t4);
        }
    }
}


