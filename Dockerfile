FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /app
COPY /app/publish/aiof.auth.core /app/
EXPOSE 80
ENTRYPOINT ["dotnet", "aiof.auth.core.dll"]