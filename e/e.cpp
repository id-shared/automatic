#include "Capture.hpp"
#include "Contact.hpp"
#include "Device.hpp"
#include "Event.hpp"
#include "Parallel.hpp"
#include "Pattern.hpp"
#include "Time.hpp"
#include "Xyloid1.hpp"
#include "Xyloid2.hpp"

const int _ = -1 + 1;

static bool repeat(UINT n, const std::function<bool(UINT)>& func) {
  for (UINT i = _; i < n; ++i) {
    if (!func(i)) return false;
  }

  return true;
}

static int to_integer(double e) {
  return static_cast<int>(round(e));
}

static bool track(HANDLE x, double e_2, double e_1, double e) {
  return Xyloid2::yx(x, to_integer(e_2), to_integer(e_1)) && Time::XO(+4);
}

static bool shift(HANDLE x, double e_2, double e_1, double e) {
  /***/if (e_1 >= +1) {
    Xyloid2::yx(x, to_integer(e_2), to_integer(+1)) && Time::XO(e);

    return shift(x, _, e_1 - 1., e);
  }
  else if (e_1 <= -1) {
    Xyloid2::yx(x, to_integer(e_2), to_integer(-1)) && Time::XO(e);

    return shift(x, _, e_1 + 1., e);
  }
  else {
    return true;
  }
}

static bool move_y(HANDLE x, double e_1, int e, bool a) {
  const int y_ = (a ? +1 : -1) * Pattern::dy(e);
  const int n_ = to_integer(e_1 / +4.);

  const int _y = abs(y_);

  std::function<bool(UINT)> fy = [&](UINT e) {
    return Xyloid2::yx(x, y_ > _ ? +1 : -1, _) && Time::XO(e_1 / n_ / _y);
    };

  std::function<bool(UINT)> f1 = [&](UINT e) {
    return repeat(_y, fy);
    };

  return abs(y_) > _ ? repeat(n_, f1) : Time::XO(e_1);
}

static bool halt(HANDLE x, USHORT e_1, UINT e) {
  const int n_ = Pattern::dn(e);

  return n_ > _ && Xyloid1::hold(x, e_1, n_);
}

static bool is_red(uint8_t* x) {
  return x[+0] <= +63 && x[+1] <= +63 && x[+2] >= (+255 - +4) && x[+3] == +255;
}

static int upon(std::function<bool(int)> z, int i) {
  return z(i) ? upon(z, i - 1) : i;
}

static int till(std::function<bool(int)> z, int i) {
  return z(i) ? till(z, i + 1) : i;
}

class Axis {
public:
  bool is;
  int y;
  int x;
  Axis(int e_1, int e, bool a) : y(e_1), x(e), is(a) {}
};

static Axis detect(uint8_t* o1, UINT e_4, UINT e_3, UINT e_2, UINT e_1, UINT e) {
  const int ny = e_2 / +2;
  const int nx = e_1 / +2;

  for (UINT e_y = _; e_y < e_2; ++e_y) {
    uint8_t* px_y = o1 + ((e_4 - ny) + e_y) * e;

    for (UINT e_x = _; e_x < e_1; ++e_x) {
      uint8_t* px_x = px_y + ((e_3 - nx) + e_x) * +4;

      /***/if (is_red(px_x)) {
        return Axis(
          static_cast<int>(e_y) - ny,
          static_cast<int>(e_x) - nx,
          true
        );
      }
    }
  }

  return Axis(_, _, false);
}

