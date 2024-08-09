// Decompiled with JetBrains decompiler
// Type: Circle
// Assembly: autoMask, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 93C42831-928E-4BBC-8F9F-03F21363630D
// Assembly location: D:\TFM\automask\autoMask.dll

using System;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable enable
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
      this.Center.X = (float) int.Parse(strArray[1]);
      this.Center.Y = (float) int.Parse(strArray[2]);
    }
    catch
    {
      throw new Exception("Incorrect circle on line: " + lineIndex.ToString());
    }
  }

  public string Write()
  {
    int num1 = aux.round((int) this.Center.X);
    int num2 = aux.round((int) this.Center.Y);
    return "R " + aux.round(this.D).ToString() + " " + num1.ToString() + " " + num2.ToString() + ";\n";
  }

  public void Draw(Canvas cv, Vector2 offset)
  {
    Vector2 vector2 = aux.Scale(this.Center, cv);
    float num = aux.Scale(this.D);
    vector2.X -= num / 2f;
    vector2.Y -= num / 2f;
    Ellipse ellipse = new Ellipse();
    ellipse.Width = (double) num;
    ellipse.Height = (double) num;
    ellipse.Stroke = (Brush) new BrushConverter().ConvertFrom((object) "#cc4c43");
    ellipse.StrokeThickness = 1.0;
    ellipse.Fill = (Brush) new BrushConverter().ConvertFrom((object) "#cc4c43");
    Ellipse element = ellipse;
    cv.Children.Add((UIElement) element);
    element.SetValue(Canvas.LeftProperty, (object) ((double) vector2.X + (double) offset.X));
    element.SetValue(Canvas.TopProperty, (object) ((double) vector2.Y + (double) offset.Y));
  }
}
