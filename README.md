# sms-dotnet-office-dashboard

A program collecting data from different sources and saving them in an InfluxDB in order to display them using grafana. It's a .NET Core application and can thus be run on Windows, iOS and Linux machines. Credentials and further settings can be provided using the settings.json file. Please make sure it's located in the same folder as the complied and executable program.

# Prerequisites

The application requires the .NET Core runtime to be installed on your machine in order to run the program. If you want to compile the program on the same machine as well, you also need the .NET Core SDK. Both can be obtained from https://dotnet.microsoft.com/download. This application is based on .NET Core 2.2 and requires at least this version of the runtime.
Since the database and grafana are dockerized, you also need a Docker environment on your system in order to have the backend running correctly. After installing Docker, please use `docker-compose up -d` to start the backend according to the docker-compose file.
