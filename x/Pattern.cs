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
      46 => 1,
      45 => 1,
      44 => 1,
      43 => 2,
      42 => 1,
      41 => 1,
      40 => 1,
      39 => 2,
      38 => 1,
      37 => 1,
      36 => 1,
      35 => 2,
      34 => 2,
      33 => 1,
      32 => 1,
      31 => 2,
      30 => 2,
      29 => 2,
      28 => 2,
      27 => 2,
      26 => 2,
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
      11 => 1,
      7 => 1,
      3 => 1,
      _ => 0,
    };
  }

  public int XAxis(int n) {
    return n switch {
      63 => 1,
      61 => 1,
      59 => 1,
      56 => 1,
      55 => 1,
      53 => 1,
      51 => 1,
      49 => 1,
      47 => 2,
      45 => 1,
      43 => 2,
      41 => 1,
      39 => 2,
      37 => 1,
      35 => 2,
      33 => 1,
      31 => 1,
      29 => 1,
      27 => 1,
      25 => 1,
      23 => 1,
      21 => 1,
      19 => 1,
      17 => 1,
      15 => 1,
      11 => 1,
      7 => 1,
      3 => 1,
      _ => 0,
    };
  }

  public Pattern(int c) {
    IS = c;
  }
}
