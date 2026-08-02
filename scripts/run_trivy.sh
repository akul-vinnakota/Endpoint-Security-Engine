#!/usr/bin/env bash

set -uo pipefail

IMAGE_NAME="secureforge:security-scan"

mkdir -p reports

echo "Building SecureForge Docker image..."
docker build --provenance=false --sbom=false --pull -t "$IMAGE_NAME" .

echo "Running Trivy container vulnerability scan..."

docker run --rm   -v /var/run/docker.sock:/var/run/docker.sock   -v "$PWD:/workspace"   -w /workspace   aquasec/trivy:latest   image   --scanners vuln   --detection-priority precise   --severity HIGH,CRITICAL   --ignore-unfixed   --exit-code 1   --format json   --output reports/trivy-report.json   "$IMAGE_NAME"

exit_code=$?

if [ "$exit_code" -ne 0 ]; then
  echo "Trivy detected fixable HIGH or CRITICAL vulnerabilities."
else
  echo "Trivy completed with no blocking fixable HIGH or CRITICAL vulnerabilities."
fi

exit "$exit_code"
