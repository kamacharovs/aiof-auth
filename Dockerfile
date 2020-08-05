FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY /app/publish/aiof.auth.core /app/
EXPOSE 80
ENTRYPOINT ["dotnet", "aiof.auth.core.dll"]