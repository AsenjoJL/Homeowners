# Quick Fix: Notification Primary Key Error

## Issue
The `Notification` model is empty (no properties), causing EF Core to require a primary key definition.

## Fix Applied
Added `HasNoKey()` configuration for Notification in `ApplicationDbContext.OnModelCreating()`:

```csharp
modelBuilder.Entity<Notification>()
    .HasNoKey(); // Mark as keyless since it's empty
```

## Next Steps
The AdminController still needs to be fully migrated from `ApplicationDbContext` to `IDataService`. This is a larger task that will be done incrementally.

For now, the Notification fix should allow the app to start, but AdminController methods that still use `_context` will need to be updated.

