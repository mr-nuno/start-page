import os
import psutil


def fetch():
    # Memory
    mem = psutil.virtual_memory()

    # Disk - use /host if mounted (Docker), else /
    disk_path = "/host" if os.path.ismount("/host") else "/"
    disk = psutil.disk_usage(disk_path)

    # CPU temperature
    temp = _get_cpu_temp()

    # CPU usage
    cpu_percent = psutil.cpu_percent(interval=0.5)

    return {
        "memory": {
            "total_gb": round(mem.total / (1024**3), 1),
            "used_gb": round(mem.used / (1024**3), 1),
            "percent": mem.percent,
        },
        "disk": {
            "total_gb": round(disk.total / (1024**3), 1),
            "used_gb": round(disk.used / (1024**3), 1),
            "free_gb": round(disk.free / (1024**3), 1),
            "percent": disk.percent,
        },
        "cpu": {
            "percent": cpu_percent,
            "temp_c": temp,
        },
    }


def _get_cpu_temp():
    # Try psutil sensors first
    try:
        temps = psutil.sensors_temperatures()
        if temps:
            for name in ("coretemp", "k10temp", "cpu_thermal", "acpitz"):
                if name in temps and temps[name]:
                    return round(temps[name][0].current, 1)
            # Fallback: first available sensor
            first = next(iter(temps.values()))
            if first:
                return round(first[0].current, 1)
    except Exception:
        pass

    # Try reading thermal zone directly
    for i in range(10):
        path = f"/sys/class/thermal/thermal_zone{i}/temp"
        try:
            with open(path) as f:
                return round(int(f.read().strip()) / 1000, 1)
        except (FileNotFoundError, ValueError):
            continue

    return None
