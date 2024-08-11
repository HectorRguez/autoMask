using autoMask;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

public class CifFile
{
  public string Author;
  public List<int> Layers;
  public Element MainElement;
  public string? SavePath;
  private string? data;
  public List<Element> InstanciatedElements = new List<Element>();

  public string Data
  {
    get => this.data == null ? this.Write() : this.data;
    set => this.data = value;
  }

  public CifFile(string author, List<int> layers, string savePath)
  {
    this.Author = author;
    this.Layers = layers;
    this.SavePath = savePath;
    this.MainElement = new Element();
  }

  public CifFile(string savePath)
  {
    this.SavePath = savePath;
    this.MainElement = new Element();
  }

  public void Save()
  {
    if (this.SavePath == null)
      throw new Exception("Invalid saving attempt");
    using (StreamWriter streamWriter = new StreamWriter(this.SavePath))
      ((TextWriter) streamWriter).Write(this.Data);
  }

  private string Write()
  {
    string str = "(CIF written by " + this.Author + ");\n(Layer names:);\n";
    for (int index = 0; index < this.Layers.Count; ++index)
      str = str + "L L" + this.Layers[index].ToString() + "; (CleWin: Layer 0 / 0fe08080 0fe08080);\n";
    for (int index = 0; index < this.MainElement.SubElements.Count; ++index)
      str += this.MainElement.SubElements[index].Write();
    return str + this.MainElement.Write() + this.MainElement.Instanciate() + "E";
  }

  public void Read()
  {
    using (StreamReader streamReader = new StreamReader(this.SavePath))
      this.Data = ((TextReader) streamReader).ReadToEnd();
    using (StringReader stringReader = new StringReader(this.Data))
    {
      Element element1 = new Element();
      int lineIndex = -1;

      string line;
      while (true)
      {
        line = ((TextReader) stringReader).ReadLine();
        ++lineIndex;
        if (line != null)
        {
          switch (line[0])
          {
            case '(':
            case 'L':
              continue;
            case '9':
              element1.ReadName(line, lineIndex);
              continue;
            case 'B':
              Box box = new Box();
              box.Read(line, lineIndex);
              element1.Boxes.Add(box);
              continue;
            case 'C':
              Element element2 = new Element();
              element2.ReadInstanciation(line, lineIndex);
              for (int index = 0; index < this.InstanciatedElements.Count<Element>(); ++index)
              {
                if (this.InstanciatedElements[index].Index == element2.Index)
                {
                  this.InstanciatedElements[index].Position = element2.Position;
                  element1.SubElements.Add(this.InstanciatedElements[index]);
                }
              }
              continue;
            case 'D':
              if (line[1] == 'S')
              {
                element1.ReadDeclaration(line, lineIndex);
                continue;
              }
              else if (line[1] == 'F')
              {
                this.InstanciatedElements.Add(element1);
                element1 = new Element();
                continue;
              }
              else return;
            case 'E':
              this.MainElement = element1.SubElements[0];
              return;
            case 'R':
              Circle circle = new Circle();
              circle.Read(line, lineIndex);
              element1.Circles.Add(circle);
              continue;
            case 'W':
              Wire wire = new Wire();
              wire.Read(line, lineIndex);
              element1.Wires.Add(wire);
              continue;
            default:
              return;
          }
        }
        else
          break;
      }
      return;
    }
  }
  public void Draw(Canvas cv) => this.MainElement.Draw(cv);
}

public class Element
{
    public int Index;
    public string Name;
    public int Layer;
    public List<Circle> Circles;
    public List<Box> Boxes;
    public List<Wire> Wires;
    public List<Element> SubElements;
    public Vector2 Position;
    public Canvas Cv;

    private string nameMod => this.Name == null ? "" : this.Name.Replace(' ', '~');

    public Element(
      int index,
      string name,
      int layer,
      Vector2 position,
      List<Circle> circles,
      List<Box> boxes,
      List<Wire> wires)
    {
        this.Index = index;
        this.Name = name;
        this.Layer = layer;
        this.Position = position;
        this.Circles = circles;
        this.Boxes = boxes;
        this.Wires = wires;
        this.SubElements = new List<Element>();
        this.Circles = new List<Circle>();
        this.Boxes = new List<Box>();
        this.Wires = new List<Wire>();
    }

