using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

public class Edge
{
    Point A;
    Point B;
    public Color C;
    Line L;
    Vector tangent;

    public Edge(Point _A, Color _C)
    {
        A = _A;
        C = _C;
        L = new Line
        {
            Stroke = new SolidColorBrush(C),
            StrokeThickness = 2,
            X1 = A.X,
            Y1 = A.Y
        };
    }

    public void SetB(Point _B, Canvas _g)
    {
        B = new Point(_B.X,_B.Y);
        _g.Children.Remove(L);
        L.X2 = B.X;
        L.Y2 = B.Y;
        _g.Children.Add(L);

        tangent = new Vector(B.X - A.X, B.Y - A.Y);
    }

    public void DrawLine(Canvas _g)
    {
        _g.Children.Add(L);
    }

    public void RemoveLine(Canvas _g)
    {
        _g.Children.Remove(L);
    }

    public Point GetA()
    {
        return new Point(A.X, A.Y);
    }

    public Point GetB()
    {
        return new Point(B.X, B.Y);
    }

    public Vector Tangent()
    {
        return tangent;
    }
}

public class CohSutWindow
{
    public Rectangle R;
    public Point Left;      //lavy horny
    public Point Right;     //pravy dolny
    Point A;                //prvy kliknem
    Point B;                //druhy kliknem

    public CohSutWindow()
    {
        R = new Rectangle
        {
            Stroke = new SolidColorBrush(Colors.Black),
        };
    }

    public void NewRectangle(Point _A)
    {
        A = _A;
    }

    public void SetRectangle(Point _B, Canvas _g)
    {
        B = _B;
        if (A.X < B.X)
        {
            if (A.Y < B.Y)
            {
                Left = new Point(A.X, A.Y);
                Right = new Point(B.X, B.Y);
            }
            else
            {
                Left = new Point(A.X, B.Y);
                Right = new Point(B.X, A.Y);
            }
        }
        else
        {
            if (A.Y < B.Y)
            {
                Left = new Point(B.X, A.Y);
                Right = new Point(A.X, B.Y);
            }
            else
            {
                Left = new Point(B.X, B.Y);
                Right = new Point(A.X, A.Y);
            }
        }

        Canvas.SetLeft(R, Left.X);
        Canvas.SetTop(R, Left.Y);
        _g.Children.Remove(R);
        R.Width = Right.X - Left.X;
        R.Height = Right.Y - Left.Y;
        _g.Children.Add(R);
    }
}

public class CyrusBeckWindow
{
    List<Point> Vert;
    List<Vector> normals;
    List<Line> Lines;
    Point HelpPoint;
    Line HelpLine;
    

    public CyrusBeckWindow()
    {
        Vert = new List<Point>();
        normals = new List<Vector>();
        Lines = new List<Line>();
        HelpPoint = new Point();
        HelpLine = new Line
        {
            Stroke = new SolidColorBrush(Colors.Black),
            StrokeThickness = 2,
        };
    }

    public void AddVert(Point V, Canvas _g)
    {
        RemoveWindow(_g);
        Vert.Add(V);
        
        if (Vert.Count > 1)
        {
            Line L = new Line
            {
                Stroke = new SolidColorBrush(Colors.Black),
                X1 = Vert[Vert.Count - 2].X,
                X2 = Vert.Last().X,
                Y1 = Vert[Vert.Count - 2].Y,
                Y2 = Vert.Last().Y,
                StrokeThickness = 2
            };
            Canvas.SetZIndex(L, 0);
            Lines.Add(L);
        }

        DrawWindow(_g);

        HelpLine.X1 = Vert.Last().X;
        HelpLine.Y1 = Vert.Last().Y;
    }

    public void Clear()
    {
        Vert.Clear();
        normals.Clear();
        Lines.Clear();
        
    }

    public int NoEdges()
    {
        return Lines.Count;
    }

