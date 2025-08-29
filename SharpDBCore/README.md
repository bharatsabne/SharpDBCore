# 📘 SharpDBCore – Usage Guide

A lightweight, reusable ADO.NET database access library using raw SQL and stored procedures. Designed with SOLID principles and support for logging, transactions, parameterized queries, and command categorization.

---

## 🔧 1. Setup Connection

Before using the library, initialize your connection:

```csharp
DbConnectionFactory.SetConnectionString("Server=.;Database=YourDb;Trusted_Connection=True;");
```
Or pass directly:
```csharp
var connection = DbConnectionFactory.CreateConnection("your-connection-string");
```
✅ Recommended: Set the connection string once on app startup for reuse.

## ⚙️ 2. Create the Database Manager
Use DatabaseManager to perform all DB operations:

```csharp
var dbManager = new DatabaseManager(DbConnectionFactory.CreateConnection());
```
Set an optional logger:
```dbManager.SetLogger(new ConsoleLogger());
dbManager.EnableDbLogging = true; // Logs to DB table if implemented
```

## 🧾 3. Execute Commands

🔹 ExecuteNonQuery
```sharp
var result = dbManager.ExecuteNonQuery(
    "INSERT INTO Books (Title) VALUES (@Title)",
    new Dictionary<string, object> { { "Title", "My Book" } },
    CommandType.Text,
    CommandCategory.Insert
);
```

🔹 ExecuteScalar
```sharp
var count = dbManager.ExecuteScalar("SELECT COUNT(*) FROM Books");
```

🔹 ExecuteReader
```sharp
using var reader = dbManager.ExecuteReader("SELECT * FROM Books");
while (reader.Read())
{
    var title = reader["Title"].ToString();
}
```

🔹 ExecuteDataTable
```sharp
var table = dbManager.ExecuteDataTable("SELECT * FROM Books");
```

## 🔁 4. Transaction Handling
```sharp
dbManager.BeginTransaction();

try
{
    dbManager.ExecuteNonQuery("UPDATE Books SET Title = @Title WHERE Id = @Id",
        new Dictionary<string, object> { { "Title", "New Title" }, { "Id", 1 } });

    dbManager.CommitTransaction();
}
catch
{
    dbManager.RollbackTransaction();
    throw;
}
```

## 🧹 5. Cleaning Old Logs
```sharp
dbManager.DeleteOldLogs();      // Deletes logs older than 1 month (default)
dbManager.DeleteOldLogs(3);     // Deletes logs older than 3 months
```

## 📋 6. Command Categories

The CommandCategory enum is used for classifying database actions:

| Value           | Description                    |
|-----------------|-------------------------------|
| Unknown         | Default or unspecified         |
| Select          | Read operations               |
| Insert          | Insert operations             |
| Update          | Update operations             |
| Delete          | Delete operations             |
| StoredProcedure | Procedure execution           |
| Maintenance     | Backup, cleanup, indexing, etc. |
| Utility         | Health check, versioning, etc. |
| Security        | Permissions, users, roles?    |

## 🧪 7. Example: Full Usage Flow
```sharp
DbConnectionFactory.SetConnectionString("Server=.;Database=MyDb;Trusted_Connection=True;");
var db = new DatabaseManager(DbConnectionFactory.CreateConnection());

db.SetLogger(new ConsoleLogger());
db.EnableDbLogging = true;

var parameters = new Dictionary<string, object>
{
    { "BookId", 1 },
    { "UserId", 42 },
    { "Rating", 5 },
    { "RatedOn", DateTime.UtcNow }
};

db.BeginTransaction();

try
{
    db.ExecuteNonQuery(
        "INSERT INTO BookRatings (BookId, UserId, Rating, RatedOn) VALUES (@BookId, @UserId, @Rating, @RatedOn)",
        parameters,
        CommandType.Text,
        CommandCategory.Insert
    );

    db.CommitTransaction();
}
catch
{
    db.RollbackTransaction();
    throw;
}
```

## ❓ Need Help?
Open an issue or reach out if you’d like to contribute or request features!
