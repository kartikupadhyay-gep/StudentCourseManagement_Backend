# Use the official .NET SDK image to build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY StudentCourseManagement/StudentCourseManagement.csproj ./StudentCourseManagement/
RUN dotnet restore StudentCourseManagement/StudentCourseManagement.csproj

# Copy the rest of the source code
COPY StudentCourseManagement/. ./StudentCourseManagement/
WORKDIR /src/StudentCourseManagement

# Build and publish the app
RUN dotnet publish -c Release -o /app --no-restore

# Use the official ASP.NET runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .

# Expose port 80 and 443
EXPOSE 80
EXPOSE 443

# Set environment variables for ASP.NET
ENV ASPNETCORE_URLS=http://+:80

# Start the application
ENTRYPOINT ["dotnet", "StudentCourseManagement.dll"]
