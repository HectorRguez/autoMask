namespace autoMask
{
  public class Chip
  {
    // Properties
    // ==============================================================
    public static int MAX_PAD_SIZE = 2000000;
    public static int PADS_BASE = 27000000;
    public static int PADS_HEIGHT = 75000000;
    public static int SQUARE_BASE = 29000000;
    public static int SQUARE_HEIGHT_A = 39740000;
    public static int SQUARE_HEIGHT_B = 42660000;
    public int ELECTRODE_N;
    public int ELECTRODE_DISTANCE;
    public int ELECTRODE_DIAMETER;
    private int pad_l;
    public int WIRE_MIN_WIDTH = 5000;
    public int WIRE_CORNER = 100000;
    public int SQUARE_WIDTH = 100000;
    public int SQUARE_L = 500000;
    public int CHIP_MARGIN;
    public int REF_SEPARATION = 17000000;
    public int N_PADS_SIDE => this.ELECTRODE_N;
    public int PAD_L
    {
      get
      {
        if (this.pad_l != 0)
          return this.pad_l;
        if (this.N_PADS_SIDE == 0)
          return 0;
        int num = Chip.PADS_HEIGHT / (this.N_PADS_SIDE - 1) / 2;
        return num > Chip.MAX_PAD_SIZE ? Chip.MAX_PAD_SIZE : num;
      }
      set => this.pad_l = value;
    }
    public int CHIP_WIDTH => Chip.SQUARE_BASE + this.SQUARE_WIDTH + this.CHIP_MARGIN;
    public int REF_H => this.PAD_L;

    // Constructor
    // ==============================================================
    public Chip(
      int eLECTRODE_N,
      int eLECTRODE_DISTANCE,
      int eLECTRODE_DIAMETER,
      int pad_l,
      int wIRE_MIN_WIDTH,
      int sQUARE_BASE,
      int sQUARE_HEIGHT_A,
      int sQUARE_HEIGHT_B,
      int sQUARE_WIDTH,
      int sQUARE_L,
      int cHIP_MARGIN,
      int rEF_SEPARATION)
    {
      this.ELECTRODE_N = eLECTRODE_N;
      this.ELECTRODE_DISTANCE = eLECTRODE_DISTANCE;
      this.ELECTRODE_DIAMETER = eLECTRODE_DIAMETER;
      this.pad_l = pad_l;
      this.WIRE_MIN_WIDTH = wIRE_MIN_WIDTH;
      Chip.SQUARE_BASE = sQUARE_BASE;
      Chip.SQUARE_HEIGHT_A = sQUARE_HEIGHT_A;
      Chip.SQUARE_HEIGHT_B = sQUARE_HEIGHT_B;
      this.SQUARE_WIDTH = sQUARE_WIDTH;
      this.SQUARE_L = sQUARE_L;
      this.CHIP_MARGIN = cHIP_MARGIN;
      this.REF_SEPARATION = rEF_SEPARATION;
    }

    public Chip(string line)
    {
      string[] strArray = line.Split(';', StringSplitOptions.None);
      this.ELECTRODE_N = int.Parse(strArray[0]);
      this.ELECTRODE_DISTANCE = int.Parse(strArray[1]);
      this.ELECTRODE_DIAMETER = int.Parse(strArray[2]);
      this.PAD_L = int.Parse(strArray[3]);
      this.WIRE_MIN_WIDTH = int.Parse(strArray[4]);
      this.SQUARE_WIDTH = int.Parse(strArray[5]);
      this.SQUARE_L = int.Parse(strArray[6]);
      this.CHIP_MARGIN = int.Parse(strArray[7]);
      this.REF_SEPARATION = int.Parse(strArray[8]);
    }

    public Chip()
    {
    }

    // Public methods
    // ==============================================================
    public string csvLine()
    {
      return this.ELECTRODE_N.ToString() + ";" + this.ELECTRODE_DISTANCE.ToString() + ";" + 
             this.ELECTRODE_DIAMETER.ToString() + ";" + this.PAD_L.ToString() + ";" + 
             this.WIRE_MIN_WIDTH.ToString() + ";" + this.SQUARE_WIDTH.ToString() + ";" + 
             this.SQUARE_L.ToString() + ";" + this.CHIP_MARGIN.ToString() + ";" + 
             this.REF_SEPARATION.ToString() + ";\n";
}
  }
}
