# Strangler Pattern

This solution demonstrates the **Strangler Pattern** using .NET technologies, including **.NET Aspire**, two web APIs, and **YARP** (Yet Another Reverse Proxy). The Strangler
Pattern is a useful software migration strategy for incrementally replacing an old system with a new one, ensuring that the old system continues to function while the new system is
introduced.

## Solution Overview

The solution consists of the following components:

- **Web API**: Represents the new system that will eventually replace `Old API`.
- **Old API**: Represents the legacy system that is gradually being replaced.
- **YARP**: A reverse proxy that routes traffic between the two APIs based on configuration, allowing us to control how much of the old system is replaced by the new system.

## Technologies

- **.NET 9**
- **.NET Aspire** for modern web development practices
- **YARP** (Yet Another Reverse Proxy) for routing requests
- **ASP.NET Core** for building the web APIs

## Links

 - [Web API](http://localhost:5510/scalar/) ([Swagger](http://localhost:5510/swagger/index.html))
 - [Old API](http://localhost:5122/scalar/) ([Swagger](http://localhost:5122/swagger/index.html))