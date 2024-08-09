// Decompiled with JetBrains decompiler
// Type: Box
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
    int num1 = aux.round((int) this.Center.X);
    int num2 = aux.round((int) this.Center.Y);
    int num3 = aux.round(this.B);
    int num4 = aux.round(this.H);
    return "B " + num3.ToString() + " " + num4.ToString() + " " + num1.ToString() + " " + num2.ToString() + ";\n";
  }

  public void Read(string line, int lineIndex)
  {
    try
    {
      string[] strArray = Regex.Split(Regex.Match(line, "^B\\s*(.*);").Groups[1].Value, "\\s+");
      this.B = int.Parse(strArray[0]);
      this.H = int.Parse(strArray[1]);
      this.Center.X = (float) int.Parse(strArray[2]);
      this.Center.Y = (float) int.Parse(strArray[3]);
    }
    catch
    {
      throw new Exception("Incorrect rectangle on line: " + lineIndex.ToString());
    }
  }

  public void Draw(Canvas cv, Vector2 offset)
  {
    Vector2 vector2 = aux.Scale(this.Center, cv);
    float num1 = aux.Scale(this.B);
    float num2 = aux.Scale(this.H);
    vector2.X -= num1 / 2f;
    vector2.Y -= num2 / 2f;
    Rectangle rectangle = new Rectangle();
    rectangle.Width = (double) num1;
    rectangle.Height = (double) num2;
    rectangle.StrokeThickness = 0.0;
    rectangle.Fill = (Brush) new BrushConverter().ConvertFrom((object) "#cc4c43");
    Rectangle element = rectangle;
    cv.Children.Add((UIElement) element);
    element.SetValue(Canvas.LeftProperty, (object) ((double) vector2.X + (double) offset.X));
    element.SetValue(Canvas.TopProperty, (object) ((double) vector2.Y + (double) offset.Y));
  }
}