    public Element(int index, string name, int layer, List<Element> subElements)
    {
        this.Index = index;
        this.Name = name;
        this.Layer = layer;
        this.SubElements = subElements;
        this.Position = new Vector2(0.0f, 0.0f);
        this.SubElements = new List<Element>();
        this.Circles = new List<Circle>();
        this.Boxes = new List<Box>();
        this.Wires = new List<Wire>();
    }

    public Element()
    {
        this.SubElements = new List<Element>();
        this.Circles = new List<Circle>();
        this.Boxes = new List<Box>();
        this.Wires = new List<Wire>();
    }

    public string Write()
    {
        string str = "DS " + this.Index.ToString() + " 1 10;\n9 " + this.nameMod + ";\nL L" + this.Layer.ToString() + ";\n";
        if (this.Circles != null)
        {
            for (int index = 0; index < this.Circles.Count; ++index)
                str += this.Circles[index].Write();
        }
        if (this.Boxes != null)
        {
            for (int index = 0; index < this.Boxes.Count; ++index)
                str += this.Boxes[index].Write();
        }
        if (this.Wires != null)
        {
            for (int index = 0; index < this.Wires.Count; ++index)
                str += this.Wires[index].Write();
        }
        if (this.SubElements != null)
        {
            for (int index = 0; index < this.SubElements.Count; ++index)
                str += this.SubElements[index].Instanciate();
        }
        return str + "DF;\n";
    }

    public string Instanciate()
    {
        string str = "C" + this.Index.ToString();
        if (this.Position != new Vector2(0.0f, 0.0f))
        {
            int x = (int)this.Position.X;
            int y = (int)this.Position.Y;
            str = str + " T " + x.ToString() + " " + y.ToString();
        }
        return str + ";\n";
    }

    public void ReadName(string line, int lineIndex)
    {
        try
        {
            this.Name = Regex.Match(line, "^9\\s*(.*);").Groups[1].Value;
        }
        catch
        {
            throw new Exception("Incorrect name on line: " + lineIndex.ToString());
        }
    }

    public void ReadDeclaration(string line, int lineIndex)
    {
        List<Vector2> vector2List = new List<Vector2>();
        try
        {
            this.Index = int.Parse(Regex.Match(line, "^DS\\s*([0-9]+) ").Groups[1].Value);
        }
        catch
        {
            throw new Exception("Incorrect object declaration on line: " + lineIndex.ToString());
        }
    }

    public void ReadInstanciation(string line, int lineIndex)
    {
        Vector2 vector2 = new Vector2(0.0f, 0.0f);
        try
        {
            int num = int.Parse(Regex.Match(line, "^C\\s*([0-9]+)(.*);").Groups[1].Value);
            if (Regex.IsMatch(line, "T\\s*(.*);"))
            {
                string[] strArray = Regex.Split(Regex.Match(line, "T\\s*(.*);").Groups[1].Value, "\\s+");
                vector2.X = (float)int.Parse(strArray[0]);
                vector2.Y = (float)int.Parse(strArray[1]);
            }
            this.Index = num;
            this.Position = vector2;
        }
        catch
        {
            throw new Exception("Incorrect object instanciation on line: " + lineIndex.ToString());
        }
    }

    public void Draw(Canvas cv)
    {
        foreach (Circle circle in this.Circles)
            circle.Draw(cv, auxFun.Scale(this.Position, cv));
        foreach (Box box in this.Boxes)
            box.Draw(cv, auxFun.Scale(this.Position, cv));
        foreach (Wire wire in this.Wires)
            wire.Draw(cv, auxFun.Scale(this.Position, cv));
        foreach (Element subElement in this.SubElements)
            subElement.Draw(cv);
    }

