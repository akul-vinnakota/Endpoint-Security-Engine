# Changelog

All significant changes to SecureForge will be documented in this file.

## [Unreleased]

### Added

- Added Trivy container vulnerability scanning
- Added CI blocking for fixable HIGH and CRITICAL container findings
- Added JSON container vulnerability reporting
- Hardened the Docker image with a multi-stage runtime build to reduce unnecessary packages and attack surface

- Added Docker containerization for the FastAPI application
- Added a dedicated non-root `secureforge` runtime user
- Added `.dockerignore` exclusions for sensitive and unnecessary files
- Verified runtime environment-variable injection inside the container

- Added Gitleaks secret detection for repository content and Git history
- Added a reusable redacted secret-scanning script
- Integrated secret detection into GitHub Actions
- Added CI blocking when potential credentials or tokens are detected

- Added `pip-audit` dependency vulnerability scanning
- Added reusable dependency-audit script with JSON reporting
- Integrated dependency scanning into GitHub Actions
- Added CI blocking for known vulnerable Python packages

- Remediated the Bandit B105 hardcoded credential finding using environment variables
- Added secure missing-configuration handling with an HTTP 503 response
- Added an `.env.example` template with public placeholders
- Expanded automated test coverage to five tests

- Bandit static application security scanning
- Reusable security scan script with JSON reporting
- GitHub Actions workflow for automated tests and security scans
- CI security gate that fails when vulnerabilities are detected

- Controlled insecure demonstration login endpoint
- Authentication success and failure tests
- Documentation for authorized security testing

- FastAPI application foundation
- Root and health-check API endpoints
- Automated pytest coverage for API endpoints

### Planned

- FastAPI application foundation
- Automated unit testing
- Static application security testing
- Dependency vulnerability scanning
- Secret detection
- Docker security scanning
- Terraform security scanning
- GitHub Actions security gates
- AWS logging and detection capabilities

## [0.1.0] - 2026-07-15

### Added

- Initial GitHub repository
- Professional project README
- Security policy
- Contribution guidelines
- Initial repository structure
- Development roadmap
