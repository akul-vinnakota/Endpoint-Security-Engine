# Endpoint Security Engine

This project investigates how endpoint security infrastructure can contain failures before they propagate into outages.

---

## Objective

Build and benchmark security architecture designed to prevent:

- faulty health decisions from unnecessarily removing healthy capacity,
- unsafe security changes from propagating across an endpoint fleet,
- shared failure domains from defeating redundant systems,
- parser or detection failures from destabilizing the host,
- retry, restart, and failover logic from amplifying an existing outage.

The system is being designed around fault isolation, independent health paths, health-gated rollout, rollback, graceful degradation, and redundant failover.

---

## Target Architecture

```text
                       ENDPOINT

             ┌────────────┬────────────┐
             ↓            ↓            ↓
            ETW       Event Logs    Future Sensors
             └────────────┼────────────┘
                          ↓
                  SENSOR ADAPTERS
                          ↓
                 COMMON EVENT MODEL
                          ↓
                  .NET CORE ENGINE
                          │
            ┌─────────────┼─────────────┐
            ↓             ↓             ↓
        Normalize      Detect        Enrich
            └─────────────┼─────────────┘
                          ↓
                   SAFE RULE ENGINE
                          ↓
                   SECURITY FINDING
                          ↓
                 ENDPOINT HEALTH
                          │
             ┌────────────┼────────────┐
             ↓            ↓            ↓
          Healthy      Degraded      Failed
             │            │            │
             ↓            ↓            ↓
          Continue     Shed Load    Fail Over
                          │
                          ↓
                  FLEET HEALTH GATE
                          ↓
              Canary → Expand → Rollback
```

---

### Fault Isolation

A parser, rule, sensor, or detection failure should remain inside its own process or component boundary rather than destabilizing the operating system.

### Independent Health Paths

Process liveness, service readiness, dependency health, and fleet capacity are treated as separate signals.

A failed dependency should not automatically cause otherwise healthy processes to restart and reduce available capacity.

### Safe Rollout

Security changes are deployed progressively:

```text
New Version / Rule
        ↓
      Canary
        ↓
 Health Evaluation
        ↓
   ┌────┴────┐
   ↓         ↓
Healthy    Degraded
   ↓         ↓
Expand    Stop Rollout
             ↓
          Roll Back
```

### Redundant Failover

Backup systems must not share the exact failure mode they are intended to protect against.

```text
Primary Path     Backup Path
     │                │
Failure Domain A  Failure Domain B
     │                │
     └──── Failover ──┘
```

---

## Planned Failure Testing

The project will deliberately introduce controlled failures including:

- malformed detection rules,
- parser crashes,
- stalled telemetry consumers,
- overloaded event queues,
- failed health signals,
- unavailable downstream dependencies,
- excessive retry behavior,
- partial fleet degradation,
- failed canary deployments,
- unavailable primary capacity,
- failover-path failures.

## Performance Benchmarking

The platform will produce reproducible measurements for:

- events processed per second,
- p50 / p95 / p99 processing latency,
- CPU utilization,
- memory utilization,
- queue depth,
- dropped-event rate,
- recovery time,
- detection-engine availability,
- canary health,
- rollout failure rate,
- failover success rate.

---

## Technology Stack

### Endpoint and Detection Engine

- C#
- .NET 10
- Windows ETW
- Windows Event Log
- ASP.NET Core
- xUnit

### Automation

- PowerShell
- GitHub Actions
- GitHub Copilot CLI

### Cloud and Observability

- Microsoft Azure
- Azure Monitor
- Application Insights
- Azure Storage
- Azure Key Vault
- Managed Identity

### Developer and Security Interface

- TypeScript
- React
- Node.js

### Security Tooling

- Trivy
- Gitleaks
- dependency vulnerability scanning
- static analysis
- container security scanning

### Research Track

- eBPF for Windows
- VBS-backed protection of critical trust state
- high-throughput user-mode telemetry
- safe dynamic detection-rule execution

---

### Implemented

- FastAPI security demonstration service
- automated Python tests
- Bandit static analysis
- `pip-audit` dependency scanning
- Gitleaks secret detection
- Docker containerization
- non-root container runtime
- multi-stage Docker builds
- Trivy container vulnerability scanning
- GitHub Actions security gates
- JSON security reports

