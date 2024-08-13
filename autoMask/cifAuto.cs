using System.Collections;
using System.Data;
using System.IO;
using System.Numerics;

namespace autoMask
{
  internal static class CifAuto
  {
    public static void Electrodes(Element el, Chip ma)
    {
      int num1 = (ma.ELECTRODE_N - 1) * ma.ELECTRODE_DISTANCE;
      int num2 = (ma.ELECTRODE_N - 1) * ma.ELECTRODE_DISTANCE;
      for (int index1 = 0; index1 < ma.ELECTRODE_N; ++index1)
      {
        for (int index2 = 0; index2 < ma.ELECTRODE_N; ++index2)
        {
          Circle circle = new Circle(new Vector2((float) (-num1 / 2 + index2 * ma.ELECTRODE_DISTANCE), (float) (num2 / 2 - index1 * ma.ELECTRODE_DISTANCE)), ma.ELECTRODE_DIAMETER);
          el.Circles.Add(circle);
        }
      }
    }

    public static void Pads2sides(Element el, Chip ma)
    {
      int num = Chip.PADS_HEIGHT / (ma.N_PADS_SIDE - 1 + 2);
      for (int index = 0; index < ma.N_PADS_SIDE; ++index)
      {
        Box box = new Box(new Vector2((float) (-Chip.PADS_BASE / 2), (float) (Chip.PADS_HEIGHT / 2 - num * (index + 1))), ma.PAD_L, ma.PAD_L);
        el.Boxes.Add(box);
      }
      for (int index = 0; index < ma.N_PADS_SIDE; ++index)
      {
        Box box = new Box(new Vector2((float) (Chip.PADS_BASE / 2), (float) (Chip.PADS_HEIGHT / 2 - num * (index + 1))), ma.PAD_L, ma.PAD_L);
        el.Boxes.Add(box);
      }
    }

    public static void VariableWires(Element el, Chip ma, bool eqWire)
    {
      List<Circle> circleList = new List<Circle>();
      int num1 = ma.N_PADS_SIDE / 2;
      for (int index = 0; index < ma.N_PADS_SIDE; ++index)
        circleList.Add(el.Circles[index * ma.N_PADS_SIDE]);
      int num2 = 10;
      double num3 = double.MaxValue;
      double num4 = double.MinValue;
      Element element1 = new Element();
      for (int index1 = 1; index1 < num2; ++index1)
      {
        Element element2 = new Element();
        int wireMinWidth = ma.WIRE_MIN_WIDTH;
        int num5 = ma.WIRE_MIN_WIDTH * index1;
        int num6 = Chip.PADS_HEIGHT / (ma.N_PADS_SIDE - 1) / 2;
        if (num6 > ma.PAD_L)
          num6 = ma.PAD_L;
        int num7 = num5 * 2;
        for (int index2 = 0; index2 < num1; ++index2)
        {
          Wire wire = new Wire()
          {
            Points = {
              circleList[index2].Center
            },
            Widths = {
              wireMinWidth
            }
          };
          wire.Points.Add(new Vector2(circleList[index2].Center.X - (float) (num7 * index2), circleList[index2].Center.Y));
          wire.Widths.Add(num5);
          wire.Points.Add(new Vector2(circleList[index2].Center.X - (float) (num7 * index2), el.Boxes[index2].Center.Y));
          wire.Widths.Add(num5);
          wire.Points.Add(new Vector2(circleList[index2].Center.X - (float) (num7 * index2) - (float) (num6 / 2), el.Boxes[index2].Center.Y));
          wire.Widths.Add(num6);
          wire.Points.Add(el.Boxes[index2].Center);
          element2.Wires.Add(wire);
        }
        double num8 = 0.0;
        foreach (Wire wire in element2.Wires)
        {
          num8 += wire.Resistance;
        }
        if (num8 < num3)
        {
          num3 = num8;
          element1 = element2;
        }
      }
      for (int index = 0; index < element1.Wires.Count<Wire>(); ++index)
      {
        double resistance = element1.Wires[index].Resistance;
        if (resistance > num4)
          num4 = resistance;
      }
      int[][] numArray1 = new int[4][]
      {
        new int[2]{ 1, 1 },
        new int[2]{ -1, 1 },
        new int[2]{ 1, -1 },
        new int[2]{ -1, -1 }
      };
      foreach (int[] numArray2 in numArray1)
      {
        for (int index3 = 0; index3 < element1.Wires.Count; ++index3)
        {
          Wire wire = new Wire();
          for (int index4 = 0; index4 < element1.Wires[index3].Points.Count; ++index4)
          {
            Vector2 vector2 = new Vector2((float) numArray2[0] * element1.Wires[index3].Points[index4].X, (float) numArray2[1] * element1.Wires[index3].Points[index4].Y);
            wire.Points.Add(vector2);
            if (index4 != element1.Wires[index3].Points.Count - 1)
            {
              int width = element1.Wires[index3].Widths[index4];
              wire.Widths.Add(width);
            }
          }
          if (eqWire)
          {
            wire.Widths[3] = 0;
            if(wire.Lengths == null)
              throw new Exception("Wire lengths were not calculated correctly by UpdateLenghts()");
            double resistance = wire.Resistance;
            double num9 = num4 - resistance;
            double num10 = (double) wire.Lengths[3] * Math.Pow(10.0, -9.0) / ((double) MainWindow.sputteringMaterial * (double) MainWindow.sputteringHeight * Math.Pow(10.0, -9.0) * num9);
            wire.Widths[3] = (int) (num10 * Math.Pow(10.0, 9.0));
          }
          el.Wires.Add(wire);
        }
      }
    }

