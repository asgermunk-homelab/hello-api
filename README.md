# hello-api

A minimal .NET 10 API. It is the proof of concept for the homelab pipeline: build in GitHub Actions, push to GHCR, deploy to k3s with Flux.

## Endpoints

| Path | Returns |
|---|---|
| `GET /` | A hello message, the environment name, and the pod name |
| `GET /healthz` | `healthy`, for the Kubernetes probes |

## Run it locally

With the .NET 10 SDK:

```bash
dotnet run --project src/HelloApi
```

With Docker:

```bash
docker build -t hello-api .
docker run --rm -p 8080:8080 hello-api
```

Then open http://localhost:8080.

## Pipeline

```
pull request  →  build + format check + image build (no push)
merge to main →  same checks → push ghcr.io/asgermunk-homelab/hello-api:<run>-<sha>-<unixtime>-dev
                 → Flux sees the new tag → deploys to the dev namespace
manual job    →  retag the same image to -prod → Flux deploys to prod
```

The deployment manifests live in the `gitops` repo, not here.
