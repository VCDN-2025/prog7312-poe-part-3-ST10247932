MunicipalReporter

A low-bandwidth municipal services web application

Overview

MunicipalReporter is a web application (ASP.NET Core + MVC) designed for municipal services: allowing users (citizens) to report municipal issues, view status, enable local events, and for administrators/municipal staff to manage service requests and events.
The solution file is MunicipalReporter.sln.

Getting Started
These instructions will help you get a copy of the project up and running on your local machine for development and testing.

Prerequisites

Make sure you have installed:
.NET SDK
 (version appropriate for project; e.g., .NET 6 or .NET 7)
Visual Studio 2022 (or later) / VS Code (with C# extension)

Clone the repository
git clone https://github.com/VCDN-2025/prog7312-poe-part-3-ST10247932.git
cd prog7312-poe-part-3-ST10247932
Open the solution
Open MunicipalReporter.sln in Visual Studio (or dotnet CLI)
Restore dependencies
Build the project
In Visual Studio: Build → Build Solution
Or via CLI:
dotnet build
Run the application
Start the application either via Visual Studio (F5) or CLI:
dotnet run --project MunicipalReporter
Once running, browse to the URL printed in the console (e.g., https://localhost:5001)
Using the application

The home page allows users to navigate key functions:

Home — landing page
Report Issues — submit new municipal service issues
Events — view local events
Status — check status of service requests
Community Chat — a chat feature (login required)

Authentication flows:

Unauthenticated users can view home, events, report new issues (if permitted)
Authenticated users (session stored) can see “Welcome, {Username}!”, logout, etc.
Administrators/Staff: you will need to seed or create a user with elevated role (if application includes roles) to manage issues/events etc.

Navigation / UI:

The navbar uses custom colours: a gradient background, brand/nav-link colour customisation.
The footer includes contact/branding details.
Low bandwidth optimisation: Designed for performance in constrained network environments.

Configuration & Deployment

Modify appsettings.Production.json for production environment settings (logging, connection strings, environment variables).
Use ASPNETCORE_ENVIRONMENT=Production or relevant environment variable to configure environment behaviour.
Ensure HTTPS is enforced, correct domain binding, certificate set up in production.
If using a cloud deployment (Azure, AWS, etc), update the connection strings and make sure migrations are applied on startup or via CI/CD pipeline.
Consider enabling logging and error-monitoring (e.g., Application Insights, Sentry) for production.

For low-bandwidth deployment:

Enable response compression (gzip/brotli)
Minify CSS & JS
Use caching headers (static assets)
Consider serving statics from CDN

Project Structure

Typical folders in the solution:
Controllers/ – MVC controllers for issues, events, status, chat, authentication
Views/ – Razor views
Models/ – Domain and view-models
Data/ – DbContext, migrations, seed-data
wwwroot/ – static files (CSS, JS, images)
Site.css – custom stylesheet (see head section for styles)
TemplateEngineHost/ – possibly unused or scaffolding folder


Manual test scenario:

Launch the application locally.
Register a new user (not necessary).
Log in, submit a new issue via “Report Issues”.
Navigate to “Status” and verify the issue appears and status updates are possible.
Navigate to “Events” page and verify listing, filtering, etc.
Try “Community Chat” link, ensure login required, chat sends/receives messages (if implemented).
Inspect responsiveness and load times in low-bandwidth simulation (Network Throttling via browser dev tools).

Known Issues & Caveats
Authentication: ensure the session/cookie configuration works in your environment (especially if using distributed architecture).
Database: if running with localdb vs full SQL Server, adjust the connection string accordingly.
Migrations: if missing, you will need to scaffold or create tables manually.
Chat/Real-time: if using SignalR or websockets, ensure the hosting environment supports it (e.g., Azure Web Apps vs IIS).
Deployment: check static file caching, bundle minification & security headers (CSP, HSTS).
Low bandwidth features: verify that image sizes, asset sizes, and network requests are optimised.

Contributing

Fork the repository.
Create your feature branch (git checkout -b feature/MyFeature).
Commit your changes (git commit -m "Add new feature").
Push to the branch (git push origin feature/MyFeature).
Submit a Pull Request via GitHub.
Ensure code is reviewed and passes build/tests before merging.

Author: Preshen Pillay ST10247932
YouTube Demo Vid: https://youtu.be/ES43CfuMam8



