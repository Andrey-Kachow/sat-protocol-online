const Semaphore = {
  newInstance: () => {
    let __semaphoreIsNotReleased = true;
    let __releaseSemaphoreRightNow;
    return {
      aquire: async () => {
        if (__semaphoreIsNotReleased) {
          await new Promise((resolve) => {
            __releaseSemaphoreRightNow = resolve;
          });
        }
      },
      release: () => {
        if (__releaseSemaphoreRightNow) {
          __releaseSemaphoreRightNow();
        } else {
          __semaphoreIsNotReleased = false;
        }
      },
      reset: () => {
        __semaphoreIsNotReleased = true;
        __releaseSemaphoreRightNow = undefined;
      }
    };
  }
};