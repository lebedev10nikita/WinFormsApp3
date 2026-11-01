using System.ComponentModel.Design.Serialization;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinFormsApp3
{
	public partial class Form1 : Form
	{
		
		public int start_X;
		public int start_Y;
		public static List<Shape> L = new List<Shape>();
		public Form1()
		{
			InitializeComponent();
			


		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;


			foreach (Shape i in L)
			{
				i.Draw(g);
			}
			drawShell(g);


		}

		private void Form1_MouseDown(object sender, MouseEventArgs e)
		{

			if (e.Button == MouseButtons.Right)
			{
				for (int i = 0; i < L.Count; i++)
				{
				if (L[i].Is_Inside(e.X, e.Y) == true)
					{
					L.Remove(L[i]);
					}
				}
				Refresh();
			}

			if (e.Button == MouseButtons.Left)
			{
				bool flag = false;
				foreach (Shape i in L)
				{
					if (i.Is_Inside(e.X, e.Y) == true)
					{
						flag = true;
						i.Is_Pin = true;
						i.delta_X = i.Coord_x - e.X;
						i.delta_Y = i.Coord_y - e.Y;
					}
				}
				if (flag == false)
				{
					L.Add(new Triangle(e.X, e.Y));
					Refresh();
				}

			}
		}



		private void Form1_MouseMove(object sender, MouseEventArgs e)
		{


			foreach (Shape i in L)

			{
			if (i.Is_Pin == true)
				{

					i.Coord_x = e.X + i.delta_X;
					i.Coord_y = e.Y + i.delta_Y;
					Refresh();
				}
				
				
			}
			

		}

		private void Form1_MouseUp(object sender, MouseEventArgs e)
		{
		foreach (Shape i in L) i.Is_Pin = false;
		}

		private void drawShell(Graphics e)
		{
			if (L.Count >= 3)
			{
				for (int i = 0; i < L.Count; i++)
				{
					for (int j = i + 1; j < L.Count; j++)
					{
						int less = 0; int more = 0; 
						float k; float b;
						if (L[i].Coord_x != L[j].Coord_x)
						{
							k = (float)(L[i].Coord_y - L[j].Coord_y) / (L[i].Coord_x - L[j].Coord_x);
							b = (L[i].Coord_y - k * L[i].Coord_x);

							for (int points = 0; points < L.Count; points++)
							{
								if (points == i || points == j) { continue; }
								else if (k * L[points].Coord_x + b > L[points].Coord_y) { more += 1; }
								else if (k * L[points].Coord_x + b < L[points].Coord_y) { less += 1; }
							}

							if (more == 0 || less == 0)
							{
								e.DrawLine(new Pen(Color.Black, 2), L[i].Coord_x, L[i].Coord_y, L[j].Coord_x, L[j].Coord_y);
							}
						}
						else
						{
							for (int p = 0; p < L.Count; p++)
							{
								if (p == i || p == j) { continue; }
								else if (L[p].Coord_x > L[i].Coord_x) { more += 1; }
								else if (L[p].Coord_x < L[i].Coord_x) { less += 1; }
							}

							if (more == 0 || less == 0)
							{
								e.DrawLine(new Pen(Color.Black, 2), L[i].Coord_x, L[i].Coord_y, L[j].Coord_x, L[j].Coord_y);
							}
						}
					}
				}
			}
		}


	}
	public abstract class Shape
	{
		public enum chosen_shape{ Circle, Triangle, Square}
		public int delta_X;
		public int delta_Y;
		public bool Is_Pin;
		public int Coord_x;
		public int Coord_y;
		public static int Radius;
		public bool DragOrDrop;
		public static Color color;
		public static Pen p;



		static Shape()
		{
			Radius = 50;
			color = Color.Red;
		}
		public Shape(int x, int y)
		{
			Coord_x = x;
			Coord_y = y;
			p = new Pen(color);
		}
		public int X
		{
			get { return Coord_x; }
			set { Coord_x = value; }
		}
		public int Y
		{
			get { return Coord_y; }
			set { Coord_y = value; }
		}
		public int Rad
		{
			get { return Radius; }
			set
			{
				try
				{
					if (value > 0) { Radius = value; }
					else
					{
						throw new Exception(@">0");
						Radius = 10;

					}
				}
				catch (Exception error) { Console.WriteLine("Ошибка!" + error.Message); }
			}
		}


		public virtual void Draw(Graphics g) { }
		public abstract bool Is_Inside(int mouse_x, int mouse_y);
	}



	class Circle : Shape
	{
		public override bool Is_Inside(int mouse_x, int mouse_y)
		{
			if ((mouse_x- Coord_x) *  (mouse_x - Coord_x) + (mouse_y-Coord_y) * (mouse_y - Coord_y) < Rad * Rad) return true;
			else return false;
		}
		public Circle(int x, int y) : base(x, y) { }

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			g.DrawEllipse(p, Coord_x - Rad, Coord_y - Rad, 2 * Rad, 2 * Rad);
		}
	}



	class Square : Shape
	{
		public override bool Is_Inside(int mouse_x, int mouse_y)
		{
			if (Coord_x - Rad < mouse_x)
			{
				if (Coord_x + Rad > mouse_x)
				{
					if (Coord_y - Rad < mouse_y)
					{
						if (Coord_y + Rad > mouse_y) { return true; }
					}
				}
			}
			 return false;
		}


		public Square(int x, int y) : base(x, y) { }


		public override void Draw(Graphics g)
		{
			base.Draw(g);
			g.DrawRectangle(p, Coord_x - Rad, Coord_y - Rad, 2 * Rad, 2 * Rad);
		}
	}




	class Triangle : Shape
	{

		public double length_of_line(double x1,  double y1, double x2, double y2) 
		{
			return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
		}

		public double Gerone(double l1, double l2, double l3){
			double p = (l1 + l2 + l3) / 2.0;
			return Math.Sqrt(p  * (p - l1) * (p - l2) * (p - l3));
		}


		public override bool Is_Inside(int mouse_x, int mouse_y)
		{
			double x1 = Coord_x;
			double y1 = Coord_y - Rad;
			double x2 = Coord_x + Rad * Math.Cos(Math.PI / 6);
			double y2 = Coord_y + Rad * Math.Sin(Math.PI / 6);
			double x3 = Coord_x - Rad * Math.Cos(Math.PI / 6);
			double y3 = Coord_y + Rad * Math.Sin(Math.PI / 6);

			double l1 = length_of_line(x1, y1, x2, y2);
			double l2 = length_of_line(x2, y2, x3, y3);
			double l3 = length_of_line(x3, y3, x1, y1);

			double m1 = length_of_line(mouse_x, mouse_y, x1, y1);
			double m2 = length_of_line(mouse_x, mouse_y, x2, y2);
			double m3 = length_of_line(mouse_x, mouse_y, x3, y3);


			double sBig = Gerone(l1, l2, l3);
			double s1 = Gerone(l1, m1, m2);
			double s2 = Gerone(l2, m2, m3);
			double s3 = Gerone(l3, m3, m1);


			if (Math.Abs(s1 + s2 + s3 - sBig) < 0.1) { return true; }
			return false;
		}
		public Triangle(int x, int y) : base(x, y) { }
		public override void Draw(Graphics g)
		{
			base.Draw(g);
			g.DrawLine(p, Coord_x , Coord_y - Rad, (float)(Coord_x + Rad * Math.Sqrt(3) / 2), (float)(Coord_y + Rad * Math.PI / 6));
			g.DrawLine(p, (float)(Coord_x + Rad * Math.Sqrt(3) / 2), (float)(Coord_y + Rad * Math.PI / 6), (float)(Coord_x - Rad * Math.Sqrt(3) / 2), (float)(Coord_y + Rad * Math.PI / 6));
			g.DrawLine(p, Coord_x , Coord_y - Rad, (float)(Coord_x - Rad * Math.Sqrt(3) / 2), (float)(Coord_y + Rad * Math.PI / 6));
		}
	}
}
