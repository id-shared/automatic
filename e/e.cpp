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

static bool move(HANDLE x, double e_3, double e_2, double e_1, double e) {
  const double y_ = e_3 == _ ? _ : e_3 >= _ ? min(e_1, e_3) : max(-e_1, e_3);
  const double x_ = e_2 == _ ? _ : e_2 >= _ ? min(e, e_2) : max(-e, e_2);

  return Xyloid2::yx(x, to_integer(y_), to_integer(x_));
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
  UINT _Y = _;
  UINT _X = _;
  UINT _R = _;
  UINT _L = _;
  UINT _D = _;
  UINT _A = _;

  std::function<void()> action2 = [&_A, &_D, &_L, &_R, &_X, &_Y, &driver]() mutable {
    Parallel::Pool zz(+1000);
    Parallel::Pool zr(+1);
    Parallel::Pool zl(+1);
    Parallel::Pool za(+1);

    const double xy = +2;
    const double xx = +2;

    const int ey = GetSystemMetrics(SM_CYSCREEN);
    const int ex = GetSystemMetrics(SM_CXSCREEN);

    const int cy = ey / +16;
    const int cx = ex / +8;

    const int ay = cy / +2;
    const int ax = cx / +2;

    double _y = _;
    double _x = _;

    std::function<bool(double, double, double, double)> work = [&_x, &_y, &_X, &_Y, &xx, &xy, &zl, &driver](double e_3, double e_2, double e_1, double e) mutable {
      zl.enqueue_task([&_x, &_y, &_X, &_Y, &xx, &xy, &driver, e_3, e_2, e_1, e]() mutable {
        const bool back = (e_3 == _y && e_2 == _x) || move(driver, e_3 * (_Y > _ ? _ : xy), e_2 * xx, e_1 * xy * +16, e * xx * +16);

        _Y = abs(e_3) < e_1 ? +1 : _Y;
        _X = abs(e_2) < e ? +1 : _X;

        _y = e_3;
        _x = e_2;
        });

      return true;
      };

    std::function<bool(double, double, double, double)> task = [&_x, &_y, &_L, &_R, &_X, &za, &zl, &zr, &work](double e_3, double e_2, double e_1, double e) mutable {
      const double y_ = e_3 + e_1;
      const double x_ = e_2 + e;

      /***/if (_R > _) {
        return work(y_, x_, e_1, e);
      }
      else if (_L > _) {
        return work(y_, x_, e_1, e);
      }
      else {
        _X = _;

        _y = _;
        _x = _;

        return true;
      }
      };

    std::function<bool(uint8_t*, UINT, UINT, UINT)> find = [&ax, &ay, &ex, &ey, &task](uint8_t* o1, UINT e_2, UINT e_1, UINT e) mutable {
      const int ny = e_2 / +2;
      const int nx = e_1 / +2;

      for (UINT e_y = _; e_y < e_2; ++e_y) {
        uint8_t* px_y = o1 + ((ay - ny) + e_y) * e;

        for (UINT e_x = _; e_x < e_1; ++e_x) {
          uint8_t* px_x = px_y + ((ax - nx) + e_x) * +4;

          if (is_red(px_x)) {
            const int axis_y = e_y - ny;
            const int axis_x = e_x - nx;

            return task(axis_y, axis_x, ey / +256., ex / +1024.);
          }
        }
      }

      return false;
      };

    std::function<bool(uint8_t*, UINT, UINT, UINT)> each = [&_X, &_Y, &zz, &find](uint8_t* o1, UINT e_2, UINT e_1, UINT e) mutable {
      zz.enqueue_task([&_X, &find, o1, e_2, e_1, e]() mutable {
        /***/if (find(o1, e_2, _X > _ ? e_1 / +16 : e_1, e)) {
          return true;
        }
        else {
          return true;
        }
        });

      return true;
      };

    Capture::screen(each, (ey - cy) / +2, (ex - cx) / +2, cy, cx, fr);
    };
  std::thread thread2(action2);


  std::function<void()> action1 = [&_A, &_D, &_L, &_R, &_X, &_Y, &_Z64, &driver]() mutable {
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

    Event::KeyboardHook hook([&_A, &_D, &_L, &_R, &_X, &_Y, &_Z64, &at, &driver, &za, &zd, &zl, &zr, &zy](UINT e, bool a) mutable {
      /***/if (e == VK_OEM_6) {
        if (a) {
          _R = +1;
          _Y = _;
          _X = _;

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
          _L = +1;
          _Y = _;
          _X = _;

          zl.enqueue_task([&_A, &_D, &_L, &_X, &_Y, &at, &driver, &zy]() mutable {
            while (_A > _ || _D > _) {
              Time::XO(fr);
            }

            UINT e_ = _;
            while (_L > _ && _X == _ && _Y == _ && e_ < +128) {
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
            prevent(driver, VZ_L, diff / +10);
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
            prevent(driver, VZ_R, diff / +10);
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
