C:\Program Files\Docker\docker.exe -SwitchLinuxEngine
docker-compose -f docker-compose.regtest.yml down
docker-compose -f docker-compose.regtest.yml up --force-recreate --build
pause
