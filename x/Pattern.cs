﻿class Pattern {
  public int IS;

  public int DY(int n) {
    return n switch {
      55 => 1,
      54 => 1,
      53 => 1,
      52 => 1,

      48 => 1,
      47 => 1,
      46 => 1,
      45 => 1,

      41 => 2,
      40 => 2,
      39 => 2,
      38 => 2,
      37 => 2,
      36 => 1,
      35 => 1,

      34 => 2,
      33 => 2,
      32 => 2,
      31 => 2,
      30 => 2,
      29 => 1,
      28 => 1,

      27 => 2,
      26 => 2,
      25 => 2,
      24 => 2,
      23 => 2,
      22 => 1,
      21 => 1,

      20 => 2,
      19 => 2,
      18 => 2,
      17 => 2,
      16 => 2,
      15 => 1,
      14 => 1,

      13 => 1,
      12 => 1,
      11 => 1,
      10 => 1,

      6 => 1,
      5 => 1,
      4 => 1,
      3 => 1,

      _ => 0,
    };
  }

  public int DX(int n) {
    return n switch {
      55 => 1,
      53 => 1,

      48 => 1,
      46 => 1,

      41 => 1,
      40 => 1,
      39 => 1,
      38 => 1,
      37 => 1,
      36 => 1,

      34 => 1,
      33 => 1,
      32 => 1,
      31 => 1,
      30 => 1,
      29 => 1,

      27 => 1,
      26 => 1,
      25 => 1,
      24 => 1,
      23 => 1,
      22 => 1,

      20 => 1,
      19 => 1,
      18 => 1,
      17 => 1,
      16 => 1,
      15 => 1,

      13 => 1,
      11 => 1,

      6 => 1,
      4 => 1,

      _ => 0,
    };
  }

  public int DN(int n) {
    return n switch {
      32 => 96,
      31 => 94,
      30 => 92,
      29 => 90,
      28 => 88,
      27 => 86,
      26 => 84,
      25 => 82,
      24 => 80,
      23 => 78,
      22 => 76,
      21 => 74,
      20 => 72,
      19 => 70,
      18 => 68,
      17 => 66,
      16 => 64,
      15 => 60,
      14 => 56,
      13 => 52,
      12 => 48,
      11 => 44,
      10 => 40,
      9 => 36,
      8 => 32,
      7 => 28,
      6 => 24,
      5 => 0,
      4 => 0,
      3 => 0,
      2 => 0,
      1 => 0,
      0 => 0,
      _ => 96,
    };
  }

  public Pattern(int c) {
    IS = c;
  }
}