    public static void Wires(Element el, Chip ma)
    {
      List<Circle> circleList = new List<Circle>();
      int num1 = ma.N_PADS_SIDE / 2;
      for (int index = 0; index < ma.N_PADS_SIDE; ++index)
        circleList.Add(el.Circles[index * ma.N_PADS_SIDE]);
      Vector2 vector2_1 = new Vector2(el.Boxes[num1 - 1].Center.X / 2f, el.Boxes[num1 - 1].Center.Y);
      Vector2 vector2_2 = new Vector2(circleList[0].Center.X, el.Boxes[0].Center.Y);
      double num2 = ((double) vector2_2.Y - (double) vector2_1.Y) / ((double) vector2_2.X - (double) vector2_1.X);
      double num3 = -(double) vector2_1.Y / num2 + (double) vector2_1.X;
      vector2_1 = new Vector2(el.Boxes[num1 - 1].Center.X / 2f, circleList[num1 - 1].Center.Y);
      Vector2 center = circleList[0].Center;
      double num4 = ((double) center.Y - (double) vector2_1.Y) / ((double) center.X - (double) vector2_1.X);
      double num5 = (double) vector2_1.Y - num4 * (double) vector2_1.X;
      for (int index = 0; index < num1; ++index)
      {
        Wire wire1 = new Wire();
        wire1.Points.Add(el.Boxes[index].Center);
        wire1.Widths.Add(ma.WIRE_MIN_WIDTH);
        float x = (float) ((double) el.Boxes[index].Center.Y / num2 + num3);
        if (index == num1 - 1)
        {
          wire1.Points.Add(new Vector2(x, el.Boxes[index].Center.Y));
          wire1.Widths.Add(ma.WIRE_MIN_WIDTH);
        }
        else
        {
          wire1.Points.Add(new Vector2(x - (float) ma.WIRE_CORNER, el.Boxes[index].Center.Y));
          wire1.Points.Add(new Vector2(x, el.Boxes[index].Center.Y - (float) ma.WIRE_CORNER));
          wire1.Widths.Add(ma.WIRE_MIN_WIDTH);
          wire1.Widths.Add(ma.WIRE_MIN_WIDTH);
        }
        float y = (float) (num4 * (double) x + num5);
        if (index == 0)
        {
          wire1.Points.Add(new Vector2(x, y));
          wire1.Widths.Add(ma.WIRE_MIN_WIDTH);
        }
        else
        {
          wire1.Points.Add(new Vector2(x, y + (float) ma.WIRE_CORNER));
          wire1.Points.Add(new Vector2(x + (float) ma.WIRE_CORNER, y));
          wire1.Widths.Add(ma.WIRE_MIN_WIDTH);
          wire1.Widths.Add(ma.WIRE_MIN_WIDTH);
        }
        wire1.Points.Add(circleList[index].Center);
        wire1.Widths.Add(ma.WIRE_MIN_WIDTH);
        el.Wires.Add(wire1);
        Wire wire2 = new Wire();
        foreach (Vector2 point in wire1.Points)
        {
          wire2.Points.Add(new Vector2(point.X, -point.Y));
          wire2.Widths.Add(ma.WIRE_MIN_WIDTH);
        }
        el.Wires.Add(wire2);
        Wire wire3 = new Wire();
        foreach (Vector2 point in wire1.Points)
        {
          wire3.Points.Add(new Vector2(-point.X, point.Y));
          wire3.Widths.Add(ma.WIRE_MIN_WIDTH);
        }
        el.Wires.Add(wire3);
        Wire wire4 = new Wire();
        foreach (Vector2 point in wire1.Points)
        {
          wire4.Points.Add(new Vector2(-point.X, -point.Y));
          wire4.Widths.Add(ma.WIRE_MIN_WIDTH);
        }
        el.Wires.Add(wire4);
      }
    }

