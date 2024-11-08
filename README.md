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


You’ll want to include the following sections in your documentation:

Application Setup (Architecture): Document the overall structure of your application. Explain the architecture, such as the MVC pattern, database setup, and any integrations.

How to Run the Application: Describe the steps to run the application, including Docker setup, connecting to the database, and any prerequisites.

Application Components: Explain the key components (e.g., MVC controllers, repositories, classes) and their roles.

Application Functionality: Provide details about what the application does. List and describe the main features.

Application Logic: Document specific code functionality. Explain any complex logic and how it’s implemented.

Unit Testing and Other Tests: Include test scenarios, code examples, and results. Describe any UI tests or other types of testing used.

Usage Guide: Create a user guide for the application's main functions, aimed at end-users.

Code Changes (Changelog): Document changes to the code, including what was changed, who made the change, and when.
