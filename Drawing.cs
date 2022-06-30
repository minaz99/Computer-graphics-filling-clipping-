using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
namespace Project4
{
    public static class Drawing
    {

        public static float[] getPoints(float x, float y)
        {
            float[] points = { x, y };
            return points;
        }

        static bool clipTest(float p, float q, ref float t1, ref float t2)
        {
            float r;
            bool retVal = true;

            //line entry point
            if (p < 0.0)
            {

                r = q / p;

                // line portion is outside the clipping edge
                if (r > t2)
                    retVal = false;

                else
                if (r > t1)
                    t1 = r;
            }

            else

            //line leaving point
            if (p > 0.0)
            {
                r = q / p;

                // line portion is outside     
                if (r < t1)
                    retVal = false;

                else if (r < t2)
                    t2 = r;
            }

            // p = 0, so line is parallel to this clipping edge 
            else

            // Line is outside clipping edge 
            if (q < 0.0)
                retVal = false;

            return (retVal);
        }
        
        

        public static (BitmapImage,Line) clipLine(BitmapImage img,Rectangle r, float[] p1, float[] p2,Color c, int filled, Color[][]colors)

         {
             float initialX = p1[0];
             float initialY = p1[1];
             float finalX = p2[0];
             float finalY = p2[1];
             float t1 = 0.0f, t2 = 1.0f;
             float dx = p2[0] - p1[0];
             float dy = p2[1] - p1[1];
             float newLine = 0;
            // Bitmap bImage = Image.BitmapImage2Bitmap(img);

             Line l = new Line(0, p1[0], p1[1], p2[0], p2[1], c, null, 0);
             // inside test wrt left edge
             if (clipTest(-dx, p1[0] - r.getTopLeft()[0], ref t1, ref t2))

                 // inside test wrt right edge 
                 if (clipTest(dx, r.getTopRight()[0] - p1[0], ref t1, ref t2))

                 {
                     dy = p2[1] - p1[1];

                     // inside test wrt bottom edge 
                     if (clipTest(-dy, p1[1] - r.getTopLeft()[1], ref t1, ref t2))

                         // inside test wrt top edge 
                         if (clipTest(dy, r.getBottomRight()[1] - p1[1], ref t1, ref t2))
                         {

                             if (t2 < 1.0)
                             {
                                 p2[0] = (int)(p1[0] + t2 * dx);
                                 p2[1] = (int)(p1[1]+ t2 * dy);
                             }

                             if (t1 > 0.0)
                             {
                                 p1[0] += (int)(t1 * dx);
                                 p1[1] += (int)(t1 * dy);
                             }


                             float newDx = p2[0] - p1[0];
                             float newDy = p2[1] - p1[1];

                             int steps =(int)(Math.Abs(newDx) > Math.Abs(newDy) ? Math.Abs(newDx) : Math.Abs(newDy));
                              l = new Line(0, p1[0], p1[1], p2[0], p2[1],c,getLinePoints(p1[0],p1[1],p2[0],p2[1]),steps + 1);
                             //img = DDA(img, p1[0], p1[1], p2[0], p2[1],c);

                             for (int i = 0; i <  l.getLineCords().Length;i++) {
                                 if (filled == 1)
                                 {
                                    // Console.WriteLine("yes sirsky");
                                     l.lineCordsColors[i][0] = colors[i][0];
                                     c = l.lineCordsColors[i][0];
                                 }
                                // bImage.SetPixel((int)l.getLineCords()[i][0], (int)l.getLineCords()[i][1], c);
                             }
                             newLine++;
                         }
                 }
             float nDx = p2[0] - p1[0];
             float nDy = p2[1] - p1[1];
            /* if(nDx == dx && nDy == dy)
             {
                 l = new Line(-2, 0, 0, 0, 0, c, null, 0);
             }*/
             (p1[0], p1[1]) = (initialX, initialY);
             (p2[0], p2[1]) = (finalX, finalY);
             return (img,l);

         }

