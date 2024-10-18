﻿class Recoil {
  public static readonly Random ZN = new();
  public static volatile bool YS;
  public static volatile bool XS;

  public Recoil() {
    YS = State();
    XS = State();
  }

  public bool State() {
    return ZN.Next(1, 3) == 1;
  }

  public int YAxis(int n) {
    int o = YS ? 0 : 1;
    int i = YS ? 1 : 0;

    return n switch {
      99 => 1,
      98 => 1,
      97 => 1,
      96 => 1,
      95 => 1,
      94 => 1,
      93 => 1,
      92 => 1,
      91 => 1,
      90 => 1,
      89 => 1,
      88 => 1,
      87 => 1,
      86 => 1,
      85 => 1,
      84 => 1,
      83 => 1,
      82 => 1,
      81 => 1,
      80 => 1,
      79 => 1,
      78 => 1,
      77 => 1,
      76 => 1,
      75 => 1,
      74 => 1,
      73 => 1,
      72 => 1,
      71 => 1,
      70 => 1,
      69 => 1,
      68 => 1,
      67 => 1,
      66 => 1,
      65 => 1,
      64 => 1,
      63 => 1,
      62 => 1,
      61 => 1,
      60 => 1,
      59 => 1,
      58 => 1,
      57 => 1,
      56 => 1,
      55 => 1,
      54 => 1,
      53 => 1,
      52 => 1,
      51 => 1,
      50 => 1,
      49 => 2,
      48 => 1,
      47 => 2,
      46 => 1,
      45 => 2,
      44 => 1,
      43 => 2,
      42 => 1,
      41 => 2,
      40 => 1,
      39 => 2,
      38 => 2,
      37 => 2,
      36 => 2,
      35 => 2,
      34 => 2,
      33 => 2,
      32 => 2,
      31 => 2,
      30 => 2,
      29 => i,
      28 => o,
      27 => i,
      26 => o,
      25 => i,
      24 => o,
      23 => i,
      22 => o,
      21 => i,
      20 => o,
      19 => i,
      18 => o,
      17 => i,
      16 => o,
      15 => i,
      14 => o,
      13 => i,
      12 => o,
      11 => i,
      10 => o,
      9 => i,
      8 => o,
      7 => i,
      6 => o,
      5 => i,
      4 => o,
      3 => i,
      2 => o,
      1 => i,
      0 => o,
      _ => 0
    };
  }

  public static int XAxis(int n) {
    return n switch {
      99 => 2,
      98 => 2,
      97 => 1,
      96 => 1,
      95 => 1,
      94 => 2,
      93 => 2,
      92 => 1,
      91 => 1,
      90 => 1,
      89 => 2,
      88 => 1,
      87 => 1,
      86 => 1,
      85 => 1,
      84 => 2,
      83 => 1,
      82 => 1,
      81 => 1,
      80 => 1,
      79 => 2,
      78 => 1,
      77 => 1,
      76 => 1,
      75 => 1,
      74 => 2,
      73 => 1,
      72 => 1,
      71 => 1,
      70 => 1,
      69 => 2,
      68 => 1,
      67 => 1,
      66 => 1,
      65 => 1,
      64 => 2,
      63 => 1,
      62 => 1,
      61 => 1,
      60 => 1,
      59 => 2,
      58 => 1,
      57 => 1,
      56 => 1,
      55 => 1,
      54 => 2,
      53 => 1,
      52 => 1,
      51 => 1,
      50 => 1,
      49 => 2,
      48 => 1,
      47 => 1,
      46 => 1,
      45 => 1,
      44 => 2,
      43 => 1,
      42 => 1,
      41 => 1,
      40 => 1,
      39 => 2,
      38 => 1,
      37 => 1,
      36 => 1,
      35 => 1,
      34 => 2,
      33 => 1,
      32 => 1,
      31 => 1,
      30 => 1,
      29 => 2,
      28 => 1,
      27 => 1,
      26 => 1,
      25 => 0,
      24 => 2,
      23 => 1,
      22 => 0,
      21 => 0,
      20 => 0,
      19 => 2,
      18 => 1,
      17 => 0,
      16 => 0,
      15 => 0,
      14 => 2,
      13 => 1,
      12 => 0,
      11 => 0,
      10 => 0,
      9 => 0,
      8 => 0,
      7 => 0,
      6 => 0,
      5 => 0,
      4 => 0,
      3 => 0,
      2 => 0,
      1 => 0,
      0 => 0,
      _ => 0
    };
  }
}
