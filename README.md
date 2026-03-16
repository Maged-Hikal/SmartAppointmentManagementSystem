# Smart Appointment Management System 📅

A modern, scalable appointment management system built with .NET 8, Blazor WebAssembly, and Entity Framework Core, enhanced with AI-powered intelligent automation. This system provides comprehensive appointment scheduling capabilities with role-based access control, real-time updates, smart recommendations, and a responsive web interface.

## 🌟 Features

### Core Functionality
- **Appointment Management**: Create, schedule, reschedule, approve, reject, and cancel appointments
- **User Authentication & Authorization**: JWT-based authentication with role-based permissions
- **Real-time Updates**: Live appointment status tracking and notifications
- **Multi-tenant Architecture**: Clean separation of concerns with Domain-Driven Design
- **Responsive UI**: Modern Blazor WebAssembly interface compatible with all devices

### Advanced Features
- **AI-Powered Intelligence**: Smart appointment recommendations and predictive scheduling
- **Health Monitoring**: Built-in health checks for system reliability
- **Rate Limiting**: API protection against abuse (60 requests/minute)
- **Security Headers**: Comprehensive security implementation with CSP, CORS, and XSS protection
- **Data Validation**: Robust server-side and client-side validation
- **Audit Trail**: Complete tracking of appointment status changes

### 🤖 AI Integration Features
- **Smart Scheduling**: AI-powered optimal time slot recommendations
- **Predictive Analytics**: Forecast appointment demand and optimize resource allocation
- **Automated Reminders**: Intelligent notification system with optimal timing
- **Natural Language Processing**: Parse appointment requests from natural language
- **Customer Insights**: AI-driven analytics for appointment patterns and preferences

## 🏗️ Architecture

This system follows **Clean Architecture** principles with clear separation of concerns:

```
SmartAppointmentSystem/
├── SmartAppointment.Domain/          # Core business logic and entities
├── SmartAppointment.Application/     # Application services and interfaces
├── SmartAppointment.Infrastructure/  # Data access and external services
├── SmartAppointment.API/            # Web API endpoints and authentication
├── SmartAppointment.Blazor/         # WebAssembly frontend
└── SmartAppointment.Tests/          # Comprehensive unit test suite
```

### Technology Stack

#### Backend
- **.NET 8** - Latest .NET framework with performance improvements
- **ASP.NET Core Web API** - RESTful API with Swagger documentation
- **Entity Framework Core** - ORM with SQL Server integration
- **ASP.NET Core Identity** - User management and authentication
- **JWT Authentication** - Secure token-based authentication
- **AutoMapper** - Object-to-object mapping

#### Frontend
- **Blazor WebAssembly** - Modern client-side web framework
- **Blazored.LocalStorage** - Client-side storage management
- **HttpClient Factory** - Optimized HTTP client management

#### Development & Testing
- **xUnit** - Testing framework
- **Moq** - Mocking framework for unit tests
- **FluentAssertions** - Readable test assertions
- **Swagger/OpenAPI** - API documentation
- **Health Checks** - System monitoring

## 🚀 Quick Start

