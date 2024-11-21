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
  const int _y = +4;

  if (abs(y_) > _) {
    Xyloid2::yx(x, to_integer(y_ * 1.5), _);
    Time::XO(_y);
    Xyloid2::yx(x, y_, _);
    Time::XO(_y);
    Xyloid2::yx(x, y_, _);
    Time::XO(_y);
    Xyloid2::yx(x, y_, _);
    Time::XO(_y);
    return true;
  }
  else {
    return true;
  }
}

bool move(HANDLE x, double e_2, double e_1, double e) {
  return Xyloid2::yx(x, to_integer(e_2), to_integer(e_1));
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
  Parallel::Pool _z(+1);
  UINT _y = _;
  UINT _x = _;
  UINT _r = _;
  UINT _l = _;

  std::function<void()> action2 = [&_l, &_r, &_x, &_y, &_z, &driver]() {
    Parallel::Pool zy(+1);
    Parallel::Pool zx(+1);

    const int xy = GetSystemMetrics(SM_CYSCREEN);
    const int xx = GetSystemMetrics(SM_CXSCREEN);

    const double ey = +0.429 * +4;
    const double ex = +0.429 * +4;

    const int cy = xy / +16;
    const int cx = xx / +4;

    const int ay = cy / +2;
    const int ax = cx / +2;

    std::function<bool(int, int, int)> work = [&_l, &_r, &_x, &_y, &zx, &ex, &ey, &driver](int e_2, int e_1, int e) {
      zx.enqueue_task([&_l, &_r, &_x, &_y, &ex, &ey, &e, &e_1, &e_2, &driver]() mutable {
        if (_r > _) {
          move(driver, (e_2 + e) * ey, (e_1 + e) * ex, +64);
          if (_x > +1 && abs(e_1) < +5) {
            Xyloid2::e1(driver, true);
            Xyloid2::e1(driver, false);
          }
        }
        else if (_l > _) {
          move(driver, (_y < +1 ? +1 : _) * (e_2 + e) * ey, (e_1 + e) * ex, +64);
        }
        else {
          move(driver, (e_2 + e) * ey, (e_1 + e) * ex, +64);
        }
        _y = _y + 1;
        _x = _x + 1;
        });

      return true;
      };

    std::function<bool(uint8_t*, UINT, UINT, UINT)> find = [&ax, &ay, &work](uint8_t* o1, UINT e_2, UINT e_1, UINT e) {
      const int y_ = e_2 / +2;
      const int x_ = e_1 / +2;

      for (UINT e_y = _; e_y < e_2; ++e_y) {
        uint8_t* px_y = o1 + ((ay - y_) + e_y) * e;

        for (UINT e_x = _; e_x < e_1; ++e_x) {
          uint8_t* px_x = px_y + ((ax - x_) + e_x) * 4;

          if (is_red(px_x)) {
            const int axis_y = e_y - y_;
            const int axis_x = e_x - x_;

            return work(axis_y, axis_x, +4);
          }
        }
      }

      return false;
      };

    std::function<bool(uint8_t*, UINT, UINT, UINT)> each = [&_l, &_r, &find](uint8_t* o1, UINT e_2, UINT e_1, UINT e) {
      if (_l > _ || _r > _) {
        /***/if (find(o1, e_2, e_1 / +4, e)) {
          return true;
        }
        else if (find(o1, e_2, e_1 / +1, e)) {
          return true;
        }
        else {
          return false;
        }
      }
      else {
        return false;
      }
      };

    Capture::screen(each, (xy - cy) / +2, (xx - cx) / +2, cy, cx, +2 * +2 * +2 * +2 * +2);
    };
  std::thread thread2(action2);


  std::function<void()> action1 = [&_l, &_r, &_x, &_y, &_z, &driver]() {
    Parallel::Pool zy(+1);
    Parallel::Pool zx(+1);
    Parallel::Pool zr(+1);
    Parallel::Pool zl(+1);

    const int time = +16;
    const int size = +64;
    int at = +1;

    Event::KeyboardHook hook([&_l, &_r, &_x, &_y, &_z, &at, &driver, &zl, &zr, &zy](UINT e, bool a) {
      if (e == VK_OEM_6) {
        if (a) {
          _r = _r + 1;

          zr.enqueue_task([&_r, &driver]() mutable {
            _r > _ ? Xyloid2::e2(driver, true) : _;
            });

          return false;
        }
        else {
          _y = _;
          _x = _;
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

          zl.enqueue_task([&_l, &_x, &_y, &at, &driver, &zy]() mutable {
            int ms = _;
            while (_l > _ && _x < +2 && ms < (+256 / +2)) {
              Time::XO(+1);
              ms = ms + 1;
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
          _y = _;
          _x = _;
          _l = _;

          zl.enqueue_task([&_l, &_x, &_y, &at, &driver, &zy]() mutable {
            _l > _ ? _ : Xyloid2::e1(driver, false);

            at = upon([&_l, &driver, &zy](int e) {
              const bool back = !(_l > _) && (+1 <= e);

              if (back) {
                zy.enqueue_task([&driver, e]() mutable {
                  pattern(driver, e, false);
                  });

                Time::XO(time / +2);

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
      else if (e == VK_A || e == VK_D) {
        if (a) {
          return true;
        }
        else {
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
