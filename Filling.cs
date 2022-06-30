using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace Project4
{
    public static class Filling
    {
        public static (float, float) getMin(Line x, Line y)
        {
            if (x.pointsI[1] > y.pointsI[1]) return (y.pointsI[1], y.pointsI[0]);
            else return (x.pointsI[1], x.pointsI[0]);
        }
        public static float getMax(float x, float y)
        {
            if (x > y) return x;
            else return y;
        }
        public static float[] globalEdgeTableInitializing(Line pointA, Line pointB, int val)
        {
            float[] components = new float[3];
            (components[0], components[1], components[2]) = (-1, -1, -1);
            var minItems = getMin(pointA, pointB);
            float maxY = getMax(pointA.pointsI[1], pointB.pointsI[1]);
            float minY = minItems.Item1;
            float xForMinY = minItems.Item2;
            //Console.WriteLine($"({pointA.pointsI[0]}, {pointA.pointsI[1]})");
            //Console.WriteLine($"({pointB.pointsI[0]}, {pointB.pointsI[1]})");           
            if (val == -1) return components;
            else
            {
                float inverseGradient = (pointB.pointsI[0] - pointA.pointsI[0]) / (pointB.pointsI[1] - pointA.pointsI[1]);
                components[0] = maxY;
                components[1] = xForMinY;
                components[2] = inverseGradient;
                return components;
            }
        }

        public static bool pointAlreadyExists(List<int[]> alphabets, float x, float y)
        {
            foreach (int[] edge in alphabets)
            {
                if ((edge[0] == x && edge[1] == y) || (edge[0] == y && edge[1] == x)) return true;
            }
            return false;
        }

         public static List<float[]>[] reOrderEdgeTable(List<float[]>[] GET, int currentNode)
        {
            float yVAL1 = GET[currentNode][0][0];
            float yVal2 = GET[currentNode][1][0];
            if (yVAL1 != -1 && yVal2 != -1)
            {
                if(yVAL1 > yVal2)
                {
                    float[] temp = GET[currentNode][0];
                    GET[currentNode][0] = GET[currentNode][1];
                    GET[currentNode][1] = temp;
                }
            }
            return GET;
        }
        public static List<float[]>[] createGlobalEdgeTable(Polygon polygon)
        {
            //we start by creating global edge which ranges from min y to max y
            float minY = polygon.getShape().Min(y => y.pointsI[1]);
            float maxY = polygon.getShape().Max(y => y.pointsF[1]);
            //Console.WriteLine($"Max is: {maxY} and minY is {minY}"});
            List<float[]>[] GET = new List<float[]>[(int)maxY + 1];
            List<Line> sortedLines = polygon.getShape();
            foreach (Line l in sortedLines) Console.WriteLine(l.pointsI[0] + " ," + l.pointsF[0]);
            List<int[]> alphabetPoints = new List<int[]>();
            //Console.WriteLine(sortedLines.Count);
            for (int i = 0; i <= maxY; i++)
            {
                GET[i] = new List<float[]>();
            }

            int nextPoint = 0;
            // int currentLine = 0;
            //foreach(Line l in sortedLines) Console.WriteLine($"({l.pointsI[0]}, {l.pointsI[1]})");

            for (int i = 0; i <= maxY; i++)
            {
                int processed = 0;
                foreach (int[] letter in alphabetPoints) Console.WriteLine(letter[0] + " " + letter[1]);
                Console.WriteLine("done");
                foreach (Line l in sortedLines)
                {
                    //if (processed == 1) break;

                    // processed = 1;
                    for (int m = 0; m < 2; m++)
                    {
                        int index = sortedLines.IndexOf(l);
                        if (index == 0)
                        {
                            if (m == 0) nextPoint = sortedLines.Count - 1;
                            if (m == 1) nextPoint = index + 1;
                        }
                        else if (index > 0 && index < sortedLines.Count - 1)
                        {
                            if (m == 0) nextPoint = index - 1;
                            if (m == 1) nextPoint = index + 1;
                        }
                        else if (index == sortedLines.Count - 1)
                        {
                            if (m == 0) nextPoint = index - 1;
                            if (m == 1) nextPoint = 0;
                        }
                        if (l.pointsI[1] == i && pointAlreadyExists(alphabetPoints, sortedLines.IndexOf(l), nextPoint) == false)
                        {
                            if (l.pointsI[1] == i && pointAlreadyExists(alphabetPoints, index, nextPoint) == false)
                            {
                                processed++;
                                int[] letter = { index, nextPoint };
                                alphabetPoints.Add(letter);
                                GET[i].Add(globalEdgeTableInitializing(l, sortedLines[nextPoint], 1));
                            }
                        }

                    }
                }
                if (processed < 2)
                {
                    if (processed == 0)
                    {
                        GET[i].Add(globalEdgeTableInitializing(sortedLines[0], sortedLines[nextPoint], -1));
                        GET[i].Add(globalEdgeTableInitializing(sortedLines[0], sortedLines[nextPoint], -1));
                    }
                    else if (processed == 1) GET[i].Add(globalEdgeTableInitializing(sortedLines[0], sortedLines[nextPoint], -1));

                }
                GET = reOrderEdgeTable(GET, i);
            }
            return GET;
        }


        public static float getMaxYOfAll(List<Line> lines)
        {
            float maxY = -1;
            foreach(Line l in lines)
            {
                if (l.getInitialPoints()[1] > maxY) maxY = l.getInitialPoints()[1];
            }
            return maxY;
        }

        public static float getMinYOfAll(List<Line> lines)
        {
            float minY = 2000;
            foreach (Line l in lines)
            {
                if (l.getInitialPoints()[1] < minY) minY = l.getInitialPoints()[1];
            }
            return minY;
        }

        public static int nodeToDelete(List<float[]> AET, int currentIndex)
        {
            if (AET[0][0] == currentIndex)
            {
                return 0;
            }
            else if (AET[1][0] == currentIndex) return 1;
            Console.WriteLine("well well what do we have here");
            return -1;
        }

        public static List<float[]> reshiftArray(List<float[]> AET, float[] nodeToAdd, int i)
        {
            if (nodeToDelete(AET, i) == 0)
            {
                AET[0] = nodeToAdd;
            }
            else if (nodeToDelete(AET, i) == 1)
            {
                float[] tempNode = AET[0];
                AET[0] = nodeToAdd;
                AET[1] = tempNode;
            }
            return AET;
        }
        public static (List<float[]>, List<float[]>, List<float[]>) createActiveEdgeTable(List<float[]>[] GET, float minYToStartReadingFrom, float yMax)
        {
            List<float[]> initialPoints = new List<float[]>();
            List<float[]> finalPoints = new List<float[]>();
            List<float[]> AET = new List<float[]>();
            AET.Add(GET[(int)minYToStartReadingFrom][0]);
            AET.Add(GET[(int)minYToStartReadingFrom][1]);
            float[] initialPoint = new float[2];
            float[] finalPoint = new float[2];
            (initialPoint[0], initialPoint[1]) = (AET[0][1], minYToStartReadingFrom);
            (finalPoint[0], finalPoint[1]) = (AET[1][1], minYToStartReadingFrom);
            initialPoints.Add(initialPoint);
            finalPoints.Add(finalPoint);
            //foreach (int[] n in AET)
            //{
            //    Console.WriteLine($"[{n[0]}|{n[1]}|{n[2]}]");
            //}

            for (int i = (int)(minYToStartReadingFrom + 1); i <= yMax; i++)
            {
                if (AET[0][0] == AET[1][0] && AET[0][1] == AET[1][1]) break;
                for (int j = 0; j < 2; j++)
                {
                    // int dont = -1;
                    if (GET[i][j][0] != -1)
                    {
                        // dont = 0;
                        // AET.RemoveAt(nodeToDelete(AET, i));                                 
                        AET = reshiftArray(AET, GET[i][j], i);
                        foreach (float[] n in AET)
                        {
                            Console.WriteLine($"{i}[{n[0]}|{n[1]}|{n[2]}]");
                        }
                    }
                    else
                    {
                        if (j == 0)
                        {
                            AET[0][1] += AET[0][2];
                            AET[1][1] += AET[1][2];
                            //Console.WriteLine(AET[0][1] + " " + AET[0][2]);
                            foreach (float[] n in AET)
                            {

                                Console.WriteLine($"{i}[{n[0]}|{n[1]}|{n[2]}]");
                            }
                        }
                    }
                    if (GET[i][j][0] != -1 || j == 0)
                    {
                        initialPoint = new float[2];
                        finalPoint = new float[2];
                        (initialPoint[0], initialPoint[1]) = (AET[0][1], i);
                        (finalPoint[0], finalPoint[1]) = (AET[1][1], i);
                        initialPoints.Add(initialPoint);
                        finalPoints.Add(finalPoint);
                        //foreach (int[] n in AET)
                        //{

                        //    Console.WriteLine($"{i}[{n[0]}|{n[1]}|{n[2]}]");
                        //}
                    }

                }
            }

            return (initialPoints, finalPoints, AET);
        }


        public static BitmapImage fillPolygon(BitmapImage img,Polygon p, Color c)
        {
            Bitmap bImg = Image.BitmapImage2Bitmap(img);
            List<Line> lines = p.getShape();
            foreach(Line l in lines)
            {
                for(int i = 0; i < l.lineCords.Length; i++)
                {
                    bImg.SetPixel((int)l.lineCords[i][0],(int)l.lineCords[i][1],c);                    
                }
            }
           
            return Image.ToBitmapImage(bImg);
        }

        public static BitmapImage DDAForFillingImages(BitmapImage image, float X0, float Y0, float X1, float Y1, Bitmap imgToFillFrom)
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
                Image.draw(img, X, Y, imgToFillFrom.GetPixel((int)X,(int)Y));
                X += Xinc;
                Y += Yinc;
            }
            return Image.ToBitmapImage(img);
        }

        public static int pixelReCalibrate(int pixelVal,Bitmap img, int hOrW)
        {
            if (hOrW == 1) //means x axis
                if (pixelVal > img.Width - 1) return img.Width - 1;
                else if (pixelVal < 0) return 0;
                else return pixelVal;
            else
            {
                if (pixelVal > img.Height - 1) return img.Height - 1;
                else if (pixelVal < 0) return 0;
                else return pixelVal;
            }
        }

        public static (BitmapImage,Polygon) fillPolygonWithImg(BitmapImage img, Polygon p,BitmapImage imageToFill)
        {
            //  int xi = 0;
            //  int yi = 0;
            Bitmap imgOriginal = Image.BitmapImage2Bitmap(img);
            Bitmap imgF = Image.BitmapImage2Bitmap(imageToFill);
            List<Line> lines = p.getShape();
            //hanym
            foreach (Line l in lines)
            {
                l.lineCordsColors = new Color[l.lineCords.Length][];
                
                for (int i = 0; i < l.lineCords.Length; i++)
                {
                    l.lineCordsColors[i] = new Color[1];                    
                    int x = pixelReCalibrate((int)l.lineCords[i][0], imgF, 1);
                    int y = pixelReCalibrate((int)l.lineCords[i][1], imgF, 0);
                    l.lineCordsColors[i][0] = imgF.GetPixel(x, y);
                    imgOriginal.SetPixel((int)l.lineCords[i][0], (int)l.lineCords[i][1], imgF.GetPixel(x, y));
                    //img = DDAForFillingImages(img, initialPoints[i][0], initialPoints[i][1], finalPoints[i][0], finalPoints[i][1], imgF);                 
                }

            }
            return (Image.ToBitmapImage(imgOriginal),p);
        }

        public static void setColorPointsFromImage(BitmapImage img,Polygon p)
        {
            Bitmap imgF = Image.BitmapImage2Bitmap(img);
            List<Line> lines = p.getShape();
            foreach(Line l in lines)
            {
                l.lineCordsColors = new Color[l.lineCords.Length][];
                for (int i = 0; i < l.lineCords.Length; i++)
                {
                    l.lineCordsColors[i] = new Color[1];
                    int x = pixelReCalibrate((int)l.lineCords[i][0], imgF, 1);
                    int y = pixelReCalibrate((int)l.lineCords[i][1], imgF, 0);
                    l.lineCordsColors[i][0] = imgF.GetPixel(x, y);
                   // imgOriginal.SetPixel((int)l.lineCords[i][0], (int)l.lineCords[i][1], imgF.GetPixel(x, y));
                    //img = DDAForFillingImages(img, initialPoints[i][0], initialPoints[i][1], finalPoints[i][0], finalPoints[i][1], imgF);                 
                }
            }            
        }

        public static Polygon generateFilledPolygon(Polygon p)
        {
             Polygon pNew = new Polygon(2, p.c,p.filled,p.polygonImg);
            Bitmap imgF = Image.BitmapImage2Bitmap(p.polygonImg);
             pNew.linesOfPolygon = p.linesOfPolygon;
            foreach (Line l in pNew.linesOfPolygon) {
                Color[][] colorsForPoints = new Color[l.lineCords.Length][];
                for(int i = 0; i < l.lineCords.Length; i++)
                {
                    l.lineCordsColors[i] = new Color[1];
                    int x = pixelReCalibrate((int)l.lineCords[i][0], imgF, 1);
                    int y = pixelReCalibrate((int)l.lineCords[i][1], imgF, 0);
                    //l.lineCordsColors[i][0] = imgF.GetPixel(x, y);
                }
            }
            return pNew;
        }

        public static BitmapImage clearImage(BitmapImage img,List<ImageProperties> shapes)
        {
            Bitmap bImg = Image.BitmapImage2Bitmap(img);
            foreach (ImageProperties shape in shapes)
            {
                foreach(Line l in shape.getShape())
                    for(int i = 0; i < l.lineCords.Length;i++)
                bImg.SetPixel((int)l.lineCords[i][0],(int) l.lineCords[i][1], Color.FromArgb(255, 255, 255));
            }
            return Image.ToBitmapImage(bImg);
        }

        public static Polygon preparePolygonForFilling(Polygon p)
        {
            Polygon newP = new Polygon(2, p.c);
            newP = p;
            for (int i = 0; i < newP.getShape().Count;i++)
            {
                for (int j = 0; j < newP.getShape().Count; j++)
                {
                    newP.getShape()[i].lineCordsColors[j] = new Color[1];
                    newP.getShape()[i].lineCordsColors[j][0] = p.getShape()[i].lineCordsColors[j][0];
                }
            }
            return newP;
        }

        public static (BitmapImage,Polygon) revertPolygon (BitmapImage img, Polygon p)
        {
            Polygon newP = new Polygon(2, p.c);
            Bitmap bImg = Image.BitmapImage2Bitmap(img);
            for(int i = 0; i < 5; i++)
            {
                newP.addLine(p.getShape()[i]);
            }
            foreach(Line l in p.getShape())
            {
                for(int i = 0; i < l.getLineCords().Length; i++)
                {
                    bImg.SetPixel((int)l.lineCords[i][0], (int)l.lineCords[i][1], Color.FromArgb(255, 255, 255));
                }
            }
            return (Image.ToBitmapImage(bImg),newP);
        }
        
        public static BitmapImage dFilled(BitmapImage img, Polygon p)
        {
            Bitmap bImg = Image.BitmapImage2Bitmap(img);
            for(int i = 0; i < p.getShape().Count; i++)
            {
                for(int j = 0; j < p.getShape()[i].lineCords.Length; j++)
                {
                    bImg.SetPixel((int)p.getShape()[i].lineCords[j][0], (int)p.getShape()[i].lineCords[j][1], p.getShape()[i].lineCordsColors[j][0]);
                }
            }
            return Image.ToBitmapImage(bImg);
        }

        public static Polygon generateColors(Polygon p, BitmapImage img)
        {
            Bitmap bImg = Image.BitmapImage2Bitmap(img);

            foreach (Line l in p.getShape())
            {
                l.lineCordsColors = new Color[l.lineCords.Length][];

                for (int i = 0; i < l.lineCords.Length; i++)
                {
                    l.lineCordsColors[i] = new Color[1];
                    int x = pixelReCalibrate((int)l.lineCords[i][0], bImg, 1);
                    int y = pixelReCalibrate((int)l.lineCords[i][1], bImg, 0);
                    l.lineCordsColors[i][0] = bImg.GetPixel(x, y);
                    //b.SetPixel((int)l.lineCords[i][0], (int)l.lineCords[i][1], imgF.GetPixel(x, y));
                    //img = DDAForFillingImages(img, initialPoints[i][0], initialPoints[i][1], finalPoints[i][0], finalPoints[i][1], imgF);                 
                }
            }
            return p;
        }

    }
}
