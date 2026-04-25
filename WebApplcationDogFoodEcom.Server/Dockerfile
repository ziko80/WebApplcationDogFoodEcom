# ============================================================================
# FILE: Dockerfile
# LOCATION: WebApplcationDogFoodEcom.Server/Dockerfile
# ============================================================================
#
# PURPOSE
# -------
# This Dockerfile packages the published .NET 10 Aspire server (which
# already contains the React/Vite frontend bundled into wwwroot) into a
# lightweight, production-ready Linux container image.
#
# WHY THIS FILE WAS ADDED
# -----------------------
# The CI/CD pipeline (azure-pipelines.yml) deploys the application to
# Azure Container Apps, which requires a container image. Without a
# Dockerfile, there is no way to produce that image. This file bridges
# the gap between `dotnet publish` output and a runnable OCI image that
# Azure Container Apps (or any container host) can execute.
#
# WHAT IT DOES
# ------------
#   1. Starts from the official Microsoft ASP.NET 10 runtime image
#      (mcr.microsoft.com/dotnet/aspnet:10.0-preview). This is a
#      minimal image that contains only the runtime — no SDK, no
#      build tools — keeping the final image small and secure.
#
#   2. Sets /app as the working directory inside the container.
#
#   3. Exposes port 8080 — the default Kestrel HTTP port used by
#      ASP.NET applications in containerised environments.
#
#   4. Copies the pre-built publish output (produced by the pipeline's
#      `dotnet publish` step) into the container. Because the pipeline
#      already placed the React frontend dist files into wwwroot before
#      publishing, the single COPY brings in both the API and the SPA.
#
#   5. Defines the entrypoint that starts the .NET server process.
#
# DESIGN DECISIONS
# ----------------
#   • Runtime-only base image – The .NET SDK is NOT included. All
#     compilation happens in the CI pipeline, so the container only
#     needs the runtime. This reduces image size (~220 MB vs ~900 MB)
#     and minimises the attack surface.
#
#   • No multi-stage build – Normally Dockerfiles use multi-stage
#     builds (SDK stage → runtime stage). Here the pipeline handles
#     the build, so the Dockerfile receives already-published
#     binaries. This keeps the Dockerfile simple and the build
#     cache efficient in the pipeline.
#
#   • Single container for API + SPA – The Aspire AppHost uses
#     PublishWithContainerFiles(webfrontend, "wwwroot") which merges
#     both projects at publish time. One container means simpler
#     networking, no CORS configuration, and fewer moving parts.
# ============================================================================

# --- Runtime base image (Linux, amd64) --------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

# The build context is the published output from `dotnet publish`,
# so we simply copy everything into the runtime image.
COPY . .

ENTRYPOINT ["dotnet", "WebApplcationDogFoodEcom.Server.dll"]