    public void CloseWindow(Canvas _g)
    {
        
        _g.Children.Remove(HelpLine);
        RemoveWindow(_g);

        if (Vert.Count > 2)
        {
            if (Vert[0] != Vert.Last())
            {
                //pridame vrchol aby sme uzavreli okno
                Vert.Add(new Point(Vert[0].X, Vert[0].Y));
                Line L = new Line
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    X1 = Vert[Vert.Count - 2].X,
                    X2 = Vert.Last().X,
                    Y1 = Vert[Vert.Count - 2].Y,
                    Y2 = Vert.Last().Y,
                    StrokeThickness = 2
                };
                Canvas.SetZIndex(L, 0);
                Lines.Add(L);
            }

            //vytvorime normaly
            for (int i = 0; i < Vert.Count - 1; i++)
            { 
                normals.Add(new Vector(Vert[i].Y - Vert[i + 1].Y, Vert[i + 1].X - Vert[i].X));
            }

            //flipneme normaly ak treba
            Vector n0 = normals[0];
            n0.Normalize();
            Vector u = Vert[2] - Midpoint(0);
            if ((n0.X*u.X + n0.Y*u.Y) < 0)
            {
                for (int i = 0; i < normals.Count; i++)
                {
                    normals[i] = -normals[i];
                }
            }


            DrawWindow(_g);
        }
    }

    public int NoVert()
    {
        return Vert.Count;
    }

    public void SetHelpPoint(Point H, Canvas _g)
    {
        if (Vert.Count > 0)
        {
            HelpPoint = H;
            _g.Children.Remove(HelpLine);
            HelpLine.X2 = HelpPoint.X;
            HelpLine.Y2 = HelpPoint.Y;
            _g.Children.Add(HelpLine);
        }
    }


    public void DrawWindow(Canvas _g)
    {
        for (int i = 0; i < Vert.Count - 1; i++)
        {
            _g.Children.Add(Lines[i]);
        }
    }

    public void RemoveWindow(Canvas _g)
    {
        for (int i = 0; i < Vert.Count - 1; i++)
        {
            _g.Children.Remove(Lines[i]);
        }
    }

    public Point Midpoint(int i)
    {
        return new Point((Vert[i].X + Vert[i + 1].X) / 2, (Vert[i].Y + Vert[i + 1].Y) / 2);
    }

    public Vector Normal(int i)
    {
        return normals[i];
    }
}