        public static BitmapImage clearAllLines(BitmapImage img, List<ImageProperties> shapes)
        {            
            foreach(ImageProperties shape in shapes)
            {
                if(shape.classNo == 0)
                {
                    img = DDA(img, shape.getInitialPoints()[0], shape.getInitialPoints()[1], shape.getFinalPoints()[0], shape.getFinalPoints()[1], Color.FromArgb(255,255,255));
                }
            }
            return img;
        }

      
        public static BitmapImage DDA(BitmapImage image, float X0, float Y0, float X1, float Y1, Color c)
        {
            Bitmap img = Image.BitmapImage2Bitmap(image);
            float dx = X1 - X0;
            float dy = Y1 - Y0;

            int steps = (int)(Math.Abs(dx) > Math.Abs(dy) ? Math.Abs(dx) : Math.Abs(dy));
            float Xinc = dx / (float)steps;
            float Yinc = dy / (float)steps;

            float X = X0;
            float Y = Y0;
            for (int i = 0; i <= steps; i++)
            {
                Image.draw(img, X, Y, c);
                X += Xinc;
                Y += Yinc;
            }
            return Image.ToBitmapImage(img);
        }

        public static float[][] getLinePoints(float X0, float Y0, float X1, float Y1)
        {
            float dx = X1 - X0;
            float dy = Y1 - Y0;

            int steps =(int)(Math.Abs(dx) > Math.Abs(dy) ? Math.Abs(dx) : Math.Abs(dy));
            float Xinc = dx / (float)steps;
            float Yinc = dy / (float)steps;

            float X = X0;
            float Y = Y0;

            float[][] cords = new float[steps + 1][];
            for (int i = 0; i <= steps; i++)
            {
                cords[i] = new float[2];
                cords[i][0] = (int)X;
                cords[i][1] = (int)Y;
                X += Xinc;
                Y += Yinc;
            }
            return cords;
        }

        public static BitmapImage drawRectangle(BitmapImage img, float[] pointI, float[] pointF, Color c, Rectangle rectangle)
        {
            float dx = pointF[0] - pointI[0];
            float dy = pointF[1] - pointI[1];

            int steps = (int)(Math.Abs(dx) > Math.Abs(dy) ? Math.Abs(dx) : Math.Abs(dy));
            steps += 1;
            img = DDA(img, pointI[0], pointI[1], pointF[0], pointI[1], c);
            Line l1 = new Line(0, pointI[0], pointI[1], pointF[0], pointI[1], c, getLinePoints(pointI[0], pointI[1], pointF[0], pointI[1]), steps);
            img = DDA(img, pointI[0], pointI[1], pointI[0], pointF[1], c);
            Line l2 = new Line(0, pointI[0], pointI[1], pointI[0], pointF[1], c, getLinePoints(pointI[0], pointI[1], pointI[0], pointF[1]), steps);
            img = DDA(img, pointF[0], pointF[1], pointI[0], pointF[1], c);
            Line l3 = new Line(0, pointF[0], pointF[1], pointI[0], pointF[1], c, getLinePoints(pointF[0], pointF[1], pointI[0], pointF[1]), steps);
            img = DDA(img, pointF[0], pointF[1], pointF[0], pointI[1], c);
            Line l4 = new Line(0, pointF[0], pointF[1], pointF[0], pointI[1], c, getLinePoints(pointF[0], pointF[1], pointF[0], pointI[1]), steps);
            rectangle.addLine(l1);
            rectangle.addLine(l2);
            rectangle.addLine(l3);
            rectangle.addLine(l4);
           

            return img;
        }

        public static Rectangle getRectangle(List<ImageProperties> shapes, float[] points)
        {
            foreach(ImageProperties shape in shapes)
            {
                if (shape.classNo == 1)
                {
                    if (points[0] >= shape.getTopLeft()[0] && points[0] <= shape.getTopRight()[0] && points[1] >= shape.getTopLeft()[1] && points[1] <= shape.getBottomRight()[1])
                    {
                        return (Rectangle)shape;
                    }
                }
            }
            return null;
        }

