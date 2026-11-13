# Endpoint Security Engine

Microsoft reports processing more than 165 trillion security signals every day. At hyperscale, security infrastructure must do more than detect threats it must remain operational when components, health systems, dependencies, and deployments fail.

This project investigates how Windows endpoint security infrastructure can contain failures before they propagate into fleet-wide outages.

---

## Objective

Build and benchmark a resilient Windows security architecture designed to prevent:

- faulty health decisions from unnecessarily removing healthy capacity,
- unsafe security changes from propagating across an endpoint fleet,
- shared failure domains from defeating redundant systems,
- parser or detection failures from destabilizing the host,
- retry, restart, and failover logic from amplifying an existing outage.

The system is being designed around fault isolation, independent health paths, health-gated rollout, rollback, graceful degradation, and redundant failover.

---

## Core Engineering Problem

A resilient security platform should not allow one failure to become thousands.

```text
Fault
  ↓
Detect
  ↓
Contain
  ↓
Degrade Safely
  ↓
Recover / Fail Over
  ↓
Preserve Fleet Availability
```

The central question of this project is:

> **Can endpoint security continue detecting threats while ensuring that failures in telemetry, detection logic, health monitoring, deployment, or recovery remain contained?**

---

## Target Architecture

```text
                    WINDOWS ENDPOINT

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

## Resilience Model

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

### Last-Known-Good Recovery

Failed or unhealthy changes should revert to a previously validated configuration rather than leaving endpoints in an unknown state.

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

The goal is not to prove that failures never happen.

The goal is to prove that:

> **one failure stays one failure.**

---

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

Performance claims will be based on measured results rather than theoretical estimates.

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

## .NET Architecture

The detection platform is intentionally separated from Windows-specific telemetry.

```text
SecurityAutomation.Core
        ↑
        │
SecurityAutomation.Windows
```

### `SecurityAutomation.Core`

Responsible for:

- common event models,
- detection logic,
- rule evaluation,
- event normalization,
- enrichment,
- health models,
- failure classification.

The core engine must not depend directly on Windows telemetry APIs.

### `SecurityAutomation.Windows`

Responsible for:

- ETW collection,
- Windows Event Log ingestion,
- Win32 integrations,
- translating Windows telemetry into the common event model.

This allows telemetry sources to evolve without requiring the detection engine to be rewritten.

---

## Existing Security Foundation

The repository began as a DevSecOps security pipeline and already contains working security controls that will be incorporated into the larger platform.

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

These components remain useful as examples of build-time security automation while the repository evolves toward the Windows/.NET resilience architecture.

---

## Project Roadmap

### Phase 1 — .NET Core Architecture

- Create the .NET solution
- Define the common security-event model
- Separate platform logic from Windows telemetry
- Build initial detection interfaces
- Establish automated tests

### Phase 2 — Windows Telemetry

- Integrate ETW
- Consume process and system telemetry
- Normalize Windows events
- Build synthetic event generators
- Benchmark event throughput

### Phase 3 — Detection Engine

- Implement rule processing
- Add security-event enrichment
- Introduce strict rule validation
- Add resource limits
- Implement last-known-good rule recovery

### Phase 4 — Health and Failure Containment

- Separate liveness and readiness signals
- Build endpoint health models
- Introduce circuit breakers
- Implement backpressure and load shedding
- Test dependency failures
- Test process failures

### Phase 5 — Fleet Safety

- Build fleet-health aggregation
- Implement health-gated canary rollout
- Stop unhealthy deployments automatically
- Implement rollback
- Introduce redundant failure domains
- Test automated failover

### Phase 6 — Azure Integration

- Send health and security telemetry to Azure
- Add Azure observability
- Protect sensitive state with managed identity and Key Vault
- Build fleet-level metrics
- Implement distributed health monitoring

### Phase 7 — Security Operations Interface

- Build a TypeScript/React dashboard
- Display endpoint health
- Display security findings
- Visualize rollout state
- Visualize failure-domain health
- Show performance benchmarks

### Phase 8 — Adversarial and Resilience Testing

- malformed-update testing,
- parser crash testing,
- dependency-outage testing,
- retry-storm testing,
- health-check failure testing,
- canary failure testing,
- failover testing,
- sustained high-volume telemetry testing.

---

## Current Status

The original DevSecOps security foundation is complete through container vulnerability scanning.

Current development is focused on the **C#/.NET platform foundation and Windows telemetry architecture**.

---

## Research Thesis

Modern security infrastructure must be designed under the assumption that components will fail.

The engineering challenge is therefore not simply:

**Can the platform detect threats?**

It is:

**Can the platform detect threats while preventing its own failures, recovery mechanisms, and deployment systems from becoming sources of fleet-wide instability?**

---

## Disclaimer

This repository is intended exclusively for defensive security engineering, authorized testing, systems research, and controlled resilience experimentation.

All failure injection and security testing should be performed only in systems explicitly authorized for testing.

---

## License

This project is licensed under the MIT License.