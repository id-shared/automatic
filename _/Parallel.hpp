#pragma once
#include <functional>
#include <queue>
#include <mutex>

namespace Parallel {
  using Byte = unsigned char;
  std::atomic<bool> keep_running{ true };

  class Pool {
  public:
    Pool(size_t num_threads);
    ~Pool();

    void enqueue_task(std::function<void()> task);
    bool is_busy();

  private:
    std::vector<std::thread> workers;
    std::queue<std::function<void()>> tasks;
    std::mutex queue_mutex;
    std::condition_variable condition;
    std::atomic<bool> stop;

    void worker_thread();
  };

  Pool::Pool(size_t num_threads) : stop(false) {
    for (size_t i = 0; i < num_threads; ++i) {
      workers.emplace_back(&Pool::worker_thread, this);
    }
  }

  Pool::~Pool() {
    stop = true;
    condition.notify_all();
    for (std::thread& worker : workers) {
      if (worker.joinable())
        worker.join();
    }
  }

  void Pool::enqueue_task(std::function<void()> task) {
    {
      std::lock_guard<std::mutex> lock(queue_mutex);
      tasks.push(task);
    }
    condition.notify_one();
  }

  void Pool::worker_thread() {
    while (!stop) {
      std::function<void()> task;
      {
        std::unique_lock<std::mutex> lock(queue_mutex);
        condition.wait(lock, [this] { return stop || !tasks.empty(); });
        if (stop && tasks.empty())
          return;
        task = std::move(tasks.front());
        tasks.pop();
      }
      task();
    }
  }

  bool Pool::is_busy() {
    std::lock_guard<std::mutex> lock(queue_mutex);
    return !tasks.empty();
  }
}
