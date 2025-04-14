dotnet ef migrations add create_db --project server --context AdminAppDbContext
dotnet ef database update --project server --context AdminAppDbContext