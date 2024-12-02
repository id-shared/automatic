#pragma once
#include <atomic>
#include <condition_variable>
#include <functional>
#include <mutex>
#include <queue>
#include <thread>
#include <vector>

namespace Parallel {
  using Byte = unsigned char;

  class Pool {
  public:
    explicit Pool(size_t num_threads);
    ~Pool();

    void enqueue_task(std::function<void()> task);
    bool is_busy();

  private:
    alignas(64) std::vector<std::thread> workers;
    std::queue<std::function<void()>> tasks;
    std::mutex queue_mutex;
    std::condition_variable condition;
    std::atomic<bool> stop{ false };

    void worker_thread();
  };

  Pool::Pool(size_t num_threads) {
    workers.reserve(num_threads);
    for (size_t i = 0; i < num_threads; ++i) {
      workers.emplace_back(&Pool::worker_thread, this);
    }
  }

  Pool::~Pool() {
    stop.store(true, std::memory_order_relaxed);
    condition.notify_all();
    for (std::thread& worker : workers) {
      if (worker.joinable()) {
        worker.join();
      }
    }
  }

  void Pool::enqueue_task(std::function<void()> task) {
    {
      std::lock_guard<std::mutex> lock(queue_mutex);
      tasks.emplace(std::move(task));
    }
    condition.notify_one();
  }

  void Pool::worker_thread() {
    while (true) {
      std::function<void()> task;
      {
        std::unique_lock<std::mutex> lock(queue_mutex);
        condition.wait(lock, [this] { return stop.load(std::memory_order_relaxed) || !tasks.empty(); });
        if (stop.load(std::memory_order_relaxed) && tasks.empty()) {
          return;
        }
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
