// Decompiled with JetBrains decompiler
// Type: Element
// Assembly: autoMask, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 93C42831-928E-4BBC-8F9F-03F21363630D
// Assembly location: D:\TFM\automask\autoMask.dll

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable enable
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
      int x = (int) this.Position.X;
      int y = (int) this.Position.Y;
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
      int num = int.Parse(Regex.Match(line, "^C([0-9]+)(.*);").Groups[1].Value);
      if (Regex.IsMatch(line, "T\\s*(.*);"))
      {
        string[] strArray = Regex.Split(Regex.Match(line, "T\\s*(.*);").Groups[1].Value, "\\s+");
        vector2.X = (float) int.Parse(strArray[0]);
        vector2.Y = (float) int.Parse(strArray[1]);
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
      circle.Draw(cv, aux.Scale(this.Position, cv));
    foreach (Box box in this.Boxes)
      box.Draw(cv, aux.Scale(this.Position, cv));
    foreach (Wire wire in this.Wires)
      wire.Draw(cv, aux.Scale(this.Position, cv));
    foreach (Element subElement in this.SubElements)
      subElement.Draw(cv);
  }

  public void DrawChamber(Canvas cv, Vector2 offset)
  {
    offset.Y += aux.Scale(5660000);
    Vector2 vector2_1 = new Vector2();
    vector2_1.X = (float) (-(double) aux.Scale(4000000) / 2.0);
    vector2_1.Y = aux.Scale(25000000) + aux.Scale(4000000) / 2f;
    Ellipse ellipse1 = new Ellipse();
    ellipse1.Width = (double) aux.Scale(4000000);
    ellipse1.Height = (double) aux.Scale(4000000);
    ellipse1.Stroke = (Brush) new BrushConverter().ConvertFrom((object) "#232323");
    ellipse1.StrokeThickness = 200.0;
    ellipse1.Fill = (Brush) Brushes.Transparent;
    Ellipse element1 = ellipse1;
    cv.Children.Add((UIElement) element1);
    element1.SetValue(Canvas.LeftProperty, (object) ((double) vector2_1.X + (double) offset.X));
    element1.SetValue(Canvas.TopProperty, (object) ((double) vector2_1.Y + (double) offset.Y));
    Vector2 vector2_2 = new Vector2();
    vector2_2.X = (float) (-(double) aux.Scale(4000000) / 2.0);
    vector2_2.Y = (float) (-(double) aux.Scale(25000000) + (double) aux.Scale(4000000) / 2.0);
    Ellipse ellipse2 = new Ellipse();
    ellipse2.Width = (double) aux.Scale(4000000);
    ellipse2.Height = (double) aux.Scale(4000000);
    ellipse2.Stroke = (Brush) new BrushConverter().ConvertFrom((object) "#232323");
    ellipse2.StrokeThickness = 200.0;
    ellipse2.Fill = (Brush) Brushes.Transparent;
    Ellipse element2 = ellipse2;
    cv.Children.Add((UIElement) element2);
    element2.SetValue(Canvas.LeftProperty, (object) ((double) vector2_2.X + (double) offset.X));
    element2.SetValue(Canvas.TopProperty, (object) ((double) vector2_2.Y + (double) offset.Y));
    Vector2 vector2_3 = new Vector2();
    vector2_3.X = (float) (-(double) aux.Scale(25000000) / 2.0);
    vector2_3.Y = (float) (-(double) aux.Scale(70000000) / 2.0);
    Rectangle rectangle1 = new Rectangle();
    rectangle1.Width = (double) aux.Scale(25000000);
    rectangle1.Height = (double) aux.Scale(70000000);
    rectangle1.Stroke = (Brush) new BrushConverter().ConvertFrom((object) "#232323");
    rectangle1.StrokeThickness = 200.0;
    rectangle1.Fill = (Brush) Brushes.Transparent;
    Rectangle element3 = rectangle1;
    cv.Children.Add((UIElement) element3);
    element3.SetValue(Canvas.LeftProperty, (object) ((double) vector2_3.X + (double) offset.X));
    element3.SetValue(Canvas.TopProperty, (object) ((double) vector2_3.Y + (double) offset.Y));
    Vector2 vector2_4 = new Vector2();
    vector2_4.X = (float) (-(double) aux.Scale(10000000) / 2.0);
    vector2_4.Y = -aux.Scale(14660000);
    Rectangle rectangle2 = new Rectangle();
    rectangle2.Width = (double) aux.Scale(10000000);
    rectangle2.Height = (double) aux.Scale(17000000);
    rectangle2.Stroke = (Brush) new BrushConverter().ConvertFrom((object) "#232323");
    rectangle2.StrokeThickness = 200.0;
    rectangle2.Fill = (Brush) Brushes.Transparent;
    Rectangle element4 = rectangle2;
    cv.Children.Add((UIElement) element4);
    element4.SetValue(Canvas.LeftProperty, (object) ((double) vector2_4.X + (double) offset.X));
    element4.SetValue(Canvas.TopProperty, (object) ((double) vector2_4.Y + (double) offset.Y));
  }
}