    public static void Squares(Element el, Chip ma)
    {
      el.Wires.Add(new Wire()
      {
        Widths = new List<int>()
        {
          ma.SQUARE_WIDTH,
          ma.SQUARE_WIDTH
        },
        Points = {
          new Vector2((float) (-Chip.SQUARE_BASE / 2), (float) (Chip.SQUARE_HEIGHT_A - ma.SQUARE_L)),
          new Vector2((float) (-Chip.SQUARE_BASE / 2), (float) Chip.SQUARE_HEIGHT_A),
          new Vector2((float) (-Chip.SQUARE_BASE / 2 + ma.SQUARE_L), (float) Chip.SQUARE_HEIGHT_A)
        }
      });
      el.Wires.Add(new Wire()
      {
        Widths = new List<int>()
        {
          ma.SQUARE_WIDTH,
          ma.SQUARE_WIDTH
        },
        Points = {
          new Vector2((float) (Chip.SQUARE_BASE / 2), (float) (Chip.SQUARE_HEIGHT_A - ma.SQUARE_L)),
          new Vector2((float) (Chip.SQUARE_BASE / 2), (float) Chip.SQUARE_HEIGHT_A),
          new Vector2((float) (Chip.SQUARE_BASE / 2 - ma.SQUARE_L), (float) Chip.SQUARE_HEIGHT_A)
        }
      });
      el.Wires.Add(new Wire()
      {
        Widths = new List<int>()
        {
          ma.SQUARE_WIDTH,
          ma.SQUARE_WIDTH
        },
        Points = {
          new Vector2((float) (-Chip.SQUARE_BASE / 2), (float) (-Chip.SQUARE_HEIGHT_B + ma.SQUARE_L)),
          new Vector2((float) (-Chip.SQUARE_BASE / 2), (float) -Chip.SQUARE_HEIGHT_B),
          new Vector2((float) (-Chip.SQUARE_BASE / 2 + ma.SQUARE_L), (float) -Chip.SQUARE_HEIGHT_B)
        }
      });
      el.Wires.Add(new Wire()
      {
        Widths = new List<int>()
        {
          ma.SQUARE_WIDTH,
          ma.SQUARE_WIDTH
        },
        Points = {
          new Vector2((float) (Chip.SQUARE_BASE / 2), (float) (-Chip.SQUARE_HEIGHT_B + ma.SQUARE_L)),
          new Vector2((float) (Chip.SQUARE_BASE / 2), (float) -Chip.SQUARE_HEIGHT_B),
          new Vector2((float) (Chip.SQUARE_BASE / 2 - ma.SQUARE_L), (float) -Chip.SQUARE_HEIGHT_B)
        }
      });
    }

