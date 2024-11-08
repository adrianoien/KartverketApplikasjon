<h2>Oversikt over applikasjon</h2>
<p><strong>Prosjektnavn</strong>: Kartverket Applikasjon</strong></p>

<p><strong>Beskrivelse</strong>: Denne applikasjonen utvikles etter Kartverket's behover. Det skal være en webapplikasjon der brukeren kan sende inn forslag til endringer i Kart, og en saksbehandler kan behandle forslagene. Brukeren skal registrere seg for å kunne foreslå endringer. Ved innlogging kan også brukeren se en oversikt med status over foreslåtte endringer.</p>
<br>
 
<h2>Aplikasjonens arkitektur<h2/>
<p>Denne applikasjonen følger MVC (Model-View-Controller) arkitekturen...</p>
<br>

<h2>Hvordan kjøre applikasjonen</h2>
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
<br>
<h2>Andre ting vi må dokumentere:</h2>

Application Components: Explain the key components (e.g., MVC controllers, repositories, classes) and their roles.

Application Functionality: Provide details about what the application does. List and describe the main features.

Application Logic: Document specific code functionality. Explain any complex logic and how it’s implemented.

Unit Testing and Other Tests: Include test scenarios, code examples, and results. Describe any UI tests or other types of testing used.

Usage Guide: Create a user guide for the application's main functions, aimed at end-users.

Code Changes (Changelog): Document changes to the code, including what was changed, who made the change, and when.
