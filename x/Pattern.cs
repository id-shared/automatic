﻿class Pattern {
  public int IS;

  public int YAxis(int n) {
    return n switch {
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
      49 => 1,
      48 => 1,

      47 => 2,
      46 => 2,
      45 => 1,
      44 => 1,
      43 => 2,
      42 => 2,
      41 => 1,
      40 => 1,
      39 => 2,
      38 => 1,
      37 => 1,
      36 => 1,
      35 => 2,
      34 => 1,
      33 => 1,
      32 => 1,

      31 => 2,
      30 => 1,
      29 => 1,
      28 => 1,
      27 => 1,
      26 => 1,
      25 => 1,
      24 => 1,
      23 => 2,
      22 => 1,
      21 => 1,
      20 => 1,
      19 => 1,
      18 => 1,
      17 => 1,
      16 => 1,

      15 => 1,
      14 => 1,
      13 => 0,
      12 => 0,
      11 => 1,
      10 => 1,
      9 => 0,
      8 => 0,
      7 => 1,
      6 => 1,
      5 => 0,
      4 => 0,
      3 => 1,
      2 => 1,
      1 => 0,
      0 => 0,

      _ => 0
    };
  }

  public int XAxis(int n) {
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
      24 => 0,
      23 => 1,
      22 => 0,
      21 => 0,
      20 => 0,
      19 => 0,
      18 => 1,
      17 => 0,
      16 => 0,
      15 => 0,
      14 => 0,
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

  public Pattern(int c) {
    IS = c;
  }
}
