# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy only csproj and restore
COPY StudentCourseManagement/StudentCourseManagement.csproj ./StudentCourseManagement/
RUN dotnet restore ./StudentCourseManagement/StudentCourseManagement.csproj

# Copy the full backend source code
COPY StudentCourseManagement ./StudentCourseManagement

# Publish
WORKDIR /app/StudentCourseManagement
RUN dotnet publish -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./

# Expose default port
EXPOSE 8080

# Start the app
ENTRYPOINT ["dotnet", "StudentCourseManagement.dll"]