    public void DrawChamber(Canvas cv, Vector2 offset)
    {
        offset.Y += auxFun.Scale(5660000);
        Vector2 vector2_1 = new Vector2();
        vector2_1.X = (float)(-(double)auxFun.Scale(4000000) / 2.0);
        vector2_1.Y = auxFun.Scale(25000000) + auxFun.Scale(4000000) / 2f;
        Ellipse ellipse1 = new Ellipse();
        ellipse1.Width = (double)auxFun.Scale(4000000);
        ellipse1.Height = (double)auxFun.Scale(4000000);
        ellipse1.Stroke = (Brush)new BrushConverter().ConvertFrom((object)"#232323");
        ellipse1.StrokeThickness = 1.0;
        ellipse1.Fill = (Brush)Brushes.Transparent;
        Ellipse element1 = ellipse1;
        cv.Children.Add((UIElement)element1);
        element1.SetValue(Canvas.LeftProperty, (object)((double)vector2_1.X + (double)offset.X));
        element1.SetValue(Canvas.TopProperty, (object)((double)vector2_1.Y + (double)offset.Y));
        Vector2 vector2_2 = new Vector2();
        vector2_2.X = (float)(-(double)auxFun.Scale(4000000) / 2.0);
        vector2_2.Y = (float)(-(double)auxFun.Scale(25000000) + (double)auxFun.Scale(4000000) / 2.0);
        Ellipse ellipse2 = new Ellipse();
        ellipse2.Width = (double)auxFun.Scale(4000000);
        ellipse2.Height = (double)auxFun.Scale(4000000);
        ellipse2.Stroke = (Brush)new BrushConverter().ConvertFrom((object)"#232323");
        ellipse2.StrokeThickness = 1.0;
        ellipse2.Fill = (Brush)Brushes.Transparent;
        Ellipse element2 = ellipse2;
        cv.Children.Add((UIElement)element2);
        element2.SetValue(Canvas.LeftProperty, (object)((double)vector2_2.X + (double)offset.X));
        element2.SetValue(Canvas.TopProperty, (object)((double)vector2_2.Y + (double)offset.Y));
        Vector2 vector2_3 = new Vector2();
        vector2_3.X = (float)(-(double)auxFun.Scale(25000000) / 2.0);
        vector2_3.Y = (float)(-(double)auxFun.Scale(70000000) / 2.0);
        Rectangle rectangle1 = new Rectangle();
        rectangle1.Width = (double)auxFun.Scale(25000000);
        rectangle1.Height = (double)auxFun.Scale(70000000);
        rectangle1.Stroke = (Brush)new BrushConverter().ConvertFrom((object)"#232323");
        rectangle1.StrokeThickness = 1.0;
        rectangle1.Fill = (Brush)Brushes.Transparent;
        Rectangle element3 = rectangle1;
        cv.Children.Add((UIElement)element3);
        element3.SetValue(Canvas.LeftProperty, (object)((double)vector2_3.X + (double)offset.X));
        element3.SetValue(Canvas.TopProperty, (object)((double)vector2_3.Y + (double)offset.Y));
        Vector2 vector2_4 = new Vector2();
        vector2_4.X = (float)(-(double)auxFun.Scale(10000000) / 2.0);
        vector2_4.Y = -auxFun.Scale(14660000);
        Rectangle rectangle2 = new Rectangle();
        rectangle2.Width = (double)auxFun.Scale(10000000);
        rectangle2.Height = (double)auxFun.Scale(17000000);
        rectangle2.Stroke = (Brush)new BrushConverter().ConvertFrom((object)"#232323");
        rectangle2.StrokeThickness = 1.0;
        rectangle2.Fill = (Brush)Brushes.Transparent;
        Rectangle element4 = rectangle2;
        cv.Children.Add((UIElement)element4);
        element4.SetValue(Canvas.LeftProperty, (object)((double)vector2_4.X + (double)offset.X));
        element4.SetValue(Canvas.TopProperty, (object)((double)vector2_4.Y + (double)offset.Y));
    }
}

public class Circle
{
    public Vector2 Center;
    public int D;

    public Circle(Vector2 center, int d)
    {
        this.Center = center;
        this.D = d;
    }

