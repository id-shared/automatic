class Pattern {
  public int IS;

  public int DY(int n) {
    return n switch {
      45 => 4,
      39 => 4,
      33 => 12,
      27 => 12,
      21 => 12,
      15 => 12,
      9 => 4,
      3 => 4,
      _ => 0,
    };
  }

  public int DX(int n) {
    return n switch {
      45 => 2,
      39 => 2,
      33 => 6,
      27 => 6,
      21 => 6,
      15 => 6,
      9 => 2,
      3 => 2,
      _ => 0,
    };
  }

  public Pattern(int c) {
    IS = c;
  }
}
