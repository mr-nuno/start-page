# start-page

Personal browser start page dashboard running on k3s at `https://start.pew.local`.

## Stack

- **App:** Flask + Gunicorn (Python 3.12)
- **Deploy:** k3s with Helm, nginx ingress, mkcert TLS (`*.pew.local`)
- **Host:** `192.168.86.249` (local access only)

## Update workflow

After making changes to the app:

```bash
# 1. Rebuild the image
podman build -t start-page-startpage:latest .

# 2. Import into k3s containerd
podman save docker.io/library/start-page-startpage:latest | sudo k3s ctr images import -

# 3. Restart the pod to pick up the new image
kubectl rollout restart deployment start-page

# 4. Watch rollout
kubectl rollout status deployment start-page
```

## Helm chart

The chart lives in `chart/`. To change configuration:

```bash
# Edit values
vim chart/values.yaml

# Apply changes
helm upgrade start-page ./chart
```

To uninstall:

```bash
helm uninstall start-page
```

## Volumes

| Host path | Container path | Mode |
|---|---|---|
| `/sys/class/thermal` | `/sys/class/thermal` | ro |
| `/sys/class/hwmon` | `/sys/class/hwmon` | ro |
| `/` | `/host` | ro |
| `/home/peter/pew` | `/vault` | rw |
