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
  Parallel::ThreadPool system(std::thread::hardware_concurrency());
  Parallel::ThreadPool single(+1);

  LPCWSTR device = Contact::device([](std::wstring_view c) {
    using namespace std::literals;
    return c.starts_with(L"RZCONTROL"sv) && c.ends_with(L"{e3be005d-d130-4910-88ff-09ae02f680e9}"sv);
    });

  HANDLE driver = Device::driver(device);

  constexpr UINT VK_D = 0x44;
  constexpr UINT VK_A = 0x41;
  bool _y = false;
  bool _x = false;
  bool _r = false;
  bool _l = false;

  std::function<void()> action2 = [&_l, &_r, &_x, &_y, &driver, &single, &system]() {
    const double xy = +0.429 * +4;
    const double xx = +0.429 * +4;

    const int ey = GetSystemMetrics(SM_CYSCREEN);
    const int ex = GetSystemMetrics(SM_CXSCREEN);

    const int cy = ey / +16;
    const int cx = ex / +5;

    const int ay = cy / +2;
    const int ax = cx / +2;

    std::function<bool(int, int, int)> work = [&_x, &_y, &xx, &xy, &driver, &system](int e_2, int e_1, int e) {
      system.enqueue_task([&_x, &_y, &xx, &xy, &e, &e_1, &e_2, &driver]() mutable {
        Xyloid2::yx(driver, to_integer((xy * (e_2 + e)) * (_y ? _ : +1)), to_integer((xx * (e_1 + e)) / (_x ? +2 : +1)));
        _y = true;
        _x = true;
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

    std::function<bool(uint8_t*, UINT, UINT, UINT)> each = [&_l, &find](uint8_t* o1, UINT e_2, UINT e_1, UINT e) {
      if (_l) {
        /***/if (find(o1, e_2, e_1 / +8, e)) {
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

    Capture::screen(each, (ey - cy) / +2, (ex - cx) / +2, cy, cx, +16);
    };
  std::thread thread2(action2);


  std::function<void()> action1 = [&_l, &_r, &_x, &_y, &driver, &single, &system]() {
    Parallel::ThreadPool parallel2(1);
    Parallel::ThreadPool parallel1(1);

    const int time = +16;
    const int size = +64;
    int at = +1;

    Event::KeyboardHook hook([&_l, &_r, &_x, &_y, &at, &driver, &single, &system](UINT e, bool a) {
      if (e == VK_OEM_6) {
        _r = a;

        system.enqueue_task([&_r, &driver]() mutable {
          Xyloid2::e2(driver, _r);
          });

        return false;
      }
      else if (e == VK_OEM_4) {
        if (a) {
          _l = a;

          system.enqueue_task([&_l, &_x, &_y, &at, &driver, &single]() mutable {
            Time::XO(+32);

            Xyloid2::e1(driver, _l);

            at = till([&_l, &single, &driver](int e) {
              const bool back = _l && (size >= e);

              if (back) {
                single.enqueue_task([e, &driver]() mutable {
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
          _l = a;

          system.enqueue_task([&_l, &_x, &_y, &at, &driver, &single]() mutable {
            Time::XO(+32);
            _y = false;
            _x = false;

            Xyloid2::e1(driver, _l);

            at = upon([&_l, &driver, &single](int e) {
              const bool back = !_l && (+1 <= e);

              if (back) {
                single.enqueue_task([&driver, e]() mutable {
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
  std::thread thread1(action1);

  thread2.join();
  thread1.join();

  return +1;
}
