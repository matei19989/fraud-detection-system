# Fraud Detection System

🚧 **Under Active Development** - Expected Completion: December 2025

## Overview

Real-time fraud detection system built with event-driven architecture, leveraging CQRS, DDD, and Clean Architecture principles.

## Tech Stack

**Backend:**
- .NET Core 8
- CQRS with MediatR
- Domain-Driven Design (DDD)
- Entity Framework Core
- Apache Kafka
- SignalR

**Frontend:**
- React 18 + TypeScript
- Vite
- React Router
- Tailwind CSS
- Axios
- SignalR Client

**Infrastructure:**
- Azure (Container Apps, SQL Database, Event Hubs)
- Docker

## Architecture

Clean Architecture with clear separation of concerns:

- **Domain Layer**: Core business entities, value objects, domain events
- **Application Layer**: Use cases, CQRS command/query handlers
- **Infrastructure Layer**: External concerns (database, messaging, external APIs)
- **API Layer**: REST endpoints, SignalR hubs
- **Frontend**: React-based dashboard for monitoring

## Project Structure
```
FraudDetectionSystem/
├── FraudDetection.Domain/          # Core business logic
├── FraudDetection.Application/     # Use cases & handlers
├── FraudDetection.Infrastructure/  # External integrations
├── FraudDetection.API/             # Web API
├── FraudDetection.Contracts/       # DTOs & contracts
└── fraud-detection-ui/             # React frontend
```

## Features (Planned)

- ⚡ Real-time transaction monitoring
- 🔍 Configurable fraud detection rules engine
- 🤖 Machine learning-based fraud scoring
- 🚨 Live alerts via SignalR WebSockets
- 📊 Admin dashboard for rule management
- 📈 Analytics and reporting

## Development Roadmap

- [x] Project scaffolding and architecture setup
- [ ] Domain model implementation (November 2025)
- [ ] CQRS command/query handlers (November 2025)
- [ ] Fraud detection engine and rules (November 2025)
- [ ] Kafka event streaming integration (November 2025)
- [ ] Frontend dashboard UI (December 2025)
- [ ] Real-time alerts with SignalR (December 2025)
- [ ] Azure deployment and CI/CD (December 2025)

## Getting Started

*Full setup instructions will be added upon project completion.*

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- SQL Server / PostgreSQL
- Apache Kafka (optional for local dev)

### Quick Start

**Backend:**
```bash
cd FraudDetection.API
dotnet run
```

**Frontend:**
```bash
cd fraud-detection-ui
npm install
npm run dev
```

## Status

🚧 **In Development** - Core architecture implemented, features being built through Q4 2025.

## Author

**Matei Cretu**
- LinkedIn: [matei-cretu](https://www.linkedin.com/in/matei-cretu-355a9919b/)
- GitHub: [matei19989](https://github.com/matei19989)
- Codeforces: Expert (Rating 1400)

## License

MIT License - feel free to use this project for learning purposes.