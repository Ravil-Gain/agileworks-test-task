## Tickets booking App

Current app is a fullstack application with database, api server & frontend client served with docker-compose file.

### Stack:

1. Angular
2. ASP.NET Core
3. Entity Framework Core
4. PostgreSQL
5. Docker Compose

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
| EventId   | Guid, FK -> Events |
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


| API                            | Description                                                      | Returns            |
| ------------------------------ | ---------------------------------------------------------------- | ------------------ |
| GET /api/events                | Get list of events                                               | List < Event >    |
| POST /api/events               | Add Event { Name, Description EventDate, TotalSeats }          | Event              |
| PUT /api/events/{id}           | Update event by id { Name, Description EventDate, TotalSeats } | Event              |
| DELETE /api/events/{id}        | Remove event by id                                               | boolean            |
| GET /api/events/{id}           | Get event details                                               | Event              |
| POST /api/events/{id}/bookings | Create booking { firstName, lastName }                          | boolean            |
| GET /api/admin/audit-logs      | Get audit-logs                                                   | List < AuditLog > |

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
