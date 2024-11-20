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

bool move(HANDLE x, double e_y, double e_x, double e_4, double e_3, double e_2, double e_1, bool a) {
  const double y_ = e_2 >= _ ? min(e_4, e_2) : max(-e_4, e_2);
  const double x_ = e_1 >= _ ? min(e_3, e_1) : max(-e_3, e_1);

  if (a) {
    const int _x = +1; // x_ >= -e_3 && x_ <= e_3 ? +2 : +1;

    if (y_ >= _) {
      return Xyloid2::yx(x, to_integer((y_ * e_y) / +2), to_integer((x_ * e_x) * _x));
    }
    else {
      return Xyloid2::yx(x, _, to_integer((x_ * e_x) * _x));
    }
  }
  else {
    return Xyloid2::yx(x, to_integer(y_ * e_y), to_integer(x_ * e_x));
  }
};

bool taps(HANDLE x, double e, bool a) {
  if (a) {
    return false;
  }
  else {
    Xyloid2::e1(x, true);
    Xyloid2::e1(x, false);
    Time::XO(e);
    return true;
  }
};

bool pattern(HANDLE x, int e, bool a) {
  const int y_ = (a ? +1 : -1) * Pattern::dy(e);
  const int _y = +3;

  if (abs(y_) > _) {
    Xyloid2::yx(x, e % +2 == _ ? y_ : _, _);
    Time::XO(_y);
    Xyloid2::yx(x, y_, _);
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
  bool _r = false;
  bool _l = false;

  std::function<void()> action2 = [&_l, &_r, &driver]() {
    Parallel::ThreadPool parallel2(1);
    Parallel::ThreadPool parallel1(1);

    const int time = +16;
    const int size = +64;
    int at = +1;

    Event::KeyboardHook hook([&_l, &_r, &at, &driver, &parallel1, &parallel2](UINT e, bool a) {
      if (e == VK_OEM_6) {
        _r = a;

        parallel1.enqueue_task([&_r, &driver]() mutable {
          Xyloid2::e2(driver, _r);
          });

        return false;
      }
      else if (e == VK_OEM_4) {
        if (a) {
          _l = a;

          parallel1.enqueue_task([&_l, &at, &driver, &parallel2]() mutable {
            Xyloid2::e1(driver, _l);

            at = till([&_l, &parallel2, &driver](int e) {
              const bool back = _l && (size >= e);

              if (back) {
                parallel2.enqueue_task([e, &driver]() mutable {
                  pattern(driver, e, true);
                  });

                Time::XO(time / +0.999);

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
          _l = a;

          parallel1.enqueue_task([&_l, &at, &driver, &parallel2]() mutable {
            Xyloid2::e1(driver, _l);

            at = upon([&_l, &driver, &parallel2](int e) {
              const bool back = !_l && (+1 <= e);

              if (back) {
                parallel2.enqueue_task([&driver, e]() mutable {
                  pattern(driver, e, false);
                  });

                Time::XO(time / +1.499);

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
  std::thread thread2(action2);

  std::function<void()> action1 = [&_l, &_r, &driver]() {
    Parallel::ThreadPool system(std::thread::hardware_concurrency());
    const int xy = GetSystemMetrics(SM_CYSCREEN);
    const int xx = GetSystemMetrics(SM_CXSCREEN);

    const double ey = +0.429 * +4 / +4;
    const double ex = +0.429 * +4;

    const int cy_ = xy / +32;
    const int cx_ = xx / +32;
    const int cy = cy_ / +2;
    const int cx = cx_ / +2;

    const int ay_ = xy / +16;
    const int ax_ = xx / +9;
    const int ay = ay_ / +2;
    const int ax = ax_ / +2;

    std::function<bool(int, int)> work = [&_l, &_r, &cx, &cy, &ex, &ey, &driver, &system](int e_1, int e) {
      if (_r && -cx <= e && +cx >= e && -cy <= e_1 && +cy >= e_1) {
        system.enqueue_task([&_l, &cx, &cy, &ex, &ey, &driver, &e_1, &e]() mutable {
          move(driver, ey, ex, cy, cx, e_1, e, _l);
          taps(driver, +999.999 / +3.999, _l);
          });
        return true;
      }
      else {
        system.enqueue_task([&_l, &cx, &cy, &ex, &ey, &driver, &e_1, &e]() mutable {
          move(driver, ey, ex, cy, cx, e_1, e, _l);
          });
        return true;
      }
      };

    std::function<bool(uint8_t*, UINT, UINT, UINT, bool)> find = [&ax, &ay, &work](uint8_t* o1, UINT e_2, UINT e_1, UINT e, bool a) {
      const int y_ = e_2 / +2;
      const int x_ = e_1 / +2;
      const int _y = +2;
      const int _x = +2;

      for (UINT e_y = _; e_y < e_2; ++e_y) {
        uint8_t* px_y = o1 + ((ay - y_) + e_y) * e;

        for (UINT e_x = _; e_x < e_1; ++e_x) {
          uint8_t* px_x = px_y + ((ax - x_) + e_x) * 4;

          if (is_red(px_x)) {
            const int axis_y = e_y - y_ + _y;
            const int axis_x = e_x - x_ + _x;

            if (!a) {
              return work(axis_y / +1, axis_x / +2);
            }
            else {
              return work(axis_y / +1, axis_x / +1);
            }
          }
        }
      }

      return false;
      };

    int at = +256;
    std::function<bool(uint8_t*, UINT, UINT, UINT)> each = [&at, &find](uint8_t* o1, UINT e_2, UINT e_1, UINT e) {
      /***/if (find(o1, e_2, e_1 / +8, e, false)) {
        return true;
      }
      else if (find(o1, e_2, e_1 / +1, e, true)) {
        return true;
      }
      else {
        at = +256;
        return true;
      }
      };

    Capture::screen(each, (xy - ay_) / +2, (xx - ax_) / +2, ay_, ax_, +2);
    };
  std::thread thread1(action1);

  return +1;
}