    public Circle()
    {
    }

    public void Read(string line, int lineIndex)
    {
        try
        {
            string[] strArray = Regex.Split(Regex.Match(line, "^R\\s*(.*);").Groups[1].Value, "\\s+");
            this.D = int.Parse(strArray[0]);
            this.Center.X = (float)int.Parse(strArray[1]);
            this.Center.Y = (float)int.Parse(strArray[2]);
        }
        catch
        {
            throw new Exception("Incorrect circle on line: " + lineIndex.ToString());
        }
    }

    public string Write()
    {
        int num1 = auxFun.round((int)this.Center.X);
        int num2 = auxFun.round((int)this.Center.Y);
        return "R " + auxFun.round(this.D).ToString() + " " + num1.ToString() + " " + num2.ToString() + ";\n";
    }

    public void Draw(Canvas cv, Vector2 offset)
    {
        Vector2 vector2 = auxFun.Scale(this.Center, cv);
        float num = auxFun.Scale(this.D);
        vector2.X -= num / 2f;
        vector2.Y -= num / 2f;
        Ellipse ellipse = new Ellipse();
        ellipse.Width = (double)num;
        ellipse.Height = (double)num;
        ellipse.Stroke = (Brush)new BrushConverter().ConvertFrom((object)"#cc4c43");
        ellipse.StrokeThickness = 1.0;
        ellipse.Fill = (Brush)new BrushConverter().ConvertFrom((object)"#cc4c43");
        Ellipse element = ellipse;
        cv.Children.Add((UIElement)element);
        element.SetValue(Canvas.LeftProperty, (object)((double)vector2.X + (double)offset.X));
        element.SetValue(Canvas.TopProperty, (object)((double)vector2.Y + (double)offset.Y));
    }
}

public class Box
{
    public Vector2 Center;
    public int B;
    public int H;

    public Box(Vector2 center, int b, int h)
    {
        this.Center = center;
        this.B = b;
        this.H = h;
    }

    public Box()
    {
    }

    public string Write()
    {
        int num1 = auxFun.round((int)this.Center.X);
        int num2 = auxFun.round((int)this.Center.Y);
        int num3 = auxFun.round(this.B);
        int num4 = auxFun.round(this.H);
        return "B " + num3.ToString() + " " + num4.ToString() + " " + num1.ToString() + " " + num2.ToString() + ";\n";
    }

    public void Read(string line, int lineIndex)
    {
        try
        {
            string[] strArray = Regex.Split(Regex.Match(line, "^B\\s*(.*);").Groups[1].Value, "\\s+");
            this.B = int.Parse(strArray[0]);
            this.H = int.Parse(strArray[1]);
            this.Center.X = (float)int.Parse(strArray[2]);
            this.Center.Y = (float)int.Parse(strArray[3]);
        }
        catch
        {
            throw new Exception("Incorrect rectangle on line: " + lineIndex.ToString());
        }
    }

    public void Draw(Canvas cv, Vector2 offset)
    {
        Vector2 vector2 = auxFun.Scale(this.Center, cv);
        float num1 = auxFun.Scale(this.B);
        float num2 = auxFun.Scale(this.H);
        vector2.X -= num1 / 2f;
        vector2.Y -= num2 / 2f;
        Rectangle rectangle = new Rectangle();
        rectangle.Width = (double)num1;
        rectangle.Height = (double)num2;
        rectangle.StrokeThickness = 0.0;
        rectangle.Fill = (Brush)new BrushConverter().ConvertFrom((object)"#cc4c43");
        Rectangle element = rectangle;
        cv.Children.Add((UIElement)element);
        element.SetValue(Canvas.LeftProperty, (object)((double)vector2.X + (double)offset.X));
        element.SetValue(Canvas.TopProperty, (object)((double)vector2.Y + (double)offset.Y));
    }
}

public class Wire
{
    public List<Vector2> Points;
    public List<int> Widths;
    public int Length;
    public double Resistance;
    public List<int> Lengths;

    public Wire(List<Vector2> points, List<int> w)
    {
        this.Points = points;
        this.Widths = w;
    }

