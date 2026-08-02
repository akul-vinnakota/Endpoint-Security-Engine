# SecureForge

SecureForge is a DevSecOps cloud security pipeline designed to identify insecure code, vulnerable dependencies, exposed credentials, container vulnerabilities, and cloud infrastructure misconfigurations before deployment.

## Project Objective

The objective of SecureForge is to demonstrate how security controls can be integrated directly into the software development lifecycle rather than applied only after an application reaches production.

The pipeline will automatically evaluate source code, application dependencies, secrets, containers, and Infrastructure as Code configurations. High-risk findings will block insecure builds and generate actionable remediation reports.

## Planned Security Capabilities

- Static application security testing
- Python dependency vulnerability scanning
- Hardcoded secret detection
- Docker container vulnerability scanning
- Terraform security scanning
- GitHub Actions security gates
- AWS cloud security monitoring
- Automated security reporting
- Incident-response playbooks
- OWASP and MITRE ATT&CK mappings

## Planned Technology Stack

- Python
- FastAPI
- GitHub Actions
- Docker
- Terraform
- AWS
- Semgrep or Bandit
- pip-audit
- Gitleaks
- Trivy
- Checkov
- Pytest

## Planned Architecture

Developer Commit
    |
    v
GitHub Repository
    |
    v
GitHub Actions Security Pipeline
    |
    +--> Source Code Scan
    +--> Dependency Scan
    +--> Secret Scan
    +--> Container Scan
    +--> Terraform Scan
    |
    v
Security Gate
    |
    +--> Pass: Allow deployment
    |
    +--> Fail: Block deployment and generate report

## Repository Structure

secureforge-devsecops/
├── .github/
│   └── workflows/
├── app/
├── docs/
├── reports/
├── scripts/
├── security/
├── terraform/
├── tests/
├── CHANGELOG.md
├── CONTRIBUTING.md
├── SECURITY.md
├── pyproject.toml
├── requirements-dev.txt
└── requirements.txt

## Development Roadmap

### Phase 1 — Application Foundation

- Create the FastAPI application
- Add health-check and demonstration endpoints
- Add unit tests
- Containerize the application

### Phase 2 — DevSecOps Pipeline

- Add static-code scanning
- Add dependency scanning
- Add secret detection
- Add container scanning
- Add security gates to GitHub Actions

### Phase 3 — Cloud Security

- Build AWS infrastructure with Terraform
- Detect Infrastructure as Code misconfigurations
- Enable AWS security logging
- Analyze CloudTrail security events

### Phase 4 — Detection and Incident Response

- Detect suspicious AWS activity
- Map detections to MITRE ATT&CK
- Generate incident reports
- Create remediation playbooks

## Current Status

Day 1: Repository initialized and project structure created.

## Disclaimer

This project is intended exclusively for defensive cybersecurity education, controlled testing, and portfolio development.

## License

This project is licensed under the MIT License.

## Running the Application

Install the project dependencies:

```bash
pip install -r requirements.txt -r requirements-dev.txt
```

Start the FastAPI development server:

```bash
uvicorn app.main:app --host 0.0.0.0 --port 8000 --reload
```

Available endpoints:

- `GET /` — confirms that the SecureForge API is running
- `GET /health` — returns the service health status
- `GET /docs` — opens the interactive Swagger API documentation

## Running Tests

Run the automated test suite:

```bash
python -m pytest -v
```

## Current Progress

- Day 1: Initialized the repository and project structure
- Day 2: Built the FastAPI foundation, health endpoint, and automated tests


## Security Demonstration Endpoint

SecureForge includes an intentionally insecure authentication endpoint for authorized security testing.

- `GET /demo-login`
- Uses environment variables for demonstration credentials
- Returns `200 OK` for valid credentials
- Returns `401 Unauthorized` for invalid credentials
- Returns `503 Service Unavailable` when credentials are not configured

> Note: This endpoint is for controlled demonstrations only. Credentials must be supplied through environment variables and never committed.


## Static Security Scanning

SecureForge uses Bandit to scan Python source code for common security weaknesses.

Run the scan locally:

    ./scripts/run_bandit.sh

The scan generates a JSON report at:

    reports/bandit-report.json

Bandit previously detected the intentionally hardcoded demonstration password as a B105 finding. The credential was remediated by moving it to environment-based configuration, and the scan now passes with zero findings.

## Security CI Pipeline

GitHub Actions automatically performs the following checks on pushes and pull requests targeting `main`:

1. Installs production and development dependencies
2. Runs the automated pytest suite
3. Runs the Bandit static security scan
4. Fails the workflow when a security finding is detected

The pipeline now passes after remediating the hardcoded credential.


## Environment Configuration

SecureForge reads demonstration credentials from environment variables rather than storing them in source code.

Set local demonstration values before starting the application:

    export DEMO_USERNAME=secureforge-demo-user
    export DEMO_PASSWORD=choose-a-local-demo-password

The `.env.example` file contains public placeholders only. Real credentials and local `.env` files must never be committed.


## Dependency Vulnerability Scanning

SecureForge uses `pip-audit` to check Python production dependencies against known vulnerability databases.

Run the scan locally:

    ./scripts/run_pip_audit.sh

The scan generates a JSON report at:

    reports/pip-audit-report.json

The GitHub Actions security pipeline runs this check automatically and blocks the build when a known vulnerable dependency is detected.


## Secret Detection

SecureForge uses Gitleaks to scan Git history and repository content for exposed credentials, API keys, tokens, passwords, and other sensitive information.

Run the scan locally:

    ./scripts/run_gitleaks.sh

The scan generates a redacted JSON report at:

    reports/gitleaks-report.json

The GitHub Actions security pipeline automatically runs secret detection on pushes and pull requests targeting `main`. The workflow fails when a potential secret is detected.


## Containerization

SecureForge is containerized with Docker using a minimal Python 3.12 slim image.

Security controls include:

- Running the application as a dedicated non-root `secureforge` user
- Excluding sensitive and unnecessary files with `.dockerignore`
- Passing credentials through environment variables at runtime
- Exposing only the application port required by FastAPI

Build the image:

    docker build -t secureforge:latest .

Run the container:

    docker run --rm -p 8000:8000 \
      -e DEMO_USERNAME=secureforge-demo-user \
      -e DEMO_PASSWORD=example-local-demo-password \
      secureforge:latest

Verify the container health endpoint:

    curl http://127.0.0.1:8000/health