        public static int typeOfResize(Rectangle r, float[] points)
        {
            //  List<Line> linesOfRectangle = r.getShape();
            int foundType = -4; //if found is line return 0, if found is point return 1,
                                //if found is rectangle as a whole return 2
            if ((r.getTopRight()[0] == points[0] + 1 || r.getTopRight()[0] == points[0] - 1 || r.getTopRight()[0] == points[0]) && (points[1] - 1 == r.getTopRight()[1] || points[1] + 1 == r.getTopRight()[1] || points[1] == r.getTopRight()[1]))
            {
                foundType = 3;
                //Console.WriteLine("found a top right point");
                return foundType;
            }
            if ((r.getTopLeft()[0] == points[0] + 1 || r.getTopLeft()[0] == points[0] - 1 || r.getTopLeft()[0] == points[0]) && (points[1] - 1 == r.getTopLeft()[1] || points[1] + 1 == r.getTopLeft()[1] || points[1] == r.getTopLeft()[1]))
            {
                foundType = 2;
                // Console.WriteLine("found a top left point");
                return foundType;
            }
            if ((r.getBottomLeft()[0] == points[0] + 1 || r.getBottomLeft()[0] == points[0] - 1 || r.getBottomLeft()[0] == points[0]) && (points[1] - 1 == r.getBottomLeft()[1] || points[1] + 1 == r.getBottomLeft()[1] || points[1] == r.getBottomLeft()[1]))
            {
                foundType = 4;
                //Console.WriteLine("found a bottom left point");
                return foundType;
            }
            if ((r.getBottomRight()[0] == points[0] + 1 || r.getBottomRight()[0] == points[0] - 1 || r.getBottomRight()[0] == points[0]) && (points[1] - 1 == r.getBottomRight()[1] || points[1] + 1 == r.getBottomRight()[1] || points[1] == r.getBottomRight()[1]))
            {
                foundType = 5;
                // Console.WriteLine("found a bottom right point");
                return foundType;
            }

            if (points[0] > r.getTopLeft()[0] + 10 && points[0] < r.getTopRight()[0] - 10 && points[1] > r.getTopLeft()[1] + 10 && points[1] < r.getBottomRight()[1] - 10)
            {
                foundType = 1;
                return foundType; //this is for moving rectangle as a whole
            }

            if (points[1] > r.getTopLeft()[1] && points[1] < r.getBottomLeft()[1] && (points[0] == r.getTopLeft()[0] || points[0] - 1 == r.getTopLeft()[0] || points[0] + 1 == r.getTopLeft()[0]))
            {
                foundType = 0; //left 
                return foundType;
            }

            if (points[1] > r.getTopRight()[1] && points[1] < r.getBottomRight()[1] && (points[0] == r.getTopRight()[0] || points[0] - 1 == r.getTopRight()[0] || points[0] + 1 == r.getTopRight()[0]))
            {
                foundType = -2; //right
                return foundType;
            }

            if (points[0] > r.getTopLeft()[0] && points[0] < r.getTopRight()[0] && (points[1] == r.getTopLeft()[1] || points[1] - 1 == r.getTopLeft()[1] || points[0] + 1 == r.getTopLeft()[0]))
            {
                foundType = -1; //top
                return foundType;
            }

            if (points[0] > r.getBottomLeft()[0] && points[0] < r.getBottomRight()[0] && (points[1] == r.getBottomLeft()[1] || points[1] - 1 == r.getBottomLeft()[1] || points[0] + 1 == r.getBottomLeft()[0]))
            {
                foundType = -3; //bottom
                return foundType;
            }
            return foundType;
        }


        public static BitmapImage clearRectangle(BitmapImage img, Rectangle rectangle)
        {
            foreach (Line l in rectangle.getShape())
            {
                img = DDA(img, l.getInitialPoints()[0], l.getInitialPoints()[1], l.getFinalPoints()[0], l.getFinalPoints()[1], Color.FromArgb(255, 255, 255));
            }
            return img;
        }

        public static BitmapImage clearEdges(BitmapImage img,float[] pI, float[] pF)
        {           
                img = DDA(img,pI[0], pI[1], pF[0], pF[1],Color.FromArgb(255,255,255));            
            return img;
        }

        public static (BitmapImage, float[]) changeVertex(BitmapImage img, float[] newPoint, Rectangle rec, int vertexPos)
        {
            //2- topLeft, 3-topRight, 4-bottomLeft, 5-bottomRight
            float[] initialVertex = new float[2];
            if (vertexPos == 2)
                initialVertex = rec.getBottomRight();
            else if (vertexPos == 3)
                initialVertex = rec.getBottomLeft();
            else if (vertexPos == 4)
                initialVertex = rec.getTopRight();
            else initialVertex = rec.getTopLeft();
            img = clearRectangle(img, rec);
            return (img, initialVertex);
        }

