## Tickets booking App

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
| Id             | Guid     |
| Name           | String   |
| Description    | String   |
| EventDate      | DateTime |
| TotalSeats     | int      |
| AvailableSeats | int      |
| CreatedAt      | DateTime |

### Bookings


| Column    | Type               |
| --------- | ------------------ |
| Id        | Guid               |
| EventId   | Guid, FK -> Events |
| FirstName | String             |
| LastName  | String             |
| CreatedAt | DateTime           |

### AuditLogs


| Column     | Type     |
| ---------- | -------- |
| Id         | Guid     |
| EntityName | String   |
| EntityId   | String   |
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
| GET /api/admin/events/{id}     | Get event details                                               | Event              |
| POST /api/events/{id}/bookings | Create booking { firstName, lastName }                          | boolean            |
| GET /api/admin/audit-logs      | Get audit-logs                                                   | List < AuditLog > |
