docker-compose -f /opt/DataCollector/docker-compose.yml up -d
sleep 10
screen /opt/DataCollector/DataCollector -d
sleep 10
chromium-browser http://localhost:3000 --start-fullscreen
