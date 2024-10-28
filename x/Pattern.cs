class Pattern {
  public int IS;

  public int DY(int n) {
    return n switch {
      45 => 4,

      39 => 4,

      33 => 4,
      31 => 4,
      29 => 4,

      27 => 4,
      25 => 4,
      23 => 4,

      210 => 4,
      190 => 4,
      170 => 4,

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
      _ => 0,
    };
  }

  public Pattern(int c) {
    IS = c;
  }
}