        public static (BitmapImage,float[],float[],int) changeEdge(BitmapImage img, Rectangle rec, float[] newPoint, int edgeToChange)
        {
            //0- left edge, -1 -top edge, -2 -right edge, -3 -bottom edge
            float[] initialP = new float[2];
            float[] finalP = new float[2];
            int xORy = -2; //x- 0, y- 1 which will change

           if(edgeToChange == 0)
            {
                img = clearEdges(img, rec.getTopLeft(), rec.getBottomLeft());
                img = clearEdges(img, rec.getTopLeft(), rec.getTopRight());
                img = clearEdges(img, rec.getBottomLeft(), rec.getBottomRight());
                initialP = rec.getBottomRight();
                finalP = rec.getTopLeft();
                xORy = 0;
            }
           else if (edgeToChange == -1)
            {
                img = clearEdges(img,rec.getTopLeft(), rec.getTopRight());
                img = clearEdges(img, rec.getTopLeft(), rec.getBottomLeft());
                img = clearEdges(img, rec.getTopRight(), rec.getBottomRight());
                initialP = rec.getBottomRight();
                finalP = rec.getTopLeft();
                xORy = 1;
            }
            else if(edgeToChange == -2)
            {
                img = clearEdges(img, rec.getTopLeft(), rec.getTopRight());
                img = clearEdges(img, rec.getBottomLeft(), rec.getBottomRight());
                img = clearEdges(img, rec.getTopRight(), rec.getBottomRight());
                initialP = rec.getBottomLeft();
                finalP = rec.getTopRight();
                xORy = 0;

            }
            else if(edgeToChange == -3)
            {
                img = clearEdges(img, rec.getBottomLeft(), rec.getBottomRight());
                img = clearEdges(img, rec.getBottomLeft(), rec.getTopLeft());
                img = clearEdges(img, rec.getBottomRight(), rec.getTopRight());
                initialP = rec.getTopRight();
                finalP = rec.getBottomLeft();
                xORy = 1;
            }            
            return (img,initialP,finalP,xORy);
        }

        public static ImageProperties[] resizeArray(ImageProperties[] shapes, int shapesCount, int indexRemoved)
        {
           
            for (int i = indexRemoved; i < shapesCount - 1; i++)
            {
                shapes[i] = shapes[i + 1];
            }
            return shapes;
        }

        public static List<float> getMinMaxPoints(List<Line>lines)
        {
            float minX = lines.Min(l=> l.pointsI[0]);
            float minY = lines.Min(l => l.pointsI[1]);
            float maxX = lines.Max(l => l.pointsF[0]);
            float maxY = lines.Max(l => l.pointsF[1]);
            List<float> points = new List<float>();
            points.Add(minX);
            points.Add(minY);
            points.Add(maxX);
            points.Add(maxY);
           
            return points;
        }

        public static int changeVertex(BitmapImage img,Polygon p, float[] point)
        {
            float currentDiff = 1200;
            int lineIndex = -1;
            foreach(Line l in p.getShape())
            {
                if(Math.Abs(l.pointsI[0] - point[0]) < currentDiff)
                {
                    currentDiff = Math.Abs(l.getInitialPoints()[0] - point[0]);
                    lineIndex = p.getShape().IndexOf(l);
                    Console.WriteLine("saved index here" + lineIndex);
                }
            }
            return lineIndex;
        }

        public static Polygon getPolygon(BitmapImage img, List<ImageProperties> shapes, float[] point)
        {
            List<float> points = new List<float>();
            foreach(ImageProperties shape in shapes)
            {
                if(shape.classNo == 2 || shape.classNo == 3)
                {
                    points = getMinMaxPoints(shape.getShape());
                    //  Console.WriteLine($"({points[0]},{points[1]}) ({points[2]},{points[3]}) point at ({point[0]},{point[1]})");

                    if(point[0] > points[0] && point[0] < points[2] && point[1] > points[1] && point[1] < points[3])
                    {
                        return (Polygon)shape;
                    }
                
                }
            }
            return null;
        }        

        public static BitmapImage movePolygon(BitmapImage img, Polygon polygon)
        {
            foreach(Line line in polygon.getShape())
            {
                img = clearEdges(img, line.getInitialPoints(), line.getFinalPoints());
            }
            return img;
        }

