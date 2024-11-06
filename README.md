For å koble til med MariaDB så må man gjøre følgende: 

Når du kompilerer fila i Visual Studio, så gjør det igjennom docker compose. 
 - Trykk på menyen hvor du builder, gå inn på "Configure startup projects"
 - Velg docker-compose

Nå skal applikasjonen være koblet til databasen


Koble til MariaDB i MySQL Client:
exec -it mariadb mariadb -u root geochangesdb -p

Enkle kommandoer for å se om den er koblet til: 
SHOW DATABASES;
SHOW TABLES;
