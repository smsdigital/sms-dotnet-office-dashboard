# sms-dotnet-office-dashboard

A program collecting data from different sources and saving them in an InfluxDB in order to display them using grafana. It's a .NET Core application and can thus be run on Windows, iOS and Linux machines. Credentials and further settings can be provided using the settings.json file. Please make sure it's located in the same folder as the complied and executable program.

# Prerequisites

The application requires the .NET Core runtime to be installed on your machine in order to run the program. If you want to compile the program on the same machine as well, you also need the .NET Core SDK. Both can be obtained from https://dotnet.microsoft.com/download. This application is based on .NET Core 2.2 and requires at least this version of the runtime.
Since the database and grafana are dockerized, you also need a Docker environment on your system in order to have the backend running correctly. After installing Docker, please use `docker-compose up -d` to start the backend according to the docker-compose file.

# Additional configuration

Files for additional configuration ar elocated in the `config` folder.

* **power_on.sh** and **power_off.sh**: Those files are used to send CEC commands to the connected TV in order to turn it on and off and select the correct input source. Place them in /home/pi/Documents/. If you place them somewhere else, please keep in mind to update the crontab file respectively.
* **crontab.example**: Two exemplary entries in order to turn the TV on at 7:30 am and turn it off at 5:00 pm from Monday to Friday each week.
* **autostart.sh**: This file contains commands to be executed on system startup. It starts the data collector, reruns the Docker containers and starts a browser window in full screen mode showing the grafana dashboard. The docker-compose-yml is located in /opt/DataCollector - together with the collector application. If you place the application and/or the docker-compose file somewhere else, please edit the path respectively. Also, the commands require Chromium to be installed and grafana to use port 3000. Changing those settings also require changing the file.
* **autostart**: That file contains startup commands for the LXDE desktop. They disable screen blanking and execute the `autostart.sh` in the home folder. Please adapt the path if you place the `autostart.sh` somewhere else.
