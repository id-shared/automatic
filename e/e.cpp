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

bool adjust(std::function<bool(double)> z, double value, int steps_count) {
  steps_count <= 0 ? throw steps_count : steps_count;

  double magnitude = std::abs(value);
  double base_step = std::floor(magnitude / steps_count);
  double remainder = magnitude - base_step * steps_count;
  double sign = (value < 0) ? -1.0 : 1.0;

  std::vector<double> steps(steps_count, base_step);

  for (int i = 0; i < static_cast<int>(remainder); ++i) {
    steps[i] += 1.0;
  }

  for (int i = 0; i < steps_count; ++i) {
    double adjustment = steps[i] * sign;
    value -= adjustment;
    adjustment == _ ? _ : z(adjustment);
  }

  return true;
}

static bool move(HANDLE x, double e_6, double e_5, double e_4, double e_3, double e_2, double e_1, double e) {
  double y_ = e_4 == _ ? _ : e_4 >= _ ? min(e_2, e_4) : max(-e_2, e_4);
  double x_ = e_3 == _ ? _ : e_3 >= _ ? min(e_1, e_3) : max(-e_1, e_3);

  Xyloid2::yx(x, to_integer(e_6 * y_), _);

  adjust([x, e_5](double e) mutable {
    const int x_ = to_integer(e_5 * e);
    Xyloid2::yx(x, _, x_);
    return Time::XO(+1);
    }, x_, to_integer(e));

  return true;
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

  /*adjust([](double e) mutable {
    std::cout << e << std::endl;
    return true;
    }, +0, +32);*/

  HANDLE driver = Device::driver(device);

  constexpr UINT VZ_R = 0x4d;
  constexpr UINT VZ_L = 0x4b;
  constexpr UINT VK_D = 0x44;
  constexpr UINT VK_A = 0x41;
  constexpr UINT fr = +64;

  ULONGLONG _Z64 = GetTickCount64();
  UINT _Y = _;
  UINT _X = _;
  UINT _R = _;
  UINT _L = _;
  UINT _D = _;
  UINT _A = _;

  std::function<void()> z2 = [&_A, &_D, &_L, &_R, &_X, &_Y, &driver]() mutable {
    Parallel::Pool xz(+1000);
    Parallel::Pool xr(+1);
    Parallel::Pool xl(+1);
    Parallel::Pool xa(+1);

    const int ey = GetSystemMetrics(SM_CYSCREEN);
    const int ex = GetSystemMetrics(SM_CXSCREEN);

    const int cy = ey / +16;
    const int cx = ex / +8;

    const int ay = cy / +2;
    const int ax = cx / +2;

    double _y = _;
    double _x = _;

    std::function<bool(double, double, double)> work = [&_x, &_y, &_X, &_Y, &xl, &driver](double e_2, double e_1, double e) mutable {
      xl.enqueue_task([&_x, &_y, &_X, &_Y, &driver, e_2, e_1, e]() mutable {
        (e_2 == _y && e_1 == _x) || move(driver, +2, +2, _Y > _ ? _ : e_2, e_1, e * +16, e * +16, fr);

        _Y = abs(e_2) < e ? +1 : _Y;
        _X = abs(e_1) < e ? +1 : _X;

        _y = e_2;
        _x = e_1;

        return true;
        });

      return true;
      };

    std::function<bool(double, double, double)> task = [&_x, &_y, &_L, &_R, &_X, &xa, &xl, &xr, &work](double e_2, double e_1, double e) mutable {
      const double y_ = e_2 + e;
      const double x_ = e_1 + e;

      /***/if (_R > _) {
        return work(y_, x_, e);
      }
      else if (_L > _) {
        return work(y_, x_, e);
      }
      else {
        _X = _;

        _y = _;
        _x = _;

        return true;
      }
      };

    std::function<bool(uint8_t*, UINT, UINT, UINT)> find = [&ax, &ay, &ey, &task](uint8_t* o1, UINT e_2, UINT e_1, UINT e) mutable {
      const int ny = e_2 / +2;
      const int nx = e_1 / +2;

      for (UINT e_y = _; e_y < e_2; ++e_y) {
        uint8_t* px_y = o1 + ((ay - ny) + e_y) * e;

        for (UINT e_x = _; e_x < e_1; ++e_x) {
          uint8_t* px_x = px_y + ((ax - nx) + e_x) * +4;

          if (is_red(px_x)) {
            const int axis_y = e_y - ny;
            const int axis_x = e_x - nx;

            return task(axis_y, axis_x, ey / +512.);
          }
        }
      }

      return false;
      };

    std::function<bool(uint8_t*, UINT, UINT, UINT)> each = [&_X, &_Y, &xz, &find](uint8_t* o1, UINT e_2, UINT e_1, UINT e) mutable {
      xz.enqueue_task([&_X, &_Y, &find, o1, e_2, e_1, e]() mutable {
        /***/if (find(o1, _Y > _ ? e_2 / +4 : e_2, _X > _ ? e_1 / +4 : e_1, e)) {
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
  std::thread thread2(z2);


  std::function<void()> z1 = [&_A, &_D, &_L, &_R, &_X, &_Y, &_Z64, &driver]() mutable {
    Parallel::Pool xy(+1);
    Parallel::Pool xx(+1);
    Parallel::Pool xr(+1);
    Parallel::Pool xl(+1);
    Parallel::Pool xd(+1);
    Parallel::Pool xa(+1);

    const int size = +64;
    const int each = +16;
    const bool is = false;
    int at = +1;

    Event::KeyboardHook hook([&_A, &_D, &_L, &_R, &_X, &_Y, &_Z64, &at, &driver, &xa, &xd, &xl, &xr, &xy](UINT e, bool a) mutable {
      /***/if (e == VK_OEM_6) {
        if (a) {
          _R = +1;
          _Y = _;
          _X = _;

          xr.enqueue_task([&_R, &_X, &_Y, &driver]() mutable {
            _R > _ ? Xyloid2::e2(driver, true) : _;
            });

          return false;
        }
        else {
          _R = _;

          xr.enqueue_task([&_R, &driver]() mutable {
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

          xl.enqueue_task([&_A, &_D, &_L, &_X, &_Y, &at, &driver, &xy]() mutable {
            while (_A > _ || _D > _) {
              Time::XO(fr);
            }

            UINT e_ = _;
            while (_L > _ && (_X == _ || _Y == _) && e_ < +1024) {
              Time::XO(fr);
              e_ = e_ + fr;
            }

            _L > _ ? Xyloid2::e1(driver, true) : _;

            is ? at = till([&_L, &driver, &xy](int e) {
              const bool back = _L > _ && (size >= e);

              if (back) {
                xy.enqueue_task([e, &driver]() mutable {
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

          xl.enqueue_task([&_L, &at, &driver, &xy]() mutable {
            _L > _ ? _ : Xyloid2::e1(driver, false);

            is ? at = upon([&_L, &driver, &xy](int e) {
              const bool back = !(_L > _) && (+1 <= e);

              if (back) {
                xy.enqueue_task([&driver, e]() mutable {
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

          xd.enqueue_task([&_D, &diff, &driver]() mutable {
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

          xa.enqueue_task([&_A, &diff, &driver]() mutable {
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
  std::thread thread1(z1);

  thread2.join();
  thread1.join();

  return +1;
}
