// Decompiled with JetBrains decompiler
// Type: cifFile
// Assembly: autoMask, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 93C42831-928E-4BBC-8F9F-03F21363630D
// Assembly location: D:\TFM\automask\autoMask.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable enable
public class cifFile
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

  public cifFile(string author, List<int> layers, string savePath)
  {
    this.Author = author;
    this.Layers = layers;
    this.SavePath = savePath;
    this.MainElement = new Element();
  }

  public cifFile(string savePath)
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
      str = str + "L L" + this.Layers[index].ToString() + " (CleWin: Layer 0 / 0fe08080 0fe08080);\n";
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
label_7:
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
              goto label_15;
            case 'D':
              if (line[1] == 'S')
              {
                element1.ReadDeclaration(line, lineIndex);
                continue;
              }
              if (line[1] == 'F')
              {
                this.InstanciatedElements.Add(element1);
                element1 = new Element();
                continue;
              }
              goto label_9;
            case 'E':
              goto label_24;
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
              goto label_20;
          }
        }
        else
          break;
      }
      return;
label_20:
      return;
label_9:
      return;
label_15:
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
      goto label_7;
label_24:
      this.MainElement = element1.SubElements[0];
    }
  }

  public void Draw(Canvas cv) => this.MainElement.Draw(cv);

  public void DrawWafer(Canvas cv)
  {
    int num = 152400;
    Ellipse ellipse = new Ellipse();
    ellipse.Width = (double) num;
    ellipse.Height = (double) num;
    ellipse.Stroke = (Brush) new BrushConverter().ConvertFrom((object) "#232323");
    ellipse.StrokeThickness = 0.0;
    ellipse.Fill = (Brush) new BrushConverter().ConvertFrom((object) "#f8f5f2");
    Ellipse element = ellipse;
    cv.Children.Add((UIElement) element);
    Panel.SetZIndex((UIElement) element, -2);
    element.SetValue(Canvas.LeftProperty, (object) ((double) -num / 2.0));
    element.SetValue(Canvas.TopProperty, (object) ((double) -num / 2.0));
  }

  public void DrawGrid(Canvas cv)
  {
    int num = 10000000;
    for (int index = 0; index < 16; ++index)
    {
      Vector2 vector1 = new Vector2((float) (-15 * num / 2 + num * index), 8E+07f);
      Vector2 vector2 = new Vector2((float) (-15 * num / 2 + num * index), -8E+07f);
      Line line1 = new Line();
      line1.X1 = (double) aux.Scale(vector1, cv).X;
      line1.Y1 = (double) aux.Scale(vector1, cv).Y;
      line1.X2 = (double) aux.Scale(vector2, cv).X;
      line1.Y2 = (double) aux.Scale(vector2, cv).Y;
      line1.StrokeThickness = 400.0;
      line1.Stroke = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFF");
      Line element1 = line1;
      cv.Children.Add((UIElement) element1);
      Panel.SetZIndex((UIElement) element1, -1);
      Vector2 vector3 = new Vector2(-8E+07f, (float) (15 * num / 2 - num * index));
      Vector2 vector4 = new Vector2(8E+07f, (float) (15 * num / 2 - num * index));
      Line line2 = new Line();
      line2.X1 = (double) aux.Scale(vector3, cv).X;
      line2.Y1 = (double) aux.Scale(vector3, cv).Y;
      line2.X2 = (double) aux.Scale(vector4, cv).X;
      line2.Y2 = (double) aux.Scale(vector4, cv).Y;
      line2.StrokeThickness = 400.0;
      line2.Stroke = (Brush) new BrushConverter().ConvertFrom((object) "#FFFFFF");
      Line element2 = line2;
      cv.Children.Add((UIElement) element2);
      Panel.SetZIndex((UIElement) element2, -1);
    }
  }
}