    public static void ElementPlace(CifFile ci, List<Element> els, Chip ma)
    {
      int num = -(ma.CHIP_WIDTH * els.Count<Element>()) / 2 + ma.CHIP_WIDTH / 2;
      for (int index = 0; index < els.Count<Element>(); ++index)
      {
        els[index].Position = new Vector2((float) (num + index * ma.CHIP_WIDTH), 0.0f);
        ci.MainElement.SubElements.Add(els[index]);
      }
    }

    public static void GroundReference(Element el, Chip ma)
    {
      int num1 = 4 * el.Wires[0].Widths[1];
      int num2 = ma.ELECTRODE_N * ma.ELECTRODE_DISTANCE;
      int num3 = (num2 - 5 * num1) / 2;
      if (num3 > ma.PAD_L)
        num3 = ma.PAD_L;
      int padL = ma.PAD_L;
      List<int> w = new List<int>() { padL, num3, num3 };
      int b = num2 - 4 * num1;
      int num4 = Chip.PADS_HEIGHT / (ma.N_PADS_SIDE - 1);
      Vector2 center1 = new Vector2((float) (-Chip.PADS_BASE / 2), (float) (-Chip.PADS_HEIGHT / 2));
      Vector2 center2 = new Vector2((float) (-Chip.PADS_BASE / 2), (float) (Chip.PADS_HEIGHT / 2));
      Vector2 center3 = new Vector2((float) (Chip.PADS_BASE / 2), (float) (-Chip.PADS_HEIGHT / 2));
      Vector2 center4 = new Vector2((float) (Chip.PADS_BASE / 2), (float) (Chip.PADS_HEIGHT / 2));
      Box box1 = new Box(center1, ma.PAD_L, ma.PAD_L);
      Box box2 = new Box(center2, ma.PAD_L, ma.PAD_L);
      Box box3 = new Box(center3, ma.PAD_L, ma.PAD_L);
      Box box4 = new Box(center4, ma.PAD_L, ma.PAD_L);
      el.Boxes.Add(box1);
      el.Boxes.Add(box2);
      el.Boxes.Add(box3);
      el.Boxes.Add(box4);
      Vector2 center5 = new Vector2(0.0f, (float) (-ma.REF_SEPARATION / 2 + ma.REF_H / 2));
      Vector2 center6 = new Vector2(0.0f, (float) (ma.REF_SEPARATION / 2 + ma.REF_H / 2));
      Box box5 = new Box(center5, b, ma.REF_H);
      Box box6 = new Box(center6, b, ma.REF_H);
      el.Boxes.Add(box5);
      el.Boxes.Add(box6);
      Vector2 vector2_1 = center1;
      Vector2 vector2_2 = center2;
      Vector2 vector2_3 = center3;
      Vector2 vector2_4 = center4;
      int x = b / 2 - num3 / 2;
      Vector2 vector2_5 = new Vector2((float) (-x - padL / 2), center1.Y);
      Vector2 vector2_6 = new Vector2((float) (x + padL / 2), center3.Y);
      Vector2 vector2_7 = new Vector2((float) (-x - padL / 2), center2.Y);
      Vector2 vector2_8 = new Vector2((float) (x + padL / 2), center4.Y);
      Vector2 vector2_9 = new Vector2((float) -x, center1.Y);
      Vector2 vector2_10 = new Vector2((float) x, center3.Y);
      Vector2 vector2_11 = new Vector2((float) -x, center2.Y);
      Vector2 vector2_12 = new Vector2((float) x, center4.Y);
      Vector2 vector2_13 = new Vector2((float) -x, center5.Y);
      Vector2 vector2_14 = new Vector2((float) x, center5.Y);
      Vector2 vector2_15 = new Vector2((float) -x, center6.Y);
      Vector2 vector2_16 = new Vector2((float) x, center6.Y);
      List<Vector2> points1 = new List<Vector2>()
      {
        vector2_1,
        vector2_5,
        vector2_9,
        vector2_13
      };
      List<Vector2> points2 = new List<Vector2>()
      {
        vector2_2,
        vector2_7,
        vector2_11,
        vector2_15
      };
      List<Vector2> points3 = new List<Vector2>()
      {
        vector2_3,
        vector2_6,
        vector2_10,
        vector2_14
      };
      List<Vector2> points4 = new List<Vector2>();
      points4.Add(vector2_4);
      points4.Add(vector2_8);
      points4.Add(vector2_12);
      points4.Add(vector2_16);
      Wire wire1 = new Wire(points2, w);
      Wire wire2 = new Wire(points1, w);
      Wire wire3 = new Wire(points3, w);
      Wire wire4 = new Wire(points4, w);
      el.Wires.Add(wire2);
      el.Wires.Add(wire1);
      el.Wires.Add(wire3);
      el.Wires.Add(wire4);
    }

