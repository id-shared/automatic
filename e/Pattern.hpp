#pragma once

namespace Pattern {
  int dy(int n) {
    switch (n) {
    case +55:
    case +54:
    case +53:
    case +52:
    case +48:
    case +47:
    case +46:
    case +45:
      return +1;
    case +41:
    case +40:
    case +39:
    case +38:
    case +37:
    case +36:
    case +35:
    case +34:
      return +2;
    case +33:
    case +32:
      return +1;
    case +31:
    case +30:
    case +29:
      return +2;
    case +28:
    case +27:
      return +1;
    case +26:
    case +25:
    case +24:
    case +23:
      return +2;
    case +22:
    case +21:
      return +1;
    case +20:
    case +19:
    case +18:
    case +17:
    case +16:
      return +2;
    case +15:

    case +14:
    case +10:
    case +9:
    case +8:
      return +1;

    case +7:
    case +3:
    case +2:
    case +1:
      return +1;
    default:
      return +0;
    }
  }

  int dx(int n) {
    switch (n) {
    case +55:
    case +53:
    case +48:
    case +46:
    case +41:
    case +40:
    case +39:
    case +38:
    case +37:
    case +36:
    case +34:
    case +33:
    case +32:
    case +31:
    case +30:
    case +29:
    case +27:
    case +26:
    case +25:
    case +24:
    case +23:
    case +22:
    case +20:
    case +19:
    case +18:
    case +17:
    case +16:
    case +15:
    case +13:
    case +11:
    case +6:
    case +4:
      return +1;
    default:
      return +0;
    }
  }

  int dn(int n) {
    switch (n) {
    case +32:
      return +96;
    case +31:
      return +94;
    case +30:
      return +92;
    case +29:
      return +90;
    case +28:
      return +88;
    case +27:
      return +86;
    case +26:
      return +84;
    case +25:
      return +82;
    case +24:
      return +80;
    case +23:
      return +78;
    case +22:
      return +76;
    case +21:
      return +74;
    case +20:
      return +72;
    case +19:
      return +70;
    case +18:
      return +68;
    case +17:
      return +66;
    case +16:
      return +64;
    case +15:
      return +60;
    case +14:
      return +56;
    case +13:
      return +52;
    case +12:
      return +48;
    case +11:
      return +44;
    case +10:
      return +40;
    case +9:
      return +36;
    case +8:
      return +32;
    case +7:
      return +28;
    case +6:
      return +24;
    case +5:
    case +4:
    case +3:
    case +2:
    case +1:
    case +0:
      return +0;
    default:
      return +96;
    }
  }
}
