using System.Numerics;
using System.Windows.Controls;

public static class auxFun
{
  public const float SCALE_FACTOR = 150000;
  public const int FILE_PRECISSION = 1000;

  public static int um2nm(double um)
  {
    int num = (int) um * 1000;
    return num - num % 1000;
  }

  public static int round(int x) => x % 1000 < 500 ? x - x % 1000 : x + 1000 - x % 1000;

  public static Vector2 Scale(Vector2 vector, Canvas cv)
  {
    Vector2 vector2 = Vector2.Divide(vector, SCALE_FACTOR);
    vector2.Y = -vector2.Y;
    return vector2;
  }

  public static float Scale(int value)
  {
    return value <= 10000 ? (float) ((double) value / SCALE_FACTOR * 8.0) : (float) value / SCALE_FACTOR;
  }

  public static double Scale(double value) => value / SCALE_FACTOR;
}