    public static ICollection WireReport(Element el, string savePath)
    {
      DataTable dataTable = new DataTable();
      dataTable.Columns.Add(new DataColumn("Index", typeof (string)));
      dataTable.Columns.Add(new DataColumn("LS1 um", typeof (string)));
      dataTable.Columns.Add(new DataColumn("WS1 nm", typeof (string)));
      dataTable.Columns.Add(new DataColumn("LS2 um", typeof (string)));
      dataTable.Columns.Add(new DataColumn("WS2 nm", typeof (string)));
      dataTable.Columns.Add(new DataColumn("LS3 um", typeof (string)));
      dataTable.Columns.Add(new DataColumn("WS3 nm", typeof (string)));
      dataTable.Columns.Add(new DataColumn("LS4 um", typeof (string)));
      dataTable.Columns.Add(new DataColumn("WS4 nm", typeof (string)));
      dataTable.Columns.Add(new DataColumn("LT  um", typeof (string)));
      dataTable.Columns.Add(new DataColumn("RT ohm", typeof (string)));

      // .csv column headers
      string fileContents = "Index;LS1;WS1;LS2;WS2;LS3;WS3;LS4;WS4;LT;RT\n";
      fileContents += ";um;nm;um;nm;um;nm;um;nm;um;ohm;\n"

      // Find the wire with the longest number of segments
      int maxSegments = el.Wires[0].Lengths.Count;
      for(int indexWire = 1; indexWire < el.Wires.Count; indexWire++){
        if(el.Wires[indexWire].Lengths.Count > maxSegments){
          maxSegments = el.Wires[indexWire].Lengths.Count:
        }
      }

      for(int indexWire = 0; indexWire < el.Wires.Count; indexWire++)
      {
          if(el.Wires[indexWire].Lengths != null)
          {
              DataRow dataRow = dataTable.NewRow();
              
              // Wire indexWire
              dataRow[0] = (object)indexWire.ToString();
              fileContents += indexWire.ToString() + ";";

              // Wire segments
              int indexColumn = 1;
              for(int indexParameter = 0; indexParameter < maxSegments){
                if(indexParameter < el.Wires[indexWire].Lengths.Count){
                  // Fetch segment length and width
                  int length = el.Wires[indexWire].Lengths[indexParameter] / 1000;
                  int width = el.Wires[indexWire].Widths[indexParameter];
                  // Write columns for the tab control
                  dataRow[indexColumn] = length.ToString();
                  dataRow[indexColumn+1] = width.ToString();
                  // Write columns for the wire report
                  fileContents += length.ToString() + ";" + width.ToString() + ";";
                  indexParameter++;
                }
                else{
                  fileContents += ";;";
                }
                columnParameter += 2;
              }

              // Wire total length and resistance
              int totalLength = el.Wires[indexWire].Length / 1000;
              int totalResistance = (int)el.Wires[indexWire].Resistance;
              dataRow[indexColumn] = totalLength.ToString();
              dataRow[indexColumn+1] = totalResistance.ToString();
              fileContents += totalLength.ToString() + ";" + totalResistance.ToString() + ";n";

              dataTable.Rows.Add(dataRow);
          }
      }
      using (StreamWriter streamWriter = new StreamWriter(savePath))
        ((TextWriter) streamWriter).Write(str1);
      return (ICollection) new DataView(dataTable);
    }
  }
}
