// Decompiled with JetBrains decompiler
// Type: aux
// Assembly: autoMask, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 93C42831-928E-4BBC-8F9F-03F21363630D
// Assembly location: D:\TFM\automask\autoMask.dll

using System.Numerics;
using System.Windows.Controls;

#nullable enable
public static class aux
{
  public const float SCALE_FACTOR = 1000f;
  public const int FILE_PRECISSION = 1000;

  public static int um2nm(double um)
  {
    int num = (int) um * 1000;
    return num - num % 1000;
  }

  public static int round(int x) => x % 1000 < 500 ? x - x % 1000 : x + 1000 - x % 1000;

  public static Vector2 Scale(Vector2 vector, Canvas cv)
  {
    Vector2 vector2 = Vector2.Divide(vector, 1000f);
    vector2.Y = -vector2.Y;
    return vector2;
  }

  public static float Scale(int value)
  {
    return value <= 10000 ? (float) ((double) value / 1000.0 * 8.0) : (float) value / 1000f;
  }

  public static double Scale(double value) => value / 1000.0;
}
