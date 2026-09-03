## Tickets booking App

Current app is a fullstack application with database, api server & frontend client served with docker-compose file.

### Stack:

1. Angular
2. ASP.NET Core
3. Entity Framework Core
4. PostgreSQL
5. Docker Compose

Tests run: 

```
dotnet test backend/tests/EventBooking.Api.UnitTests/EventBooking.Api.UnitTests.csproj
```

## DataModel

### Events

| Column         | Type     |
| -------------- | -------- |
| Id             | int      |
| Title          | String   |
| Description    | String   |
| EventDate      | DateTime |
| TotalSeats     | int      |
| AvailableSeats | int      |
| CreatedAt      | DateTime |

### Bookings

| Column    | Type               |
| --------- | ------------------ |
| Id        | int                |
| EventId   | int?, FK -> Events |
| FirstName | String             |
| LastName  | String             |
| CreatedAt | DateTime           |

### AuditLogs

| Column     | Type     |
| ---------- | -------- |
| Id         | int      |
| EntityName | String   |
| EntityId   | int      |
| Action     | String   |
| Details    | String   |
| CreatedAt  | DateTime |

## API Endpoints

| API                           | Description                                                       | Returns            |
| ----------------------------- | ----------------------------------------------------------------- | ------------------ |
| GET /api/events               | Get list of events                                                | List < Event >    |
| POST /api/events              | Add Event { Title, Description EventDate, TotalSeats }          | Event              |
| PUT /api/events/{id}          | Update event by id { Title, Description EventDate, TotalSeats } | Event              |
| DELETE /api/events/{id}       | Remove event by id                                                | Event              |
| GET /api/events/{id}          | Get event details                                                | Event              |
| POST /api/bookings/{event_id} | Create booking { firstName, lastName }                           | Booking            |
| GET /api/bookings/{event_id} | Get Event Bookings                                               | List< Booking >    |
| GET /api/audit-logs           | Get audit-logs                                                    | List < AuditLog > |

### App creation steps

Create api project template

```
dotnet new webapi -o src/EventBooking.Api
```

Trust certificate & add SwaggerUI from NuGet for testing purposes

```
dotnet dev-certs https --trust
```

Add packages to work with db

```
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
```

Then create init migration & update local db

```
dotnet ef migrations add init
dotnet ef database update
```
