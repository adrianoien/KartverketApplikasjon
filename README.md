For å koble til med MariaDB så må man gjøre følgende: 

Gå i en terminal og naviger til prosjektmappen.
Tips: Kopier pathen ifra direkte hvis man høyreklikker på solutions filen inne i Visual Studio

Skriv: 
cd path/to/your/project
Feks: 
C:\Users\user\source\repos\Kartverket\KartverketApplikasjon\

Start opp containereren
docker-compose up -d

Sjekk om den kjører
docker ps

Koble til MariaDB gjennom docker
docker exec -it mariadb mariadb -u root -p

Enkle kommandoer for å se om den er koblet til: 
SHOW DATABASES;
SHOW TABLES;
