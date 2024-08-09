// Decompiled with JetBrains decompiler
// Type: Wire
// Assembly: autoMask, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 93C42831-928E-4BBC-8F9F-03F21363630D
// Assembly location: D:\TFM\automask\autoMask.dll

using autoMask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable enable
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
      string str2 = str1 + "W " + aux.round(this.Widths[index]).ToString();
      int num2 = aux.round((int) this.Points[index].X);
      int y = (int) this.Points[index].Y;
      string str3 = str2 + " " + num2.ToString() + " " + y.ToString();
      int num3;
      for (num3 = 0; num3 < this.Points.Count<Vector2>() - 1 - index; ++num3)
      {
        int num4 = aux.round((int) this.Points[num3 + 1 + index].X);
        int num5 = aux.round((int) this.Points[num3 + 1 + index].Y);
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
        vector2.X = (float) int.Parse(strArray[index]);
        vector2.Y = (float) int.Parse(strArray[index + 1]);
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
      line.X1 = (double) aux.Scale(this.Points[index], cv).X + (double) offset.X;
      line.Y1 = (double) aux.Scale(this.Points[index], cv).Y + (double) offset.Y;
      line.X2 = (double) aux.Scale(this.Points[index + 1], cv).X + (double) offset.X;
      line.Y2 = (double) aux.Scale(this.Points[index + 1], cv).Y + (double) offset.Y;
      line.StrokeThickness = (double) aux.Scale(this.Widths[index]);
      line.Stroke = (Brush) new BrushConverter().ConvertFrom((object) "#cc4c43");
      Line element = line;
      element.SnapsToDevicePixels = true;
      element.UseLayoutRounding = true;
      cv.Children.Add((UIElement) element);
    }
  }

  public void UpdateLengths()
  {
    this.Lengths = new List<int>();
    for (int index = 0; index < this.Points.Count<Vector2>() - 1; ++index)
      this.Lengths.Add((int) Vector2.Distance(this.Points[index], this.Points[index + 1]));
    this.Length = 0;
    for (int index = 0; index < this.Lengths.Count<int>(); ++index)
      this.Length += this.Lengths[index];
    this.Resistance = 0.0;
    for (int index = 0; index < this.Lengths.Count<int>(); ++index)
    {
      if (this.Widths[index] != 0)
        this.Resistance += (double) this.Lengths[index] * Math.Pow(10.0, -9.0) / ((double) MainWindow.sputteringMaterial * (double) MainWindow.sputteringHeight * Math.Pow(10.0, -9.0) * (double) this.Widths[index] * Math.Pow(10.0, -9.0));
    }
  }
}
