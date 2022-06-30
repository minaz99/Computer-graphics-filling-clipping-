using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace Project4
{
    public abstract class ImageProperties
    {

        // public BitmapImage img = new BitmapImage();

        public int classNo;
        public Color c;
        public ImageProperties(int classId, Color color)
        {
            c = color;
            classNo = classId;
        }

        public abstract float[] getInitialPoints();
        public abstract float[] getFinalPoints();
        public abstract List<Line> getShape();
        public abstract void addLine(Line line);
        public abstract float[] getTopRight();
        public abstract float[] getTopLeft();

        public abstract float[] getBottomRight();

        public abstract float[] getBottomLeft();
        public abstract float[][] getLineCords();
        public abstract int isFilled();
        public abstract Color[][] getLineCordsColrs();
        public abstract BitmapImage getImage();
        public abstract string getImageName();

    }

    public class Rectangle : ImageProperties
    {
        float[] topRight;
        float[] topLeft;
        float[] bottomRight;
        float[] bottomLeft;
        float[] pointI;
        float[] pointF;
        List<Line> lines;
        public Rectangle(float[] pointI, float[] pointF, int classId, Color c) : base(classId, c)
        {
            this.pointI = pointI;
            this.pointF = pointF;
            lines = new List<Line>();
            (topRight, topLeft, bottomRight, bottomLeft) = (new float[2], new float[2], new float[2], new float[2]);
            setBoundariesPoints();
            Console.WriteLine($"({topLeft[0]},{topLeft[1]})");
            Console.WriteLine($"({topRight[0]},{topRight[1]})");
            Console.WriteLine($"({bottomLeft[0]},{bottomLeft[1]})");
            Console.WriteLine($"({bottomRight[0]},{bottomRight[1]})");
        }
        public override void addLine(Line line)
        {
            lines.Add(line);
        }

        private void setBoundariesPoints()
        {
            if (pointI[0] < pointF[0] && pointI[1] < pointF[1])
            {
                (topLeft[0], topLeft[1]) = (pointI[0], pointI[1]);
                (topRight[0], topRight[1]) = (pointF[0], pointI[1]);
                (bottomLeft[0], bottomLeft[1]) = (pointI[0], pointF[1]);
                (bottomRight[0], bottomRight[1]) = (pointF[0], pointF[1]);
            } else if (pointF[0] < pointI[0] && pointI[1] < pointF[1])
            {
                (topLeft[0], topLeft[1]) = (pointF[0], pointI[1]);
                (topRight[0], topRight[1]) = (pointI[0], pointI[1]);
                (bottomLeft[0], bottomLeft[1]) = (pointF[0], pointF[1]);
                (bottomRight[0], bottomRight[1]) = (pointI[0], pointF[1]);
            } else if (pointI[1] > pointF[1] && pointI[0] < pointF[1])
            {
                (topLeft[0], topLeft[1]) = (pointI[0], pointF[1]);
                (topRight[0], topRight[1]) = (pointF[0], pointF[1]);
                (bottomLeft[0], bottomLeft[1]) = (pointI[0], pointI[1]);
                (bottomRight[0], bottomRight[1]) = (pointF[0], pointI[1]);
            }
            else
            {

                (topLeft[0], topLeft[1]) = (pointF[0], pointF[1]);
                (topRight[0], topRight[1]) = (pointI[0], pointF[1]);
                (bottomLeft[0], bottomLeft[1]) = (pointF[0], pointI[1]);
                (bottomRight[0], bottomRight[1]) = (pointI[0], pointI[1]);
            }
        }

        public override float[] getFinalPoints()
        {
            return pointF;
        }

        public override float[] getInitialPoints()
        {
            return pointI;
        }

        public override List<Line> getShape()
        {
            return lines;
        }

        public override float[] getTopRight()
        {
            return topRight;
        }

        public override float[] getTopLeft()
        {
            return topLeft;
        }

        public override float[] getBottomRight()
        {
            return bottomRight;
        }

        public override float[] getBottomLeft()
        {
            return bottomLeft;
        }

        public override float[][] getLineCords()
        {
            throw new NotImplementedException();
        }

        public override int isFilled()
        {
            throw new NotImplementedException();
        }

        public override Color[][] getLineCordsColrs()
        {
            throw new NotImplementedException();
        }

        public override BitmapImage getImage()
        {
            throw new NotImplementedException();
        }

        public override string getImageName()
        {
            throw new NotImplementedException();
        }
    }

    public class Line : ImageProperties
    {
        public float[] pointsI = new float[2];
        public float[] pointsF = new float[2];
        public float[][] lineCords;
        public Color[][] lineCordsColors;
        public Line(int classNo, float xI, float yI, float xF, float yF, Color c, float[][] cords, int size) : base(classNo, c)
        {
            pointsI[0] = xI;
            pointsI[1] = yI;
            pointsF[0] = xF;
            pointsF[1] = yF;
            lineCords = new float[size][];
            lineCordsColors = new Color[size][];
            for (int i = 0; i < size; i++)
            {
                lineCords[i] = new float[2];
                lineCordsColors[i] = new Color[2];
            }
            lineCords = cords;
        }

        public void setColorsToPoints (Color[][] c)
        {
            c = lineCordsColors;
        }
        public Color[][] getColorOfPoints()
        {
            return lineCordsColors;
        }
        public override void addLine(Line line)
        {
            throw new NotImplementedException();
        }

        public override float[] getBottomLeft()
        {
            throw new NotImplementedException();
        }

        public override float[] getBottomRight()
        {
            throw new NotImplementedException();
        }

        public override float[] getFinalPoints()
        {
            return pointsF;
        }

        public override float[] getInitialPoints()
        {
            return pointsI;
        }

        public override float[][] getLineCords()
        {
            return lineCords;
        }

        public override List<Line> getShape()
        {
            throw new NotImplementedException();
        }

        public override float[] getTopLeft()
        {
            throw new NotImplementedException();
        }

        public override float[] getTopRight()
        {
            throw new NotImplementedException();
        }

        public override int isFilled()
        {
            throw new NotImplementedException();
        }

        public override Color[][] getLineCordsColrs()
        {
            return lineCordsColors;
        }

        public override BitmapImage getImage()
        {
            throw new NotImplementedException();
        }

        public override string getImageName()
        {
            throw new NotImplementedException();
        }
    }

    public class FilledPolygon : ImageProperties
    {
        List<Line> lines = new List<Line>();
       // List<Color[]> colorForPoints = new List<Color[]>();
        
        public FilledPolygon(int classNo, Color c) : base(classNo, c)
        {
            lines = new List<Line>();
        }
        public override void addLine(Line line)
        {            
            lines.Add(line);
        }

        public override float[] getBottomLeft()
        {
            throw new NotImplementedException();
        }

        public override float[] getBottomRight()
        {
            throw new NotImplementedException();
        }

        public override float[] getFinalPoints()
        {
            throw new NotImplementedException();
        }

        public override BitmapImage getImage()
        {
            throw new NotImplementedException();
        }

        public override string getImageName()
        {
            throw new NotImplementedException();
        }

        public override float[] getInitialPoints()
        {
            throw new NotImplementedException();
        }

        public override float[][] getLineCords()
        {
            throw new NotImplementedException();
        }

        public override Color[][] getLineCordsColrs()
        {
            throw new NotImplementedException();
        }

        public override List<Line> getShape()
        {
            throw new NotImplementedException();
        }

        public override float[] getTopLeft()
        {
            throw new NotImplementedException();
        }

        public override float[] getTopRight()
        {
            throw new NotImplementedException();
        }

        public override int isFilled()
        {
            throw new NotImplementedException();
        }
    }

    public class Polygon : ImageProperties
    {
       public List<Line> linesOfPolygon;
       public int filled = 0;
        public string ImageName;
        public BitmapImage polygonImg;
        
        public Polygon(int classNo, Color c):base(classNo,c)
        {
            linesOfPolygon = new List<Line>();
            
        }
        public Polygon(int classNo, Color c, int isFilled,BitmapImage img): base(classNo, c)
        {
            linesOfPolygon = new List<Line>();
            filled = isFilled;
            polygonImg = img;
        }
        public override void addLine(Line line)
        {
            linesOfPolygon.Add(line);
        }

        public override float[] getBottomLeft()
        {
            throw new NotImplementedException();
        }

        public override float[] getBottomRight()
        {
            throw new NotImplementedException();
        }

        public override float[] getFinalPoints()
        {
            throw new NotImplementedException();
        }

        public override BitmapImage getImage()
        {
            return polygonImg;
        }

        public override string getImageName()
        {
            return ImageName;          
        }

        public override float[] getInitialPoints()
        {
            throw new NotImplementedException();
        }

        public override float[][] getLineCords()
        {
            throw new NotImplementedException();
        }

        public override Color[][] getLineCordsColrs()
        {
            throw new NotImplementedException();
        }

        public override List<Line> getShape()
        {
            return linesOfPolygon;
        }

        public override float[] getTopLeft()
        {
            throw new NotImplementedException();
        }

        public override float[] getTopRight()
        {
            throw new NotImplementedException();
        }

        public override int isFilled()
        {
           return filled;
        }
    }
}