int main() {
  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL"sv) && c.ends_with(L"{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  constexpr double XY = +1 / +0.566;

  constexpr UINT AKR = 0x4d;
  constexpr UINT AKL = 0x4b;
  constexpr UINT AKC = 0x1d;

  constexpr UINT KX = 0x58;
  constexpr UINT KQ = 0x51;
  constexpr UINT KD = 0x44;
  constexpr UINT KC = 0x43;
  constexpr UINT KA = 0x41;

  constexpr UINT FZ = +4;
  constexpr UINT FR = +4;
  constexpr UINT FA = +4;

  bool _Y = false;
  bool _X = false;
  bool _R = false;
  bool _N = false;
  bool _L = false;
  bool _D = false;
  bool _C = false;
  bool _A = false;

  int _e = _;

  std::function<void()> z2 = [&_e, &_A, &_D, &_L, &_R, &_X, &_Y, &driver]() mutable {
    Parallel::Pool _Z1K(+1000);

    Parallel::Pool xs(+1);

    const int ey = GetSystemMetrics(SM_CYSCREEN);
    const int ex = GetSystemMetrics(SM_CXSCREEN);

    const int cy = ey / +16;
    const int cx = ex / +4;

    const int ay = cy / +2;
    const int ax = cx / +2;

    const double _x = static_cast<double>(FZ * FR * FA) / static_cast<double>(ax);
    const double _a = ay / +16.;

    std::function<bool(double, double, double, bool)> work = [&_X, &_Y, &driver](double e_2, double e_1, double e, bool a) mutable {
      _Y ? track(driver, _, e_1, e) : shift(driver, e_2, e_1, e);

      /***/if (a) {
        _Y = a;
        _X = a;

        return true;
      }
      else {
        return true;
      }
      };

    std::function<bool(double, double, double, bool)> task = [&_a, &_x, &_L, &_R, &ex, &ey, &work](double e_2, double e_1, double e, bool a) mutable {
      const double y_ = (e_2 + _a) * e;
      const double x_ = (e_1 + _a) * e;

      /***/if (_R) {
        return work(y_, x_, _x, a);
      }
      else if (_L) {
        return work(y_, x_, _x, a);
      }
      else {
        return true;
      }
      };

    std::function<bool(uint8_t*, UINT, UINT, UINT, bool)> find = [&ax, &ay, &task](uint8_t* o1, UINT e_2, UINT e_1, UINT e, bool a) mutable {
      Axis axis = detect(o1, ay, ax, e_2, e_1, e);

      /***/if (axis.is) {
        return task(axis.y, axis.x, XY, a);
      }
      else {
        return false;
      }
      };

    std::function<bool(uint8_t*, UINT, UINT, UINT)> each = [&_e, &_X, &_Y, &_Z1K, &find, &driver](uint8_t* o1, UINT e_2, UINT e_1, UINT e) mutable {
      int id = _e;
      _e = _e + 1;

      _Z1K.enqueue_task([&find, &driver, id, o1, e_2, e_1, e]() mutable {
        /***/if (id == _ || id % FZ == _) {
          /***/if (find(o1, e_2, e_1 / +4, e, true)) {
            return true;
          }
          else if (find(o1, e_2, e_1, e, false)) {
            return true;
          }
          else {
            return true;
          }
        }
        else {
          return false;
        }
        });

      return true;
      };

    Capture::screen(each, (ey - cy) / +2, (ex - cx) / +2, cy, cx, static_cast<UINT>(FR));
    };
  std::thread thread2(z2);


  std::function<void()> z1 = [&_e, &_A, &_D, &_L, &_R, &_C, &_X, &_Y, &driver]() mutable {
    ULONGLONG _Z64 = GetTickCount64();

    Parallel::Pool xy(+1);
    Parallel::Pool xx(+1);
    Parallel::Pool xr(+1);
    Parallel::Pool xl(+1);
    Parallel::Pool xq(+1);
    Parallel::Pool xd(+1);
    Parallel::Pool xc(+1);
    Parallel::Pool xa(+1);

    const int size = +49;

    int init = +1;

    Event::KeyboardHook hook([&_e, &_A, &_D, &_L, &_R, &_C, &_X, &_Y, &_Z64, &xa, &xc, &xd, &xq, &xl, &xr, &xx, &xy, &init, &driver](UINT e, bool a) mutable {
      /***/if (e == VK_OEM_6) {
        if (a) {
          _R = a;
          _e = _;

          xr.enqueue_task([&_R, &driver]() mutable {
            _R ? Xyloid2::e2(driver, _R) : _;
            });

          return false;
        }
        else {
          _R = a;

          xr.enqueue_task([&_R, &driver]() mutable {
            _R ? _ : Xyloid2::e2(driver, _R);
            });

          return false;
        }
      }
      else if (e == VK_OEM_4) {
        if (a) {
          _Y = false;
          _X = false;
          _L = a;
          _e = _;

          xl.enqueue_task([&_A, &_D, &_L, &_C, &_X, &init, &driver]() mutable {
            while (_A || _D) {
              Time::XO(+1);
            }

            UINT e_ = _;
            while (_L && !_X && e_ < (FZ * FR * FA)) {
              e_ = e_ + +1;
              Time::XO(+1);
            }

            _L ? Xyloid2::e1(driver, true) : _;

            init = till([&_L, &driver](int e) {
              const bool back = _L && (size >= e);

              if (back) {
                move_y(driver, +16, e, true);

                return back;
              }
              else {
                return back;
              }
              }, init) - 1;
            });

          return false;
        }
        else {
          _L = a;

          xl.enqueue_task([&_L, &init, &driver]() mutable {
            _L ? _ : Xyloid2::e1(driver, false);

            init = upon([&_L, &driver](int e) {
              const bool back = !_L && (+1 <= e);

              if (back) {
                move_y(driver, +16, e, false);

                return back;
              }
              else {
                return back;
              }
              }, init) + 1;
            });

          return false;
        }
      }
      else if (e == KC) {
        if (a) {
          xc.enqueue_task([&driver]() mutable {
            Time::XO(+800);

            Xyloid2::e2(driver, true);
            Time::XO(+64);
            Xyloid2::e2(driver, false);
            });

          return true;
        }
        else {
          return true;
        }
      }
      else if (e == KD) {
        if (a) {
          _Z64 = GetTickCount64();
          _D = a;

          return true;
        }
        else {
          UINT diff = static_cast<unsigned int>(GetTickCount64() - _Z64);

          xd.enqueue_task([&_D, &diff, &driver, a]() mutable {
            halt(driver, AKL, diff / +10);
            _D = a;
            });

          return true;
        }
      }
      else if (e == KA) {
        if (a) {
          _Z64 = GetTickCount64();
          _A = a;

          return true;
        }
        else {
          UINT diff = static_cast<unsigned int>(GetTickCount64() - _Z64);

          xa.enqueue_task([&_A, &diff, &driver, a]() mutable {
            halt(driver, AKR, diff / +10);
            _A = a;
            });

          return true;
        }
      }
      else {
        return true;
      }
      });
    };
  std::thread thread1(z1);

  thread2.join();
  thread1.join();

  return +1;
}

//xs.enqueue_task([&_L, _S, &driver]() mutable {
//  Xyloid1::ea(driver, AKC, true);
//
//  UINT e_ = _;
//  while (_L && e_ < +160) {
//    e_ = e_ + +1;
//    Time::XO(+1);
//  }
//
//  Xyloid1::ea(driver, AKC, false);
//  });

/*const int tick1 = GetTickCount();
int tick2 = GetTickCount();
std::cout << (tick2 - tick1) << std::endl;*/
