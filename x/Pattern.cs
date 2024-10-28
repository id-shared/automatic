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

      21 => 4,
      19 => 4,
      17 => 4,

      15 => 4,
      13 => 4,
      11 => 4,

      9 => 4,

      3 => 4,

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
