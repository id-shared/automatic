﻿class Pattern {
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
      //63 => 1,
      //61 => 1,
      //59 => 1,
      //57 => 1,
      //55 => 1,
      //53 => 1,
      //51 => 1,
      //49 => 1,

      //47 => 1,
      //45 => 1,
      //43 => 1,
      //41 => 1,
      //39 => 1,
      //37 => 1,
      //35 => 1,
      //33 => 1,

      //31 => 1,
      //29 => 1,
      //27 => 1,
      //25 => 1,
      //23 => 1,
      //21 => 1,
      //19 => 1,
      //17 => 1,

      //15 => 1,
      //13 => 1,
      //11 => 1,
      //9 => 1,
      //7 => 1,
      //5 => 1,
      //3 => 1,
      //1 => 1,

      _ => 0,
    };
  }

  public Pattern(int c) {
    IS = c;
  }
}
