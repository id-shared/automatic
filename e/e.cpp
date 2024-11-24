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

static bool move(HANDLE x, double e_3, double e_2, double e_1, double e, bool a) {
  const int y_ = a ? e_3 > _ ? to_integer(min(e_3, e_1)) : to_integer(max(e_3, -e_1)) : to_integer(e_3);
  const int x_ = a ? e_2 > _ ? to_integer(min(e_2, e)) : to_integer(max(e_2, -e)) : to_integer(e_2);

  return Xyloid2::yx(x, y_, x_);
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

  constexpr UINT frame_rate = +1;
  constexpr UINT VZ_R = 0x4d;
  constexpr UINT VZ_L = 0x4b;
  constexpr UINT VK_D = 0x44;
  constexpr UINT VK_A = 0x41;

  ULONGLONG _z64 = GetTickCount64();
  UINT _z = _;
  UINT _y = _;
  UINT _x = _;
  UINT _r = _;
  UINT _l = _;
  UINT _d = _;
  UINT _a = _;

  std::function<void()> action2 = [&_l, &_r, &_x, &_y, &_z, &driver]() mutable {
    Parallel::Pool zz(+1000);
    Parallel::Pool zy(+1);
    Parallel::Pool zx(+1);

    const int xy = GetSystemMetrics(SM_CYSCREEN);
    const int xx = GetSystemMetrics(SM_CXSCREEN);

    const int ey = xy / +16;
    const int ex = xx / +8;

    const int cy = ey / +2;
    const int cx = ex / +2;

    const int ay = +2;
    const int ax = +2;


    std::function<bool(double, double, int, int, bool)> work2 = [&_x, &_y, &_z, &ax, &ay, &xx, &xy, &driver](double e_3, double e_2, int e_1, int e, bool a) mutable {
        const bool back = (e == _x && e_1 == _y) || move(driver, e_1 * ay * (_z > _ ? _ : +1), e * ax * (_z > _ ? +0.5 : +1), xy / e_3, xx / e_2, _z > _);

        Time::XO(+4);

        _z = a ? _ : _z + 1;
        _y = a ? _ : e_1;
        _x = a ? _ : e;

        return back;
      };

    std::function<bool(int, int, int, int)> task = [&_l, &_r, &work2](int e_3, int e_2, int e_1, int e) mutable {
      const int y_ = e_3 + e_1;
      const int x_ = e_2 + e;

      /***/if (_r > _) {
        return true;
      }
      else if (_l > _) {
        return work2(+32, +32, y_, x_, false);
      }
      else {
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

    std::function<bool(uint8_t*, UINT, UINT, UINT)> each = [&_x, &_y, &zz, &find](uint8_t* o1, UINT e_2, UINT e_1, UINT e) mutable {
      zz.enqueue_task([&find, o1, e_2, e_1, e]() mutable {
        /***/if (find(o1, e_2, e_1, e)) {
          return true;
        }
        else {
          return true;
        }
        });

      return true;
      };

    Capture::screen(each, (xy - ey) / +2, (xx - ex) / +2, ey, ex, frame_rate);
    };
  std::thread thread2(action2);


  std::function<void()> action1 = [&_a, &_d, &_l, &_r, &_z, &_z64, &driver]() mutable {
    Parallel::Pool zy(+1);
    Parallel::Pool zx(+1);
    Parallel::Pool zr(+1);
    Parallel::Pool zl(+1);
    Parallel::Pool zd(+1);
    Parallel::Pool za(+1);

    const int size = +64;
    const int each = +16;
    int at = +1;

    Event::KeyboardHook hook([&_a, &_d, &_l, &_r, &_z, &_z64, &at, &driver, &za, &zd, &zl, &zr, &zy](UINT e, bool a) mutable {
      /***/if (e == VK_OEM_6) {
        if (a) {
          _r = _r + 1;
          _z = _;

          zr.enqueue_task([&_r, &driver]() mutable {
            _r > _ ? Xyloid2::e2(driver, true) : _;
            });

          return false;
        }
        else {
          _r = _;

          zr.enqueue_task([&_r, &driver]() mutable {
            _r > _ ? _ : Xyloid2::e2(driver, false);
            });

          return false;
        }
      }
      else if (e == VK_OEM_4) {
        if (a) {
          _l = _l + 1;
          _z = _;

          zl.enqueue_task([&_a, &_d, &_l, &_z, &at, &driver, &zy]() mutable {
            UINT e_ = _;
            while (_l > _ && _z == _ && e_ < +16) {
              Time::XO(+1);
              e_ = e_ + 1;
            }

            while (_a > _ || _d > _) {
              Time::XO(frame_rate);
            }

            _l > _ ? Xyloid2::e1(driver, true) : _;

            at = till([&_l, &driver, &zy](int e) {
              const bool back = _l > _ && (size >= e);

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
              }, at) - 1;
            });

          return false;
        }
        else {
          _l = _;

          zl.enqueue_task([&_l, &at, &driver, &zy]() mutable {
            _l > _ ? _ : Xyloid2::e1(driver, false);

            at = upon([&_l, &driver, &zy](int e) {
              const bool back = !(_l > _) && (+1 <= e);

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
              }, at) + 1;
            });

          return false;
        }
      }
      else if (e == VK_D) {
        if (a) {
          _z64 = GetTickCount64();
          _d = _d + 1;

          return true;
        }
        else {
          UINT diff = static_cast<unsigned int>(GetTickCount64() - _z64);

          zd.enqueue_task([&_d, &diff, &driver]() mutable {
            prevent(driver, VZ_L, diff / 10);
            _d = _;
            });

          return true;
        }
      }
      else if (e == VK_A) {
        if (a) {
          _z64 = GetTickCount64();
          _a = _a + 1;

          return true;
        }
        else {
          UINT diff = static_cast<unsigned int>(GetTickCount64() - _z64);

          za.enqueue_task([&_a, &diff, &driver]() mutable {
            prevent(driver, VZ_R, diff / 10);
            _a = _;
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

/*x_ == _x && y_ == _y ? _ : move(driver, y_ * ay, x_ * ax, xy / +64., xx / +64., _z > _);

Time::XO(+4);

if (_x > _ && abs(e_2) < +4) {
  Xyloid2::e1(driver, true);
  Xyloid2::e1(driver, false);
}

_z = _z + 1;
_y = y_;
_x = x_;*/
