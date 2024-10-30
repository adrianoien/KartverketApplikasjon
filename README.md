For å koble til med MariaDB så må man gjøre følgende: 

# Naviger til prosjektmappen
cd path/to/your/project
Feks: 
C:\Users\user\source\repos\Kartverket\KartverketApplikasjon\

# Start opp containereren
docker-compose up -d

# Sjekk om den kjører
docker ps

#Koble til MariaDB gjennom docker
docker exec -it mariadb mysql -u root -p

# Enkle kommandoer for å se om den er koblet til: 
SHOW DATABASES;
SHOW TABLES;
