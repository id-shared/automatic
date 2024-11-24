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

static int to_integer(double e) {
  return static_cast<int>(round(e));
}

static bool prevent(HANDLE x, USHORT e_1, UINT e) {
  const int n_ = Pattern::dn(e);

  return n_ > _ && Xyloid1::hold(x, e_1, n_);
}

static bool pattern(HANDLE x, int e, bool a) {
  const int y_ = (a ? +1 : -1) * Pattern::dy(e);
  const int x_ = (a ? -1 : +1) * Pattern::dx(e);
  const int n_ = +4;

  if (abs(y_) > _) {
    Xyloid2::yx(x, to_integer(y_ * +1.5), x_);
    Time::XO(n_);
    Xyloid2::yx(x, y_, _);
    Time::XO(n_);
    Xyloid2::yx(x, y_, _);
    Time::XO(n_);
    Xyloid2::yx(x, y_, _);
    Time::XO(n_);
    return true;
  }
  else {
    return true;
  }
}

static bool move(HANDLE x, double e_1, double e) {
  return Xyloid2::yx(x, to_integer(e_1), to_integer(e));
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

int main() {
  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL"sv) && c.ends_with(L"{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  constexpr UINT VZ_R = 0x4d;
  constexpr UINT VZ_L = 0x4b;
  constexpr UINT VK_D = 0x44;
  constexpr UINT VK_A = 0x41;
  constexpr UINT fr = +1;

  ULONGLONG _Z64 = GetTickCount64();
  UINT _Z = _;
  UINT _R = _;
  UINT _L = _;
  UINT _D = _;
  UINT _A = _;

  std::function<void()> action2 = [&_L, &_R, &_Z, &driver]() mutable {
    Parallel::Pool zz(+1000);
    Parallel::Pool zr(+1);
    Parallel::Pool zl(+1);
    Parallel::Pool za(+1);

    const int xy = GetSystemMetrics(SM_CYSCREEN);
    const int xx = GetSystemMetrics(SM_CXSCREEN);

    const int ey = xy / +12;
    const int ex = xx / +6;

    const int cy = ey / +2;
    const int cx = ex / +2;

    const int ay = +2;
    const int ax = +2;

    int _y = _;
    int _x = _;

    std::function<bool(int, int)> work = [&_x, &_y, &_Z, &ax, &ay, &xx, &xy, &driver](int e_1, int e) mutable {
      const bool back = (e == _x && e_1 == _y) || move(driver, e_1 * ay * (_Z > _ ? _ : +1), e * ax);

      _Z = _Z + 1;
      _y = e_1;
      _x = e;

      return back;
      };

    std::function<bool(int, int, int, int)> task = [&_x, &_y, &_L, &_R, &_Z, &za, &zl, &zr, &work](int e_3, int e_2, int e_1, int e) mutable {
      const int y_ = e_3 + e_1;
      const int x_ = e_2 + e;

      /***/if (_R > _) {
        zr.enqueue_task([&x_, &y_, &work]() mutable {
          work(y_, x_);
          });
        return true;
      }
      else if (_L > _) {
        zl.enqueue_task([&x_, &y_, &work]() mutable {
          work(y_, x_);
          });
        return true;
      }
      else {
        _Z = _;
        _y = _;
        _x = _;
        return true;
      }
      };

    std::function<bool(uint8_t*, UINT, UINT, UINT)> find = [&cx, &cy, &xx, &xy, &task](uint8_t* o1, UINT e_2, UINT e_1, UINT e) mutable {
      const int y_ = e_2 / +2;
      const int x_ = e_1 / +2;

      for (UINT e_y = _; e_y < e_2; ++e_y) {
        uint8_t* px_y = o1 + ((cy - y_) + e_y) * e;

        for (UINT e_x = _; e_x < e_1; ++e_x) {
          uint8_t* px_x = px_y + ((cx - x_) + e_x) * +4;

          if (is_red(px_x)) {
            const int axis_y = e_y - y_;
            const int axis_x = e_x - x_;

            return task(axis_y, axis_x, xy / 256, xx / 1024);
          }
        }
      }

      return false;
      };

    std::function<bool(uint8_t*, UINT, UINT, UINT)> each = [&_Z, &zz, &find](uint8_t* o1, UINT e_2, UINT e_1, UINT e) mutable {
      zz.enqueue_task([&_Z, &find, o1, e_2, e_1, e]() mutable {
        /***/if (find(o1, e_2, _Z > _ ? e_1 / +16 : e_1, e)) {
          return true;
        }
        else {
          return true;
        }
        });

      return true;
      };

    Capture::screen(each, (xy - ey) / +2, (xx - ex) / +2, ey, ex, fr);
    };
  std::thread thread2(action2);


  std::function<void()> action1 = [&_A, &_D, &_L, &_R, &_Z, &_Z64, &driver]() mutable {
    Parallel::Pool zy(+1);
    Parallel::Pool zx(+1);
    Parallel::Pool zr(+1);
    Parallel::Pool zl(+1);
    Parallel::Pool zd(+1);
    Parallel::Pool za(+1);

    const int size = +64;
    const int each = +16;
    const bool is = true;
    int at = +1;

    Event::KeyboardHook hook([&_A, &_D, &_L, &_R, &_Z, &_Z64, &at, &driver, &za, &zd, &zl, &zr, &zy](UINT e, bool a) mutable {
      /***/if (e == VK_OEM_6) {
        if (a) {
          _R = _R + 1;
          _Z = _;

          zr.enqueue_task([&_R, &driver]() mutable {
            _R > _ ? Xyloid2::e2(driver, true) : _;
            });

          return false;
        }
        else {
          _R = _;

          zr.enqueue_task([&_R, &driver]() mutable {
            _R > _ ? _ : Xyloid2::e2(driver, false);
            });

          return false;
        }
      }
      else if (e == VK_OEM_4) {
        if (a) {
          _L = _L + 1;
          _Z = _;

          zl.enqueue_task([&_A, &_D, &_L, &_Z, &at, &driver, &zy]() mutable {
            UINT e_ = _;
            while (_L > _ && (_A > _ || _D > _ || _Z < +1) && e_ < +16) {
              Time::XO(fr);
              e_ = e_ + fr;
            }

            _L > _ ? Xyloid2::e1(driver, true) : _;

            is ? at = till([&_L, &driver, &zy](int e) {
              const bool back = _L > _ && (size >= e);

              if (back) {
                zy.enqueue_task([e, &driver]() mutable {
                  pattern(driver, e, true);
                  });

                Time::XO(each);

                return back;
              }
              else {
                return back;
              }
              }, at) - 1 : _;
            });

          return false;
        }
        else {
          _L = _;

          zl.enqueue_task([&_L, &at, &driver, &zy]() mutable {
            _L > _ ? _ : Xyloid2::e1(driver, false);

            is ? at = upon([&_L, &driver, &zy](int e) {
              const bool back = !(_L > _) && (+1 <= e);

              if (back) {
                zy.enqueue_task([&driver, e]() mutable {
                  pattern(driver, e, false);
                  });

                Time::XO(each);

                return back;
              }
              else {
                return back;
              }
              }, at) + 1 : _;
            });

          return false;
        }
      }
      else if (e == VK_D) {
        if (a) {
          _Z64 = GetTickCount64();
          _D = _D + 1;

          return true;
        }
        else {
          UINT diff = static_cast<unsigned int>(GetTickCount64() - _Z64);

          zd.enqueue_task([&_D, &diff, &driver]() mutable {
            prevent(driver, VZ_L, diff / 10);
            _D = _;
            });

          return true;
        }
      }
      else if (e == VK_A) {
        if (a) {
          _Z64 = GetTickCount64();
          _A = _A + 1;

          return true;
        }
        else {
          UINT diff = static_cast<unsigned int>(GetTickCount64() - _Z64);

          za.enqueue_task([&_A, &diff, &driver]() mutable {
            prevent(driver, VZ_R, diff / 10);
            _A = _;
            });

          return true;
        }
      }
      else {
        return true;
      }
      });
    };
  std::thread thread1(action1);

  thread2.join();
  thread1.join();

  return +1;
}