### Prerequisites
- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** - Local or SQL Server Express
- **Visual Studio 2022** or **VS Code** with C# extension

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/Maged-Hikal/SmartAppointmentManagementSystem.git
   cd SmartAppointmentManagementSystem
   ```

2. **Configure User Secrets**
   ```bash
   cd SmartAppointment.API
   dotnet user-secrets set "Jwt:Key" "Generate-your-32-character-secret-key-here"
   dotnet user-secrets set "Jwt:Issuer" "your-issuer"
   dotnet user-secrets set "Jwt:Audience" "your-audience"
   ```

3. **Update Database Connection**
   Edit `appsettings.json` in `SmartAppointment.API`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=your-server;Database=SmartAppointmentDB;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Run Database Migrations**
   ```bash
   cd SmartAppointment.API
   dotnet ef database update
   ```

5. **Run the Application**
   ```bash
   # Start API (in terminal 1)
   cd SmartAppointment.API
   dotnet run

   # Start Blazor App (in terminal 2)
   cd SmartAppointment.Blazor
   dotnet run
   ```

6. **Access the Application**
   - **Blazor Frontend**: https://localhost:7161
   - **API Documentation**: https://localhost:7216/swagger
   - **Health Check**: https://localhost:7216/health

## 📊 System Status & Monitoring

### Health Checks
The system includes comprehensive health monitoring:
- **JWT Configuration Check**: Validates security settings
- **Database Connectivity**: Ensures SQL Server connection
- **Endpoint**: `/health` returns system status

### Performance Features
- **Rate Limiting**: 60 requests per minute per client
- **CORS Configuration**: Secure cross-origin resource sharing
- **Security Headers**: Protection against XSS, clickjacking, and other attacks

## 🧪 Testing

This project includes a **comprehensive test suite** with **101 tests** covering all major components:

### Test Coverage Areas
- **Domain Entities**: Business logic validation
- **Application Services**: CRUD operations and business rules
- **DTO Validation**: Input validation and error handling
- **Controllers**: API endpoint testing
- **User Registration**: Authentication and authorization flows

### Running Tests
```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run specific test categories
dotnet test --filter "FullyQualifiedName~AppointmentTests"
dotnet test --filter "FullyQualifiedName~RegisterModelTests"
```

### Test Statistics
- **Total Tests**: 101
- **Test Framework**: xUnit with Moq and FluentAssertions
- **Coverage**: Domain, Application, Infrastructure, API layers
- **All Tests**: ✅ Passing

## 🔧 Configuration

### Environment Variables
Configure for different deployment environments:

#### Development
```bash
API_BASE_URL=https://localhost:7216
BLAZOR_URL=https://localhost:7161
```

#### Production
```bash
API_BASE_URL=https://api.yourdomain.com
BLAZOR_URL=https://yourdomain.com
```

### Docker Deployment
```bash
docker build -t smartappointment-system .
docker run -e API_BASE_URL=https://api.yourdomain.com -e BLAZOR_URL=https://yourdomain.com smartappointment-system
```

## 📱 API Documentation

### Authentication
All API endpoints require JWT authentication (except registration and login).

### Key Endpoints
- `POST /api/auth/login` - User authentication
- `POST /api/auth/register` - User registration
- `GET /api/appointments` - Get user appointments
- `POST /api/appointments` - Create new appointment
- `PUT /api/appointments/{id}/approve` - Approve appointment
- `PUT /api/appointments/{id}/reject` - Reject appointment
- `PUT /api/appointments/{id}/reschedule` - Reschedule appointment

### Full API Documentation
Visit https://localhost:7216/swagger for interactive API documentation.

## 🔒 Security Features

### Authentication & Authorization
- **JWT Tokens**: Secure token-based authentication
- **Role-Based Access Control**: Granular permission system
- **Password Policies**: Secure password requirements
- **Token Expiration**: Automatic token refresh

### Security Headers
- **Content Security Policy**: Prevents XSS attacks
- **X-Frame-Options**: Prevents clickjacking
- **X-Content-Type-Options**: Prevents MIME sniffing
- **Referrer Policy**: Controls referrer information

### Data Protection
- **Input Validation**: Comprehensive server-side validation
- **SQL Injection Protection**: Entity Framework parameterization
- **Rate Limiting**: API abuse prevention
- **HTTPS Enforcement**: Secure communication

## 🌐 Deployment

### Azure App Service
1. Create Azure App Service and SQL Database
2. Configure application settings for connection strings and JWT settings
3. Deploy using Visual Studio or Azure CLI

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SmartAppointment.API/SmartAppointment.API.csproj", "SmartAppointment.API/"]
RUN dotnet restore "SmartAppointment.API/SmartAppointment.API.csproj"
COPY . .
WORKDIR "/src/SmartAppointment.API"
RUN dotnet build "SmartAppointment.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartAppointment.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartAppointment.API.dll"]
```

## 📈 Performance & Scalability

### Optimizations
- **Async/Await**: Non-blocking operations throughout
- **Entity Framework**: Optimized queries and change tracking
- **HttpClient Factory**: Efficient HTTP connection management
- **Lazy Loading**: Optimized data loading strategies

### Monitoring
- **Health Checks**: Real-time system status
- **Logging**: Comprehensive error and activity logging
- **Performance Counters**: Built-in performance monitoring

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines
- Follow Clean Architecture principles
- Write unit tests for new features
- Ensure all tests pass before submitting
- Follow C# coding conventions
- Update documentation for API changes

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Maged Hikal**  
*Full Stack .NET Developer*  
*Specializing in modern web applications with .NET, Blazor, and cloud technologies*

📧 **Email**: maged.hikal@yahoo.com  
🔗 **LinkedIn**: [linkedin.com/in/maged-hikal](https://www.linkedin.com/in/maged-hikal)
🐙 **GitHub**: [github.com/Maged-Hikal](https://[github.com/Maged-Hikal](https://github.com/Maged-Hikal))

## 🙏 Acknowledgments

- Microsoft for the excellent .NET ecosystem
- The Blazor team for the amazing web framework
- Entity Framework team for the powerful ORM
- Open source community for the valuable tools and libraries

---

⭐ **Star this repository if it helped you!**  
🔄 **Follow for updates and new features**  
📢 **Share with your network**

**Built with ❤️ using .NET 8 and modern web technologies**

## Projects ScreenShot


