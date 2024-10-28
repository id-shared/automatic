class Pattern {
  public int IS;

  public int DY(int n) {
    return n switch {
      45 => 2,
      44 => 2,

      39 => 2,
      38 => 2,

      33 => 2,
      32 => 2,
      31 => 2,
      30 => 2,
      29 => 2,
      28 => 2,

      27 => 2,
      26 => 2,
      25 => 2,
      24 => 2,
      23 => 2,
      22 => 2,

      21 => 2,
      20 => 2,
      19 => 2,
      18 => 2,
      17 => 2,
      16 => 2,

      15 => 2,
      14 => 2,
      13 => 2,
      12 => 2,
      11 => 2,
      10 => 2,

      9 => 2,
      6 => 2,

      3 => 2,
      2 => 2,

      _ => 0,
    };
  }

  public int DX(int n) {
    return n switch {
      _ => 0,
    };
  }

  public Pattern(int c) {
    IS = c;
  }
}
