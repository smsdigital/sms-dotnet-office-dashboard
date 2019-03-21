screen /opt/DataCollector/DataCollector -d
docker-compose -f /opt/DataCollector/docker-compose.yml up -d
sleep 20
chromium-browser http://localhost:3000 --start-fullscreen
