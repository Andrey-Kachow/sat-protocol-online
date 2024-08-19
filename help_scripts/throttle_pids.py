import os
import psutil
import re
import subprocess
import time


'''
    Limit the number of cores used by all processes that have 'wasm' in it.
    Such processes include those that unity runs to build and run WebGL build...

    Run this script if building Unity WebGL caused you BSOD error.
    Or if you want to prevent it from happening.
'''


ALLOWED_NUM_CORES_FOR_WASM = 10
PERIOD_SECONDS = 15


def set_windows_terminal_size(cols, rows):
    os.system(f'mode con: cols={cols} lines={rows}')


def pids_contain_substr(*substrs):
    matching_pids = []
    for proc in psutil.process_iter(['pid', 'name']):
        try:
            for substr in substrs:
                if substr in proc.info['name']:
                    matching_pids.append(proc.info['pid'])
        except (psutil.NoSuchProcess, psutil.AccessDenied, psutil.ZombieProcess):
            pass
    return matching_pids


def thin_log(*args):
    print()
    print(*args, end=" ", flush=True)
    

def get_affinity(pid):
    try:
        p = psutil.Process(pid)
        return p.cpu_affinity()
    except psutil.NoSuchProcess:
        return []


def set_affinity(pid, cpus):
    try:
        p = psutil.Process(pid)
        p.cpu_affinity(cpus)
    except psutil.NoSuchProcess:
        pass


def trhottle_pids(pids):
    for pid in pids:
        set_affinity(pid, get_affinity(pid)[:ALLOWED_NUM_CORES_FOR_WASM])
    thin_log("Throttled pids:", *pids)


def main():
    #
    #  Keep Terminal window small but visible
    # 
    set_windows_terminal_size(50, 1)
    while True:
        matching_pids = pids_contain_substr('wasm', 'Unity Job')
        trhottle_pids(matching_pids)
        time.sleep(PERIOD_SECONDS)


if __name__ == '__main__':
    main()