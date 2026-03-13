# start-page

Personal browser start page dashboard running on k3s at `https://start.pew.local`.

## Stack

- **App:** Flask + Gunicorn (Python 3.12)
- **Deploy:** k3s with Argo CD GitOps, nginx ingress, mkcert TLS (`*.pew.local`)
- **Registry:** `registry.pew.local`
- **GitOps repo:** `~/Projects/k3s-gitops`
- **Host:** `192.168.86.249` (local access only)

## Deploy a code change

```bash
TAG=$(date +%s)
podman build -t registry.pew.local/start-page:$TAG .
podman push registry.pew.local/start-page:$TAG

cd ~/Projects/k3s-gitops
sed -i "s/tag: .*/tag: \"$TAG\"/" apps/start-page/values.yaml
git add -A && git commit -m "start-page: $TAG"
```

Argo CD picks up the commit and rolls out the new image.

## Deploy a config change

Edit `~/Projects/k3s-gitops/apps/start-page/values.yaml`, commit. Argo CD auto-syncs.

## Volumes

| Host path | Container path | Mode |
|---|---|---|
| `/sys/class/thermal` | `/sys/class/thermal` | ro |
| `/sys/class/hwmon` | `/sys/class/hwmon` | ro |
| `/` | `/host` | ro |
| `/home/peter/pew` | `/vault` | rw |