    public Wire()
    {
        this.Points = new List<Vector2>();
        this.Widths = new List<int>();
    }

    public string Write()
    {
        string str1 = "";
        int num1;
        for (int index = 0; index < this.Points.Count<Vector2>() - 1; index = num1 + 1)
        {
            string str2 = str1 + "W " + auxFun.round(this.Widths[index]).ToString();
            int num2 = auxFun.round((int)this.Points[index].X);
            int y = (int)this.Points[index].Y;
            string str3 = str2 + " " + num2.ToString() + " " + y.ToString();
            int num3;
            for (num3 = 0; num3 < this.Points.Count<Vector2>() - 1 - index; ++num3)
            {
                int num4 = auxFun.round((int)this.Points[num3 + 1 + index].X);
                int num5 = auxFun.round((int)this.Points[num3 + 1 + index].Y);
                str3 = str3 + " " + num4.ToString() + " " + num5.ToString();
                if (num3 == this.Points.Count<Vector2>() - 2 - index || this.Widths[num3 + 1 + index] != this.Widths[index])
                    break;
            }
            num1 = index + num3;
            str1 = str3 + ";\n";
        }
        return str1;
    }

    public void Read(string line, int lineIndex)
    {
        try
        {
            string[] strArray = Regex.Split(Regex.Match(line, "^W\\s*(.*);").Groups[1].Value, "\\s+");
            for (int index = 1; index < strArray.Length - 1; index += 2)
            {
                Vector2 vector2;
                vector2.X = (float)int.Parse(strArray[index]);
                vector2.Y = (float)int.Parse(strArray[index + 1]);
                this.Points.Add(vector2);
                this.Widths.Add(int.Parse(strArray[0]));
            }
        }
        catch
        {
            throw new Exception("Incorrect wire on line: " + lineIndex.ToString());
        }
    }

    public void Draw(Canvas cv, Vector2 offset)
    {
        for (int index = 0; index < this.Points.Count<Vector2>() - 1; ++index)
            new Circle(this.Points[index], this.Widths[index]).Draw(cv, offset);
        for (int index = 1; index < this.Points.Count<Vector2>(); ++index)
            new Circle(this.Points[index], this.Widths[index - 1]).Draw(cv, offset);
        for (int index = 0; index < this.Points.Count<Vector2>() - 1; ++index)
        {
            Line line = new Line();
            line.X1 = (double)auxFun.Scale(this.Points[index], cv).X + (double)offset.X;
            line.Y1 = (double)auxFun.Scale(this.Points[index], cv).Y + (double)offset.Y;
            line.X2 = (double)auxFun.Scale(this.Points[index + 1], cv).X + (double)offset.X;
            line.Y2 = (double)auxFun.Scale(this.Points[index + 1], cv).Y + (double)offset.Y;
            line.StrokeThickness = (double)auxFun.Scale(this.Widths[index]);
            line.Stroke = (Brush)new BrushConverter().ConvertFrom((object)"#cc4c43");
            Line element = line;
            element.SnapsToDevicePixels = true;
            element.UseLayoutRounding = true;
            cv.Children.Add((UIElement)element);
        }
    }

    public void UpdateLengths()
    {
        this.Lengths = new List<int>();
        for (int index = 0; index < this.Points.Count<Vector2>() - 1; ++index)
            this.Lengths.Add((int)Vector2.Distance(this.Points[index], this.Points[index + 1]));
        this.Length = 0;
        for (int index = 0; index < this.Lengths.Count<int>(); ++index)
            this.Length += this.Lengths[index];
        this.Resistance = 0.0;
        for (int index = 0; index < this.Lengths.Count<int>(); ++index)
        {
            if (this.Widths[index] != 0)
                this.Resistance += (double)this.Lengths[index] * Math.Pow(10.0, -9.0) / ((double)MainWindow.sputteringMaterial * (double)MainWindow.sputteringHeight * Math.Pow(10.0, -9.0) * (double)this.Widths[index] * Math.Pow(10.0, -9.0));
        }
    }
}

