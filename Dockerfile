FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /app
COPY /app/publish/aiof.auth.core /app/
EXPOSE 80
ENTRYPOINT ["dotnet", "aiof.auth.core.dll"]