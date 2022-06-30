using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Project4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BitmapImage img;
        int drawingType = 0; //by default it draws lines i.e 0, circle = 1, brush = 2
        float[] pointsI = new float[2];
        float[] pointsF = new float[2];
        float[] newFinalForEdge = new float[2];
        int pointsCounter = 0;
        float offsetY = 177;
        int xORy = -1;
        int makePolygon = 0;
        int clippingMode = 0;
        int relocateRectangle = -1;
        int filledPolygon = 0;
        int resizePolygonMode = 0;
        float[] cordsForResizingPolygon;
        List<ImageProperties> shapes = new List<ImageProperties>(); //new ImageProperties[50];
        Polygon p;
        Rectangle currentlyEditingRectangle;
        Rectangle clippingRectangle;
        BitmapImage imageToFillShapeWith;
       // int shapesCounter = 0;
        int relocatePolygon = -1;
        float[] pointsIForNewPolygon = new float[2];
        float[] pointsIForNewRectangle = new float[2];
        System.Drawing.Color c = System.Drawing.Color.FromArgb(0, 0, 0);
        public MainWindow()
        {
            InitializeComponent();
            foreach (PropertyInfo prop in typeof(Colors).GetProperties())
            {
                colorSelection.Items.Add(prop.Name);
            }
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow.WindowState = WindowState.Maximized;
            img = Image.initializeImage(1920, 1080);
            // imageToStore = Image.initializeImage(1920, 1080);
            image.Source = img;
        }

        private void colorPixel(object sender, MouseButtonEventArgs e)
        {            
            var point = PointToScreen(e.GetPosition(Application.Current.MainWindow));                      
                pointsCounter++;
            if (relocatePolygon != -1)
            {
                pointsCounter--;
                relocatePolygon = -1;
                pointsF[0] = (int)point.X - pointsIForNewPolygon[0];
                pointsF[1] = (int)((point.Y - offsetY) - pointsIForNewPolygon[1]);
             
                   Polygon pNew = new Polygon(2, p.c);
               
                //if (p.filled == 1) Filling.setColorPointsFromImage(p.polygonImg, p);
                var items = Drawing.drawPolygon(img, p.getShape(), pointsF, c,p.filled);               
                if(clippingMode != 1)
                    img = items.Item1;                
                   // image.Source = img;
                    pNew = items.Item2;
                if (p.filled == 1)
                    pNew.filled = 1;
                // Console.WriteLine("pNew initial and final cords let's see " + pNew.getShape()[0].getInitialPoints()[0]);
                Console.WriteLine("New points to be added (" + pointsF[0] + "," + pointsF[1] + ")");
                Console.WriteLine($"({pNew.getShape()[0].getInitialPoints()[0]},{pNew.getShape()[0].getInitialPoints()[1]})");

                shapes.Add(pNew);
                //image.Source = img;
                if (clippingMode == 1)
                {
                    //img = Drawing.clearPolygon(img, pNew);
                    img = Drawing.drawRectangle(img, clippingRectangle.getInitialPoints(), clippingRectangle.getFinalPoints(), System.Drawing.Color.FromArgb(0, 0, 0), clippingRectangle);
                    List<ImageProperties> shapesToBeAdded = new List<ImageProperties>();
                   // img = Drawing.clearAllPolygons(img, shapes);
                    System.Drawing.Color clippingColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    if (pNew.getShape().Count >= 10) clippingColor = pNew.c;
                    Polygon pp = new Polygon(2, clippingColor);
                    foreach (Line l in pNew.getShape())
                    {
                        var items2 = Drawing.clipLine(img, clippingRectangle, l.getInitialPoints(), l.getFinalPoints(), clippingColor, pNew.filled,l.lineCordsColors);
                        //img = items2.Item1;
                        pp.addLine(items2.Item2);
                        //shapesToBeAdded.Add(pp);
                       // Console.WriteLine($"({pp.getShape()[pp.getShape().IndexOf(items2.Item2)].getInitialPoints()[0]},{pp.getShape()[pp.getShape().IndexOf(items2.Item2)].getInitialPoints()[1]})");

                    }
                    if (pNew.filled == 1) pp.filled = 1;
                    float[] pff = { 0, 0 };
                    Console.WriteLine($"after loop ({pp.getShape()[0].getInitialPoints()[0]},{pp.getShape()[0].getInitialPoints()[1]})");

                    var ete = Drawing.drawPolygon(img, pp.getShape(), pff, clippingColor, pp.filled);
                    // float[] pef = { 0, 0 };
                    img = ete.Item1;
                    /*foreach (ImageProperties shape in shapesToBeAdded)*/
                    //shapes.Remove(pNew);
                    //shapes.Add(pp);
                                     
                        Console.WriteLine($"Shapes count inside color pixel {pp.getShape().Count}");
                    // shapesCounter++;
                }
                image.Source = img;
            }
            else if (relocateRectangle != -1)
            {
                pointsCounter--;
                float[] pointyF = new float[2];
                pointyF[0] = (int)point.X - pointsIForNewRectangle[0];
                pointyF[1] = (int)(point.Y - offsetY) - pointsIForNewRectangle[1];
                var items = Drawing.moveRectangle(img, currentlyEditingRectangle, pointyF, c);
                currentlyEditingRectangle = items.Item2;
                img = Drawing.drawRectangle(img, currentlyEditingRectangle.getInitialPoints(), currentlyEditingRectangle.getFinalPoints(), System.Drawing.Color.FromArgb(0,0,0), currentlyEditingRectangle);
                shapes.Add(currentlyEditingRectangle);
                relocateRectangle = -1;
                image.Source = img;
            }
            else
            {
                if (pointsCounter % 2 == 0)
                {
                    pointsF = Drawing.getPoints((int)point.X, (int)(point.Y - offsetY));
                    if (drawingType == 0)
                    {
                        float dx = pointsF[0] - pointsI[0];
                        float dy = pointsF[1] - pointsI[1];

                        int steps = (int)(Math.Abs(dx) > Math.Abs(dy) ? Math.Abs(dx) : Math.Abs(dy));
                        img = Drawing.DDA(img, pointsI[0], pointsI[1], pointsF[0], pointsF[1], c);

                        Line l = new Line(0, pointsI[0], pointsI[1], pointsF[0], pointsF[1], c, Drawing.getLinePoints(pointsI[0], pointsI[1], pointsF[0], pointsF[1]), steps + 1);
                        if (makePolygon == 1) p.addLine(l);
                        else
                        {
                            // shapes[shapesCounter] = l;
                            shapes.Add(l);
                            //shapesCounter++;
                        }
                        //MessageBox.Show($"({pointsI[0]},{pointsI[1]})");
                    }
                    else if (drawingType == 1)
                    {
                        if (xORy == 1)
                        {
                            pointsF[0] = newFinalForEdge[0];
                            xORy = -1;
                        }
                        else if (xORy == 0)
                        {
                            pointsF[1] = newFinalForEdge[1];
                            xORy = -1;
                        }
                        Rectangle rectangle = new Rectangle(pointsI, pointsF, 1, c);
                        img = Drawing.drawRectangle(img, pointsI, pointsF, System.Drawing.Color.FromArgb(0,0,0), rectangle);
                      //  currentlyEditingRectangle = rectangle;
                        
                        // shapes[shapesCounter] = rectangle;
                        //shapesCounter++;
                        shapes.Add(rectangle);
                    }
                    // pointsCounter = 0;
                    Console.WriteLine($"({pointsI[0]},{pointsI[1]} opposite is ({pointsF[0]},{pointsF[1]})");
                    image.Source = img;
                }
                else
                    pointsI = Drawing.getPoints((int)point.X, (int)(point.Y - offsetY));
            }
        }



        private void resizeObject(object sender, MouseButtonEventArgs e)
        {
            if(resizePolygonMode != 0)
            {
                var point = PointToScreen(e.GetPosition(Application.Current.MainWindow));
                //float[] cords = new float[2];
                cordsForResizingPolygon = new float[2];
                (cordsForResizingPolygon[0], cordsForResizingPolygon[1]) = ((float)point.X, (float)(point.Y - offsetY));
                Polygon pToResize = Drawing.getPolygon(img, shapes, cordsForResizingPolygon);
                if (pToResize != null)
                {
                    MessageBox.Show("Got rectangle");
                    Line ToEdit = pToResize.getShape()[Drawing.changeVertex(img, pToResize, cordsForResizingPolygon)];
                    img = Drawing.clearLine(img, ToEdit);
                    image.Source = img;
                    makePolygon = 1;
                    shapes.Remove(pToResize);
                    pToResize.getShape().Remove(ToEdit);
                    resizePolygonMode = 0;
                    p = pToResize;

                }
               

            }           
           else if (makePolygon == 1)
            {
                makePolygon = 0;
                //shapes[shapesCounter] = p;
                //shapesCounter++;
               
                //MessageBox.Show("saved polygon" + $" polygon has {p.getShape().Count}");
                if (p.filled == 1)
                    loadImg(sender, e);
                else if (p.getShape().Count > 10)
                    filling(sender, e);               
                else shapes.Add(p);

                //  foreach (Line l in p.getShape()) Console.WriteLine($"Line initial points: ({l.getInitialPoints()[0]},{l.getInitialPoints()[1]})");
            }
            else
            {
                var point = PointToScreen(e.GetPosition(Application.Current.MainWindow));
                float[] cords = new float[2];
                (cords[0], cords[1]) = ((float)point.X, (float)(point.Y - offsetY));
             /*   foreach(ImageProperties shape in shapes)
                {
                    if (shape.classNo == 1)
                        MessageBox.Show($"({shape.getTopLeft()[0]},{shape.getTopLeft()[1]}),({shape.getTopRight()[0]},{shape.getTopRight()[1]}), ({shape.getBottomLeft()[0]},{shape.getBottomLeft()[1]}, ({shape.getBottomRight()[0]},{shape.getBottomRight()[1]})");
                }*/
                if (drawingType == 1)
                {
                    Rectangle r = Drawing.getRectangle(shapes, cords);
                    if (r != null)
                    {
                       // MessageBox.Show("found one");
                        //img = Drawing.clearRectangle(img, r);
                        // image.Source = img;
                        int resizeType = Drawing.typeOfResize(r, cords);
                        if (resizeType < 1 && resizeType > -4)
                        {
                            pointsCounter++;
                            var items = Drawing.changeEdge(img, r, cords, resizeType);
                            img = items.Item1;
                            image.Source = img;
                            pointsI = items.Item2;
                            newFinalForEdge = items.Item3;
                            xORy = items.Item4;                            
                            // if (shapesCounter > 1)
                            //                                shapes = Drawing.resizeArray(shapes, shapesCounter, Array.IndexOf(shapes, r));
                            shapes.Remove(r);
                            // shapesCounter--;
                        }
                        else if (resizeType > 1 && resizeType < 6)
                        {
                            pointsCounter++;
                            var items = Drawing.changeVertex(img, cords, r, resizeType);
                            img = items.Item1;
                            image.Source = img;
                            pointsI = items.Item2;                            
                            /* if (shapesCounter > 1)
                                 shapes = Drawing.resizeArray(shapes, shapesCounter, Array.IndexOf(shapes, r));
                             shapesCounter--;*/
                            shapes.Remove(r);
                        }
                        else if (resizeType == 1)
                        {
                            pointsIForNewRectangle[0] = cords[0];
                            pointsIForNewRectangle[1] = cords[1];                            
                            img = Drawing.clearRectangle(img, r);
                            image.Source = img;
                            relocateRectangle = 0;
                            currentlyEditingRectangle = r;
                            shapes.Remove(r);
                        }

                    }
                }
                else if(drawingType == 0)
                {
                    Polygon poly = Drawing.getPolygon(img, shapes, cords);                    
                    if (poly != null)
                    {
                        float[] newPointF = { 0, 0 };

                        img = Drawing.clearPolygon(img, poly); //Drawing.clearAllPolygons(img, shapes); //Drawing.movePolygon(img, poly);
                       // var it = Drawing.drawPolygon(img, poly.getShape(),newPointF , System.Drawing.Color.FromArgb(255, 255, 255));
                       // img = it.Item1;
                       if(clippingRectangle != null)
                        img = Drawing.drawRectangle(img, clippingRectangle.getInitialPoints(), clippingRectangle.getFinalPoints(), System.Drawing.Color.FromArgb(0, 0, 0), clippingRectangle);

                        image.Source = img;
                        relocatePolygon = 0;
                        // if (poly.filled == 1)  //Console.WriteLine("yes sir");
                        /* if (poly.filled == 1) { 
                             filledPolygon = 1;
                             Console.WriteLine("we are here in filled");
                         }*/
                        /* if (shapesCounter > 1)
                             shapes = Drawing.resizeArray(shapes, shapesCounter, Array.IndexOf(shapes, poly));
                         shapesCounter--;*/
                       
                        shapes.Remove(poly);

                       // if (poly.filled == 1) p = Filling.generateFilledPolygon(poly);
                        p = poly;
                       

                     /*   foreach (Line l in p.getShape())
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                Console.WriteLine("inside resize function p after saving found polygon " + l.lineCordsColors[i][0]);
                            }
                        }*/

                        //p.filled = 1;
                        pointsIForNewPolygon = cords;
                    }
                    else MessageBox.Show("polygon not found");
                }
            }
        }

        private void line(Object sender, RoutedEventArgs e)
        {
            drawingType = 0;
            pointsCounter = 0;
        }

        private void rectangle(Object sender, RoutedEventArgs e)
        {
            drawingType = 1;
            pointsCounter = 0;
        }
        private void polygon(Object sender, RoutedEventArgs e)
        {
            if (makePolygon == 0)
            {
               // resizePolygonMode = 0;
                makePolygon = 1;
                p = new Polygon(2, c);
            }           
        }

        private void showCurrentColor(Object sender, RoutedEventArgs e)
        {
            //var values = typeof(Brushes).GetProperties().Select(b => new { Name = b.Name, Brush = b.GetValue(null) as Brush }).ToArray();

            string value = colorSelection.SelectedItem.ToString();
            PropertyInfo prop = typeof(Colors).GetProperty(value);
            c = System.Drawing.Color.FromName(prop.Name);
            //System.Windows.Media.SolidColorBrush brush  = (System.Windows.Media.SolidColorBrush)prop.GetValue(null, null); 
        }
        private void clipping(Object sender, RoutedEventArgs e)
        {
            // List<ImageProperties> shapesToList = shapes.ToList();
            // int indexCount = 0;
            if (clippingMode == 0)
            {
                clippingMode = 1;
                

                //image.Source = img;
                List<ImageProperties> shapesToBeRemoved = new List<ImageProperties>();
                List<ImageProperties> shapesToBeAdded = new List<ImageProperties>();
                clippingRectangle = Drawing.findRec(shapes);
                if (clippingRectangle != null)
                {
                    foreach (ImageProperties shape in shapes)
                    {

                        if (shape.classNo == 2)
                        {
                            img = Drawing.clearPolygon(img,(Polygon)shape);
                            img = Drawing.drawRectangle(img, clippingRectangle.getInitialPoints(), clippingRectangle.getFinalPoints(), System.Drawing.Color.FromArgb(0, 0, 0), clippingRectangle);
                           
                            System.Drawing.Color clippingColor = System.Drawing.Color.FromArgb(255, 0, 0);
                            if (shape.getShape().Count >= 10) clippingColor = shape.c; 
                            p = new Polygon(2, clippingColor);
                            Console.WriteLine($"({shape.getShape()[0].getInitialPoints()[0]},{shape.getShape()[0].getInitialPoints()[1]})");

                            //Polygon ToBeClipped = (Polygon)shape;
                            //Console.WriteLine("THis polygon is made up of " + ToBeClipped.filled);
                            foreach (Line l in shape.getShape())
                            {
                                Console.WriteLine("Let's see if it is really filled or not 5" + shape.isFilled());
                                var items = Drawing.clipLine(img, clippingRectangle, l.getInitialPoints(), l.getFinalPoints(), clippingColor,shape.isFilled(),l.lineCordsColors);
                               // img = items.Item1;                                
                                p.addLine(items.Item2);                                
                                shapesToBeRemoved.Add(shape);
                            }
                            if (shape.isFilled() == 1) p.filled = 1;
                            float[] pef = { 0, 0 };
                            var ete = Drawing.drawPolygon(img, p.getShape(), pef, clippingColor, p.filled);
                            // float[] pef = { 0, 0 };
                            img = ete.Item1;                                //var etems = Drawing.drawPolygon(img, p.getShape(), pef, clippingColor, 0);
                             //   img = etems.Item1;
                               image.Source = img;
                            shapesToBeAdded.Add(p);                            
                        }                       
                    }
                    Console.WriteLine($"({p.getShape()[0].getInitialPoints()[0]},{p.getShape()[0].getInitialPoints()[1]})");
                    //foreach(ImageProperties shapeToBeA in shapesToBeAdded)
                    //shapes.Add(shapeToBeA);
                    Console.WriteLine("After clipping shapes count: " + shapes.Count);
                   // foreach (ImageProperties shapeB in shapesToBeRemoved)
                    //    shapes.Remove(shapeB);
                    // foreach (ImageProperties shape in shapesToBeRemoved) shapes.Remove(shape);
                    //foreach (ImageProperties shape in shapesToBeAdded) shapes.Add(shape);
                    //foreach (ImageProperties shape in shapes) Console.WriteLine(shape.classNo);
                    //image.Source = img;
                }
            }
            else clippingMode = 0;
        }
        private void filling(Object sender, RoutedEventArgs e)
        {
            float[] nf = { 0, 0 };
            shapes.Remove(p);
            var it = Drawing.drawPolygon(img, p.getShape(), nf, System.Drawing.Color.FromArgb(255, 255, 255));
            img = it.Item1;
            p = it.Item2;

            float maxY = Filling.getMaxYOfAll(p.getShape());
            float minY = Filling.getMinYOfAll(p.getShape());
            List<float[]>[] GET = new List<float[]>[(int)maxY + 1];
            GET = Filling.createGlobalEdgeTable(p);
            Console.WriteLine(minY);
            /*foreach (List<int[]> p in GET)
            {
                if (p[0][0] != -1 && p[1][0] != -1)
                    Console.WriteLine($"[{p[0][0]}|{p[0][1]}|{p[0][2]}]->[{p[1][0]}|{p[1][1]}|{p[1][2]}]");
                else if (p[1][0] == -1 && p[0][1] != -1)
                    Console.WriteLine($"[{p[0][0]}|{p[0][1]}|{p[0][2]}]");
                else
                    Console.WriteLine("[null]");
            }*/
            
            var items = Filling.createActiveEdgeTable(GET, minY, maxY);
            List<float[]> initialPoints = items.Item1;
            List<float[]> finalPoints = items.Item2;
            Polygon filledPolygon = new Polygon(2, c);
            for (int i = 0; i < finalPoints.Count; i++)
            {
                //Console.WriteLine($"({initialPoints[i][0]},{initialPoints[i][1]}) , ({finalPoints[i][0]},{finalPoints[i][1]})");
                float dx = finalPoints[i][0] - initialPoints[i][0];
                float dy = finalPoints[i][1] - initialPoints[i][1];

                int steps = (int)(Math.Abs(dx) > Math.Abs(dy) ? Math.Abs(dx) : Math.Abs(dy));
                Line filledLine = new Line(0, initialPoints[i][0], initialPoints[i][1], finalPoints[i][0], finalPoints[i][1], c, Drawing.getLinePoints(initialPoints[i][0], initialPoints[i][1], finalPoints[i][0], finalPoints[i][1]), steps + 1);
                filledPolygon.addLine(filledLine);
            }
               
               img = Filling.fillPolygon(img, filledPolygon,c);
               image.Source = img;
                shapes.Remove(p);
               shapes.Add(filledPolygon);
        }

       
        

        private void loadImg(Object sender,RoutedEventArgs e)
        {
            string imageName = "";
            if(imageToFillShapeWith == null)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.* ";
                if (dlg.ShowDialog() == true)
                {
                    Stream stream = File.Open(dlg.FileName, FileMode.Open);
                    BitmapImage imgsrc = new BitmapImage();
                    imgsrc.BeginInit();
                    imgsrc.StreamSource = stream;
                    imgsrc.EndInit();
                    //image.Source = imgsrc;
                    imageToFillShapeWith = imgsrc;
                    imageName = dlg.FileName;
                    //image.Source = imageToFillShapeWith;
                    Console.WriteLine(dlg.FileName);
                }
            }
            shapes.Remove(p);
            float[] nf = { 0, 0 };
            var it = Drawing.drawPolygon(img, p.getShape(), nf, System.Drawing.Color.FromArgb(255, 255, 255));
            img = it.Item1;
            p = it.Item2;
            //if(imageName != "")
            p.ImageName = imageName;
            float maxY = Filling.getMaxYOfAll(p.getShape());
            float minY = Filling.getMinYOfAll(p.getShape());
            List<float[]>[] GET = new List<float[]>[(int)maxY + 1];
            GET = Filling.createGlobalEdgeTable(p);
            Console.WriteLine(minY);
            /*foreach (List<int[]> p in GET)
            {
                if (p[0][0] != -1 && p[1][0] != -1)
                    Console.WriteLine($"[{p[0][0]}|{p[0][1]}|{p[0][2]}]->[{p[1][0]}|{p[1][1]}|{p[1][2]}]");
                else if (p[1][0] == -1 && p[0][1] != -1)
                    Console.WriteLine($"[{p[0][0]}|{p[0][1]}|{p[0][2]}]");
                else
                    Console.WriteLine("[null]");
            }*/
            Polygon filledPolygon = new Polygon(2, c,1,imageToFillShapeWith);
            var items = Filling.createActiveEdgeTable(GET, minY, maxY);
            List<float[]> initialPoints = items.Item1;
            List<float[]> finalPoints = items.Item2;
            for (int i = 0; i < finalPoints.Count; i++)
            {
                // Console.WriteLine($"({initialPoints[i][0]},{initialPoints[i][1]}) , ({finalPoints[i][0]},{finalPoints[i][1]})");
                float dx = finalPoints[i][0] - initialPoints[i][0];
                float dy = finalPoints[i][1] - initialPoints[i][1];

                int steps = (int)(Math.Abs(dx) > Math.Abs(dy) ? Math.Abs(dx) : Math.Abs(dy));
                Line filledLine = new Line(0, initialPoints[i][0], initialPoints[i][1], finalPoints[i][0], finalPoints[i][1], c, Drawing.getLinePoints(initialPoints[i][0], initialPoints[i][1], finalPoints[i][0], finalPoints[i][1]), steps + 1);
                filledPolygon.addLine(filledLine);
            }
            var it2 = Filling.fillPolygonWithImg(img,filledPolygon,imageToFillShapeWith);
            if(imageName != "")
            filledPolygon.ImageName = imageName;
            img = it2.Item1;
            image.Source = img;
            shapes.Remove(p);
            shapes.Add(filledPolygon);
            foreach(Line l in filledPolygon.getShape())
            {
                for(int i = 0; i < 1; i++)
                {
                    Console.WriteLine("inside imgfill function " + l.lineCordsColors[i][0]);
                }
            }
           
            //foreach(Line l in filledPolygon.getShape()) Console.WriteLine(l.getColorOfPoints()[0],l.get)
        }

        private void clear(Object sender, RoutedEventArgs e)
        {
            img = Filling.clearImage(img,shapes);
            List<ImageProperties> shapesToBeDeleted = new List<ImageProperties>();
            foreach(ImageProperties shape in shapes)
            {
                shapesToBeDeleted.Add(shape);
            }
            foreach (ImageProperties shape in shapesToBeDeleted)
                shapes.Remove(shape);
            image.Source = img;
            imageToFillShapeWith = null;
        }

        private void resizePolygon(Object sender, RoutedEventArgs e)
        {
            if (resizePolygonMode == 0) resizePolygonMode = 1;
          //  else resizePolygonMode = 0;
            
        }

        private void save(Object sender, RoutedEventArgs e)
        {
            int i = 0;
            string dirParameter = AppDomain.CurrentDomain.BaseDirectory + $@"\file{i}.txt";
            while (File.Exists(dirParameter))
            {
                Random r = new Random();
                i = r.Next(0, 100);
                dirParameter = AppDomain.CurrentDomain.BaseDirectory + $@"\file{i}.txt";
            }
            FileStream fs = new FileStream(dirParameter, FileMode.Create, FileAccess.Write);
            StreamWriter m_WriterParameter = new StreamWriter(fs);
            for (int j = 0; j < shapes.Count; j++)
            {
                if (shapes[j].classNo == 1)
                {
                    m_WriterParameter.WriteLine("Class no: " + shapes[j].classNo);
                   
                    m_WriterParameter.WriteLine($"initial: ({shapes[j].getInitialPoints()[0]},{shapes[j].getInitialPoints()[1]}) , final: ({shapes[j].getFinalPoints()[0]},{shapes[j].getFinalPoints()[1]}))");                    
                }
                else if (shapes[j].classNo == 2)
                {
                    m_WriterParameter.WriteLine("Class no: " + shapes[j].classNo);
                    m_WriterParameter.WriteLine(shapes[j].isFilled());
                    if(shapes[j].getImage() != null)
                    m_WriterParameter.WriteLine(shapes[j].getImageName());
                    foreach (Line l in shapes[j].getShape())
                    {
                        //m_WriterParameter.WriteLine("initial and final points");
                        m_WriterParameter.WriteLine($"intial: ({l.pointsI[0]},{l.pointsI[1]}) , final: ({l.pointsF[0]},{l.pointsF[1]})");
                        m_WriterParameter.WriteLine("Size: " + l.getLineCords().Length);
                        
                        //m_WriterParameter.WriteLine("Color cords");
                       /* for (int m = 0; m < l.getLineCordsColrs().Length; m++)
                            m_WriterParameter.WriteLine($"ColorCords: {l.getLineCordsColrs()[m][0]}");*/
                    }

                   
                }
              
            }
            m_WriterParameter.Flush();
            m_WriterParameter.Close();
        }

        public static int extractColor(string s)
        {
            int startingIndex = 0;
            int counter = 0;
            int pointsExtraced = 0; // = new int[3];
            for (int i = 0; i < s.Length; i++)
            {
                if (startingIndex != 0) counter++;
                if (s[i] == '=') startingIndex = i + 1;
                if (s[i] == ',')
                {
                    pointsExtraced = int.Parse(s.Substring(startingIndex, counter - 1));
                    //pointsExtraced[1] = 
                    startingIndex = i + 1;
                    counter = 0;
                    break;
                }

            }
            return pointsExtraced;
        }

        private float[] getValue(string s)
        {
            int startingIndex = 0;
            int counter = 0;
            
            float[] pointsExtraced = new float[2];
            for(int i = 0; i < s.Length; i++)
            {
                if (startingIndex != 0) counter++;
                if (s[i] == '(') startingIndex = i + 1;
                if (s[i] == ',')
                {
                    pointsExtraced[0] = float.Parse(s.Substring(startingIndex, counter - 1));
                    //pointsExtraced[1] = 
                    startingIndex = i + 1;
                    counter = 0;
                }
                if (s[i] == ')')
                {
                    pointsExtraced[1] = float.Parse(s.Substring(startingIndex, counter - 1));
                    //pointsExtraced[1] = 
                    startingIndex = i + 1;
                    counter = 0;
                    break;
                }
            }
            return pointsExtraced;
        }

        private void load(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                using (StreamReader sr = File.OpenText(openFileDialog.FileName))
                {

                    string s = "";
                    int i = 0;
                    int classNo = -1;
                    string[] shapeProperties = new string[7];
                    List<Line> lines = new List<Line>();
                    float[] poinI = new float[2];
                    float[] poinF = new float[2];
                    float[] poinIP = new float[2];
                    float[] poinFP = new float[2];
                    int filled = 0;
                    float[][] lineCordons;
                    System.Drawing.Color[][] colorData = null;
                    int lineCounter = 0;
                    Rectangle rero = null;
                    List<Rectangle> rectangles = new List<Rectangle>();
                    List<Polygon> polygons = new List<Polygon>();
                    Polygon pToBeLoaded = new Polygon(2, c);
                    int size = 4;
                    int indy = 0;
                    Line lino = null;
                    //string[] shapeProperties = new string[7];
                    //int dataCounter = 0;
                    while ((s = sr.ReadLine()) != null)
                    {

                        if (s.StartsWith("Class no:"))
                        {                            
                             if (classNo == 2)
                            {                               
                                polygons.Add(pToBeLoaded);
                            }
                            classNo = int.Parse(s.Substring(10, 1));
                            pToBeLoaded = new Polygon(2, c);
                            //indy = 0;
                        }
                        if (s.StartsWith("initial:") && classNo == 1)
                        {
                            string[] subs = s.Split(' ');
                            poinIP = getValue(subs[1]);
                            poinFP = getValue(subs[4]);
                            rero = new Rectangle(poinIP, poinFP, 1, c);
                            rectangles.Add(rero);
                        }
                        if (classNo == 2 && (s.StartsWith("0") || s.StartsWith("1")))
                        {
                            filled = int.Parse(s);
                            pToBeLoaded.filled = filled;                            
                        }
                        if (classNo == 2 && s.StartsWith("C:"))
                        {
                            pToBeLoaded.ImageName = s;
                            //pToBeLoaded.polygonImg = new BitmapImage(new Uri(pToBeLoaded.ImageName));
                        }
                        if (classNo == 2 && s.StartsWith("Size: "))
                        {
                            size = int.Parse(s.Substring(6,s.Length - 6));
                           // colorData = new System.Drawing.Color[size][];
                        }
                        if (classNo == 2 && s.StartsWith("intial: "))
                        {
                           /* if (indy > 0)
                            {
                                lino.lineCordsColors = colorData;
                                pToBeLoaded.addLine(lino);
                            }
                            indy = 0;
                            */
                            string[] subs = s.Split(' ');
                            poinI = getValue(subs[1]);
                            poinF = getValue(subs[4]);
                             lino = new Line(0, poinI[0], poinI[1], poinF[0], poinF[1], c, Drawing.getLinePoints(poinI[0], poinI[1], poinF[0], poinF[1]), size);
                            pToBeLoaded.addLine(lino);
                        }
                       /* if(classNo == 2 && s.StartsWith("ColorCords: "))
                        {
                            string[] subs = s.Split(' ');
                            int R = extractColor(subs[3]);
                            int G = extractColor(subs[4]);
                            int B = extractColor(subs[5]);

                           // colorData[indy] = new System.Drawing.Color[1];
                            //colorData[indy][0] = System.Drawing.Color.FromArgb(R, G, B);
                           // indy++;
                        }*/
                        /*if(classNo == 2 && s.StartsWith("ColorCords: "))
                        {

                        }*/

                    }
                    /* foreach (Line lelo in pToBeLoaded.getShape())
                     {
                         Console.WriteLine($"({lelo.pointsI[0]},{lelo.pointsI[1]},({lelo.pointsF[0]},{lelo.pointsF[1]})");
                     }                    
                     pToBeLoaded.filled = filled;                    */
                    float[] debo = { 0, 0 };
                    foreach (Rectangle recty in rectangles)
                    {
                        img = Drawing.drawRectangle(img, recty.getInitialPoints(), recty.getFinalPoints(), c, recty);
                        image.Source = img;
                    }
                    if(polygons.Count > 0)
                    foreach(Polygon pepo in polygons)
                    {
                        if (pepo.filled == 1)
                        {
                            pepo.polygonImg = new BitmapImage(new Uri(pepo.ImageName));
                            Polygon pNewOne = Filling.generateColors(pepo, pepo.polygonImg);
                            img = Filling.dFilled(img, pNewOne);
                        }
                        else
                        {
                            var itomy = Drawing.drawPolygon(img, pepo.getShape(), debo, c, pepo.filled);
                            img = itomy.Item1;
                        }
                        image.Source = img;
                    }
                  
                    
                   
                   

                  /*  if (pToBeLoaded.filled == 1)
                    {
                        pToBeLoaded.polygonImg = new BitmapImage(new Uri(pToBeLoaded.ImageName));
                        pToBeLoaded = Filling.generateColors(pToBeLoaded, pToBeLoaded.polygonImg);
                        img = Filling.dFilled(img, pToBeLoaded);
                        image.Source = img;
                      /* for(int l = 0; l < pToBeLoaded.linesOfPolygon.Count; l++)
                        {
                           for(int l2 = 0; l2 < pToBeLoaded.linesOfPolygon[l].lineCordsColors.Length; i++)
                            {
                                Console.WriteLine(pToBeLoaded.linesOfPolygon[l].lineCordsColors[l2][0]);
                            }
                            Console.WriteLine("########");
                        }
                    }*/
                   /* else
                    {
                        var itomy = Drawing.drawPolygon(img, pToBeLoaded.getShape(), debo, c, pToBeLoaded.filled);
                        img = itomy.Item1;
                        image.Source = img;
                    }*/
                }
        }

        private void Button_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            imageToFillShapeWith = null;
        }
    }
}