        public static (BitmapImage, Polygon) drawPolygon(BitmapImage img, List<Line> lines, float[] pointF, Color c, int imgFilled = 0)
        {
            Bitmap bimg = Image.BitmapImage2Bitmap(img);
            Polygon polygon = new Polygon(2, c);

            //if(imgFilled == 1)
            //Console.WriteLine(imgFilled);
            for (int j = 0; j < lines.Count; j++) {

                float dx = lines[j].getFinalPoints()[0] + pointF[0] - lines[j].getInitialPoints()[0] + pointF[0];
                float dy = lines[j].getFinalPoints()[1] + pointF[1] - lines[j].getInitialPoints()[1] + pointF[1];
                
                int steps = (int)(Math.Abs(dx) > Math.Abs(dy) ? Math.Abs(dx) : Math.Abs(dy));
                polygon.addLine(new Line(0, lines[j].getInitialPoints()[0] + pointF[0], lines[j].getInitialPoints()[1] + pointF[1], lines[j].getFinalPoints()[0] + pointF[0], lines[j].getFinalPoints()[1] + pointF[1], c, getLinePoints(lines[j].getInitialPoints()[0] + pointF[0], lines[j].getInitialPoints()[1] + pointF[1], lines[j].getFinalPoints()[0] + pointF[0], lines[j].getFinalPoints()[1] + pointF[1]), steps + 1));
                polygon.getShape()[j].lineCordsColors = lines[j].lineCordsColors;
                if(lines[j].lineCords != null)
                for (int i = 0; i < lines[j].lineCords.Length; i++) {
                    if (imgFilled == 1) c = lines[j].lineCordsColors[i][0];
                    bimg.SetPixel((int)(lines[j].lineCords[i][0] + pointF[0]), (int)(lines[j].lineCords[i][1] + pointF[1]), c);
                }
            }
               // img = DDA(img, line.getInitialPoints()[0] + pointF[0], line.getInitialPoints()[1] + pointF[1], line.getFinalPoints()[0] + pointF[0], line.getFinalPoints()[1] + pointF[1],c);
            
            return (Image.ToBitmapImage(bimg),polygon);
        }

        public static (ImageProperties[],int) reShapeArray(ImageProperties[] shapes, int objectCounter ,int indexCount,ImageProperties[] objects)
        {
            List <ImageProperties> dataList = shapes.ToList();
            for (int i = 0; i < indexCount; i++)
            {
                dataList.Remove(objects[i]);
                objectCounter--;
                
            }
            shapes = dataList.ToArray();
            return (shapes,objectCounter);
        }

        public static Rectangle findRec(List<ImageProperties> shapes)
        {
            foreach(ImageProperties shape in shapes)
            {
                if (shape.classNo == 1) return (Rectangle)shape;
            } 
            return null;
        }

        public static (BitmapImage,Rectangle) moveRectangle(BitmapImage img,Rectangle r, float[] newOffset, Color c)
        {
            float[] newInitialPoint = new float[2];
            float[] newFinalPoint = new float[2];
            (newInitialPoint[0],newInitialPoint[1]) = (r.getInitialPoints()[0] + newOffset[0], r.getInitialPoints()[1] + newOffset[1]);
            (newFinalPoint[0], newFinalPoint[1]) = (r.getFinalPoints()[0] + newOffset[0], r.getFinalPoints()[1] + newOffset[1]);

            Rectangle rectangle = new Rectangle(newInitialPoint, newFinalPoint, 1, c);
            // img = drawRectangle(img, newInitialPoint, newFinalPoint, c, rectangle);
           // img = DDA(img, newInitialPoint[0], newFinalPoint[1], newFinalPoint[0], newInitialPoint[1], c);
            return (img, rectangle);
        }

        public static BitmapImage clearAllPolygons(BitmapImage img, List<ImageProperties> shapes)
        {
            foreach (ImageProperties shape in shapes)
            {
                if (shape.classNo == 2)
                {
                    foreach (Line l in shape.getShape())
                    {
                        img = DDA(img, l.getInitialPoints()[0], l.getInitialPoints()[1], l.getFinalPoints()[0], l.getFinalPoints()[1], Color.FromArgb(255, 255, 255));
                    }
                }
            }
            return img;
        }


        public static BitmapImage clearPolygon(BitmapImage img, Polygon shapes)
        {
            Bitmap bImg = Image.BitmapImage2Bitmap(img);
            List<Line> lines = shapes.getShape();
            foreach(Line l in lines)
            {
                for (int i = 0; i < l.lineCords.Length; i++)
                    bImg.SetPixel((int)l.lineCords[i][0], (int)l.lineCords[i][1], Color.FromArgb(255, 255, 255));
            }
            
            return Image.ToBitmapImage(bImg);
        }

        public static BitmapImage clearLine(BitmapImage img,Line l)
        {
            Bitmap bImg = Image.BitmapImage2Bitmap(img);
            foreach(float[] point in l.getLineCords())
            {
                bImg.SetPixel((int)point[0], (int)point[1], Color.FromArgb(255, 255, 255));
            }
            return Image.ToBitmapImage(bImg);
        }

    }
}
    