namespace Clipping
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        bool DrawCohenSutherland = true;
        bool IsCutting = false;
        List<Edge> Edges;

        CohSutWindow CSWindow;
        CyrusBeckWindow CBWindow;

        int IterL = 0;
        int IterR = 0;
        bool DrawingLine = false;
        bool DrawingWindow = false;
        Random rnd;

        public MainWindow()
        {
            InitializeComponent();
            Edges = new List<Edge>();
            rnd = new Random();
            CSWindow = new CohSutWindow();
            CBWindow = new CyrusBeckWindow();
            Helper.Content = "";
            Helper.Content += "Left mouse button click:\n  add line\n";
            Helper.Content += "\nRight mouse button click:\n  draw clipping window\n";
        }


        //nakreslenie vsetkych useciek
        public void DrawEdges()
        {
            for (int i = 0; i < Edges?.Count; i++)
            {
                Edges[i].DrawLine(g);
            }
        }


        //klik lavym tlacidlom - kreslenie usecky
        private void g_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!DrawingWindow)
            {
                if (IterL % 2 == 0)
                {
                    IterL++;
                    DrawingLine = true;
                    Edges.Add(new Edge(e.GetPosition(g), Color.FromRgb(Convert.ToByte(rnd.Next(0, 255)),
                                                                       Convert.ToByte(rnd.Next(0, 255)),
                                                                       Convert.ToByte(rnd.Next(0, 255)))));

                }
                else
                {
                    IterL++;
                    DrawingLine = false;
                    Edges.Last().SetB(e.GetPosition(g), g);

                    //orezavame do okna
                    if (IsCutting)
                    {
                        Edges.Last().RemoveLine(g);
                        if (DrawCohenSutherland)
                        {
                            double L = CSWindow.Left.X;
                            double R = CSWindow.Right.X;
                            double T = CSWindow.Right.Y;
                            double Bot = CSWindow.Left.Y;
                            CSLine(Edges.Last(), L, R, Bot, T);
                        }
                        else
                        {
                            CBLine(Edges.Last());
                        }
                    }
                }
            }
        }


        //hybanie mysou - aktualizuju sa prave kreslene objekty
        private void g_MouseMove(object sender, MouseEventArgs e)
        {
            if (DrawingLine)
            {
                Edges.Last().SetB(e.GetPosition(g), g);
            }

            if (DrawingWindow)
            {
                if (DrawCohenSutherland)
                {
                    CSWindow.SetRectangle(e.GetPosition(g), g);
                }
                else
                {
                    CBWindow.SetHelpPoint(e.GetPosition(g), g);
                }
            }
        }


        //vytvorenie nahodnych useciek
        private void RandomLinesButton_Click(object sender, RoutedEventArgs e)
        {
            int width = Convert.ToInt32(Math.Floor(g.Width));
            int height = Convert.ToInt32(Math.Floor(g.Height));
            for (int i = 0; i < 10; i++)
            {
                Point A = new Point(rnd.Next(width), rnd.Next(height));
                Point B = new Point(rnd.Next(width), rnd.Next(height));
                Color C = Color.FromRgb(Convert.ToByte(rnd.Next(0, 255)),
                                        Convert.ToByte(rnd.Next(0, 255)),
                                        Convert.ToByte(rnd.Next(0, 255)));
                Edges.Add(new Edge(A, C));
                Edges.Last().SetB(B, g);


                //orezavame do okna
                if (IsCutting)
                {
                    Edges.Last().RemoveLine(g);
                    if (DrawCohenSutherland)
                    {
                        double L = CSWindow.Left.X;
                        double R = CSWindow.Right.X;
                        double T = CSWindow.Right.Y;
                        double Bot = CSWindow.Left.Y;
                        CSLine(Edges.Last(), L, R, Bot, T);
                    }
                    else
                    {
                        CBLine(Edges.Last());
                    }
                }
            }
        }


        //vymazanie kresliacej plochy
        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            Edges.Clear();
            g.Children.Clear();
            IterL = 0;
            IterR = 0;
            DrawingWindow = false;
            DrawingLine = false;
            IsCutting = false;

        }


        //klik pravym tlacidlom - kreslenie orezavacieho okna
        private void g_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!DrawingLine)
            {
                //CohenSutherland
                if (DrawCohenSutherland)
                {
                    if (IterR % 2 == 0)
                    {
                        IterR++;
                        DrawingWindow = true;
                        IsCutting = false;
                        CSWindow.NewRectangle(e.GetPosition(g));
                        g.Children.Clear();
                        DrawEdges();

                    }
                    else
                    {
                        IterR++;
                        DrawingWindow = false;
                        CSWindow.SetRectangle(e.GetPosition(g), g);
                        IsCutting = true;
                        CohenSutherland();

                    }
                }
                //CyrusBeck
                else
                {
                    //pri prvom kliku (aj po uzavreti polygonu) sa zacne kreslit vzdy nove okno
                    if (IterR == 0)
                    {
                        IsCutting = false;
                        DrawingWindow = true;
                        IterR++;
                        g.Children.Clear();
                        DrawEdges();
                        CBWindow.Clear();
                        CBWindow.AddVert(e.GetPosition(g), g);
                    }
                    else
                    {
                        CBWindow.AddVert(e.GetPosition(g), g);
                    }
                }
            }
        }

        //pomocna funkcia na vykreslenie usecky
        public void DrawLine(Point V_1, Point V_2, SolidColorBrush b)
        {
            Line L = new Line
            {
                Stroke = b,
                X1 = V_1.X,
                X2 = V_2.X,
                Y1 = V_1.Y,
                Y2 = V_2.Y,
                StrokeThickness = 2
            };
            Canvas.SetZIndex(L, 0);
            g.Children.Add(L);
        }


        //ziskanie kodu algoritmu CohenSutherland pre dany bod
        public Byte GetCode(Point _P, double L, double R, double B, double T)
        {
            Byte b = 0;
            if (_P.X < L) b += 1 << 3;
            if (_P.X > R) b += 1 << 2;
            if (_P.Y < B) b += 1 << 1;
            if (_P.Y > T) b += 1;
            return b;
        }

        //orezanie algoritmom CohenSutherland pre jednu usecku
        private void CSLine(Edge E, double L, double R, double B, double T)
        {
            E.RemoveLine(g);

            Point[] P = new Point[2] { E.GetA(), E.GetB() };
            Byte[] LRBT = new byte[2] { 0, 0 };

            for (int i = 0; i < 2; i++)
            {
                LRBT[i] = GetCode(P[i], L, R, B, T);
            }

            //trivialne pripady - usecka dnu a von
            if ((LRBT[0] | LRBT[1]) == 0)
            {
                DrawLine(P[0], P[1], new SolidColorBrush(E.C));
                return;
            }

            if ((LRBT[0] & LRBT[1]) != 0)
            {
                return;
            }

            double m = (P[1].Y - P[0].Y) / (P[1].X - P[0].X);

            //netrivialne pripady - clipping
            for (int i = 0; i < 2; i++)
            {
                if ((LRBT[i] & (1 << 3)) == (1 << 3))
                {
                    //clip L
                    P[i] = new Point(L, P[i].Y + m * (L - P[i].X));
                    LRBT[i] = GetCode(P[i], L, R, B, T);
                }

                if ((LRBT[i] & (1 << 2)) == (1 << 2))
                {
                    //clip R
                    P[i] = new Point(R, P[i].Y + m * (R - P[i].X));
                    LRBT[i] = GetCode(P[i], L, R, B, T);
                }

                if ((LRBT[i] & (1 << 1)) == (1 << 1))
                {
                    //clip B
                    P[i] = new Point(P[i].X + 1 / m * (B - P[i].Y), B);
                    LRBT[i] = GetCode(P[i], L, R, B, T);
                }

                if ((LRBT[i] & 1) == 1)
                {
                    //clip T
                    P[i] = new Point(P[i].X + 1 / m * (T - P[i].Y), T);
                }
            }

            DrawLine(P[0], P[1], new SolidColorBrush(E.C));

        }

        //Algoritmus CohenSutherland pre vsetky usecky
        private void CohenSutherland()
        {
            double L = CSWindow.Left.X;
            double R = CSWindow.Right.X;
            double T = CSWindow.Right.Y;
            double B = CSWindow.Left.Y;

            g.Children.Clear();
            g.Children.Add(CSWindow.R);

            for (int i = 0; i < Edges.Count; i++)
            {
                CSLine(Edges[i], L, R, B, T);
            }
        }

        //skalarny sucin
        private double DotProduct(Vector u, Vector v)
        {
            return u.X * v.X + u.Y * v.Y;
        }

        //Algoritmus CyrusBeck pre jednu usecku
        private void CBLine(Edge E)
        {
            E.RemoveLine(g);

            Vector _d = E.Tangent();
            double tEnter = 0;
            double tLeave = 1;
            
            for (int i = 0; i < CBWindow.NoEdges(); i++)
            {
                Vector n = CBWindow.Normal(i);
                Vector V = CBWindow.Midpoint(i) - E.GetA();
                

                //enter parameter
                if (DotProduct(_d, n) > 0)
                {
                    double _t = DotProduct(V, n) / DotProduct(_d, n);
                    tEnter = Math.Max(tEnter, _t);
                }
                //leave parameter
                else if (DotProduct(_d, n) < 0)
                {
                    double _t = DotProduct(V, n) / DotProduct(_d, n);
                    tLeave = Math.Min(tLeave, _t);
                }
            }

            //dana usecka pretina nase orezavacie okno
            if (tEnter < tLeave)
            {
                Point _A = E.GetA() + _d * tEnter;
                Point _B = E.GetA() + _d * tLeave;

                DrawLine(_A, _B, new SolidColorBrush(E.C));
            }
        }

        //Algoritmus CyrusBeck pre vsetky usecky
        public void CyrusBeck()
        {
            g.Children.Clear();
            CBWindow.DrawWindow(g);

            for (int i = 0; i < Edges.Count; i++)
            {
                CBLine(Edges[i]);
            }
        }

        //Chceme orezavat algoritmom CyrusBeck
        private void CohenSutherlandRB_Unchecked(object sender, RoutedEventArgs e)
        {
            DrawCohenSutherland = false;
            IsCutting = false;
            IterR = 0;
            g.Children.Clear();
            DrawEdges();

            if (Helper != null)
            {
                Helper.Content = "";
                Helper.Content += "Left mouse button click: \n  add line\n";
                Helper.Content += "\nRight mouse button click:\n  draw clipping window" +
                    "\n    - draw convex polygon\n    - orientation can be arbitrary\n";
                Helper.Content += "\nMouse wheel button click:\n  start clipping\n";
            }
        }

        //Chceme orezavat algoritmom CohenSutherland
        private void CohenSutherlandRB_Checked(object sender, RoutedEventArgs e)
        {
            DrawCohenSutherland = true;
            IsCutting = false;
            IterR = 0;
            g.Children.Clear();
            DrawEdges();

            if (Helper != null)
            {
                Helper.Content = "";
                Helper.Content += "Left mouse button click:\n  add line\n";
                Helper.Content += "\nRight mouse button click:\n  draw clipping window\n";
            }
        }

        //Stlacenie kolecka na mysi - potvrdenie orezavania pre CyrusBeck
        private void g_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int noVert = CBWindow.NoVert();
            if (e.ChangedButton == MouseButton.Middle && !DrawCohenSutherland && DrawingWindow && noVert > 2)
            {
                DrawingWindow = false;
                IsCutting = true;
                IterR = 0;
                CBWindow.CloseWindow(g);
                CyrusBeck();
            }
        }
    }
}
