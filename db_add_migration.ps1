dotnet ef migrations add "$(Get-Date -Format yyyyMMddHHmmss)" `
  --project ./ShopOnline.Infrastructure/ShopOnline.Infrastructure.csproj `
  --startup-project ./WebApp/WebApp/WebApp.csproj
Read-Host "Press Enter to exit"
# InitialCreate  ||| $(Get-Date -Format yyyyMMddHHmmss)