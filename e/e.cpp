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

int to_integer(double e) {
  return static_cast<int>(round(e));
}

bool pattern(HANDLE x, int e, bool a) {
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

bool move(HANDLE x, double e_3, double e_2, double e_1, double e, bool a) {
  const int y_ = a ? e_3 > _ ? to_integer(min(e_3, e_1)) : to_integer(max(e_3, -e_1)) : to_integer(e_3);
  const int x_ = a ? e_2 > _ ? to_integer(min(e_2, e)) : to_integer(max(e_2, -e)) : to_integer(e_2);

  Xyloid2::yx(x, y_, x_);

  return Time::XO(+4);
}

bool is_red(uint8_t* x) {
  return x[+0] <= +63 && x[+1] <= +63 && x[+2] >= (+255 - +4) && x[+3] == +255;
}

int upon(std::function<bool(int)> z, int i) {
  return z(i) ? upon(z, i - 1) : i;
}

int till(std::function<bool(int)> z, int i) {
  return z(i) ? till(z, i + 1) : i;
}

int main() {
  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL"sv) && c.ends_with(L"{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  constexpr UINT VK_D = 0x44;
  constexpr UINT VK_A = 0x41;
  UINT _z = +4;
  UINT _y = _;
  UINT _x = _;
  UINT _r = _;
  UINT _l = _;
  UINT _a = _;

  std::function<void()> action2 = [&_a, &_l, &_r, &_x, &_y, &_z, &driver]() {
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

    std::function<bool(int, int, int, int)> work = [&_a, &_l, &_r, &_x, &_y, &ax, &ay, &xx, &xy, &zx, &driver](int e_3, int e_2, int e_1, int e) {
      const int y_ = e_3 + e_1;
      const int x_ = e_2 + e;

      /***/if (_r > _) {
        x_ == _x && y_ == _y ? _ : move(driver, y_ * ay, x_ * ax, xy / +64., xx / +64., _a > _);

        if (_x > _ && abs(e_2) < +4) {
          Xyloid2::e1(driver, true);
          Xyloid2::e1(driver, false);
        }

        _a = _a + 1;
        _y = y_;
        _x = x_;

        return true;
      }
      else if (_l > _) {
        x_ == _x && y_ == _y ? _ : move(driver, y_ * ay * (_a > _ ? _ : +1), x_ * ax * (_a > _ ? +0.5 : +1), xy / +32., xx / +32., _a > _);

        _a = _a + 1;
        _y = y_;
        _x = x_;

        return true;
      }
      else {
        //move(driver, y_ == _y ? _ : y_ * ay * (_a > _ ? _ : +1), x_ == _x ? _ : x_ * ax * (_a > _ ? +0.5 : +1), xy / +64., xx / +64., _a > _);

        //zx.enqueue_task([&_l, &driver]() mutable {
        //  if (!(_l > _)) {
        //    Xyloid1::ea(driver, 0x1a, true);
        //    Time::XO(+128);
        //    Xyloid1::ea(driver, 0x1a, false);
        //    Time::XO(+128);
        //  }
        //  });

        return true;
      }
      };

    std::function<bool(uint8_t*, UINT, UINT, UINT)> find = [&cx, &cy, &xx, &xy, &work](uint8_t* o1, UINT e_2, UINT e_1, UINT e) {
      const int y_ = e_2 / +2;
      const int x_ = e_1 / +2;

      for (UINT e_y = _; e_y < e_2; ++e_y) {
        uint8_t* px_y = o1 + ((cy - y_) + e_y) * e;

        for (UINT e_x = _; e_x < e_1; ++e_x) {
          uint8_t* px_x = px_y + ((cx - x_) + e_x) * +4;

          if (is_red(px_x)) {
            const int axis_y = e_y - y_;
            const int axis_x = e_x - x_;

            return work(axis_y, axis_x, xy / 1024, xx / 1024);
          }
        }
      }

      return false;
      };

    std::function<bool(uint8_t*, UINT, UINT, UINT)> each = [&_x, &_y, &_z, &zz, &find](uint8_t* o1, UINT e_2, UINT e_1, UINT e) {
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

    Capture::screen(each, (xy - ey) / +2, (xx - ex) / +2, ey, ex, _z);
    };
  std::thread thread2(action2);


  std::function<void()> action1 = [&_a, &_l, &_r, &_z, &driver]() {
    Parallel::Pool zy(+1);
    Parallel::Pool zx(+1);
    Parallel::Pool zr(+1);
    Parallel::Pool zl(+1);

    const int time = +16;
    const int size = +64;
    int at = +1;

    Event::KeyboardHook hook([&_a, &_l, &_r, &_z, &at, &driver, &zl, &zr, &zy](UINT e, bool a) {
      /***/if (e == VK_OEM_6) {
        if (a) {
          _r = _r + 1;
          _a = _;

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
          _a = _;

          zl.enqueue_task([&_a, &_l, &_z, &at, &driver, &zy]() mutable {
            UINT e_ = _;
            while (_l > _ && _a == _ && e_ < +4) {
              Time::XO(_z);
              e_ = e_ + 1;
            }

            _l > _ ? Xyloid2::e1(driver, true) : _;

            at = till([&_l, &driver, &zy](int e) {
              const bool back = _l > _ && (size >= e);

              if (back) {
                zy.enqueue_task([e, &driver]() mutable {
                  pattern(driver, e, true);
                  });

                Time::XO(time);

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

                Time::XO(time);

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

/*Xyloid2::e2(driver, true);
Xyloid2::e2(driver, false);

while (_l > _) {
  Xyloid1::ea(driver, 0x1a, true);
  Time::XO(+99);
  Xyloid1::ea(driver, 0x1a, false);
  Xyloid2::e1(driver, true);
  Xyloid2::e1(driver, false);
}*/
