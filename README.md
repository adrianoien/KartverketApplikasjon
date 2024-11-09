<h2>Oversikt over applikasjon</h2>
<strong>Prosjektnavn</strong>: Kartverket Applikasjon</strong>  

<strong>Beskrivelse</strong>: Denne applikasjonen utvikles etter Kartverket's behover. Det skal være en webapplikasjon der brukeren kan sende inn forslag til endringer i Kart, og en saksbehandler kan behandle forslagene. Brukeren skal registrere seg for å kunne foreslå endringer. Ved innlogging kan også brukeren se en oversikt med status over foreslåtte endringer.  
<br>
 
<h2>Aplikasjonens arkitektur</h2>
Denne applikasjonen følger MVC (Model-View-Controller) arkitekturen:
 - <strong>Models</strong> administrerer dataen, logikken og reglene i applikasjonen (for eksempel UserData.cs og MapCorrections.cs).
 - <strong>Views</strong> administrerer UI logikken (for eksempel Index.cshtml og Kart.cshtml).
 - <strong>Controllers</strong> administrerer kommunikasjonen mellom models og views (for eksempel HomeController.cs og AccountController.cs).

Applikasjonen bruker <strong>MariaDB</strong> til å lagre brukerdata og kartendringsdata.  
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

<h2>Applikasjonskomponenter</h2>
Explain the key components (e.g., MVC controllers, repositories, classes) and their roles.
MVC, Model, View og controller. Rammeverket som separer en applikasjon inn i tre logiske komponenter, der vær komponent har som oppgave å handle en spesifikk job i applikasjonen. Med et MVC så separerer du mye av logikken, som for eksempel UI logik, input logikk og business logikk, disse tre har vær sin jobb i applikasjonen.  

Model har som jobb å utføre data logikk samt interakte med databaser, View handler om hvordan dataen blir representert til brukerne og kontroller handler om å koble sammen model og view til en fungerende applikasjon. 
I vår applikasjon så bruker vi MVC for å lett opprettholde orden i mappe struktøren og for å gjøre det lettere for personer som kommer til å ta i bruk applikasjonen og se hvordan ting er koblet sammen. MVC gjør det også lettere for testing da logikken mellom komponenter er separert. 
I vår applikasjon så er Model, Models, det er her vi har de forskjellige modellene som bruker data, register view, map corrections, og resten av .cs modelene som applikasjonen bruker. View heter Views og er hvor GUI en er, Views er hva som møter brukerne når de bbruker siden vår, enten det er å for eksempel logge in, registrere seg, egge inn endringer på kart (AreaChange). Til sist så har vi controller, eller da Controllers, det er her Views og model bblir koblet sammen til en fungerende applikasjon. Her har for eksempel AcountController, som lar brukerne blant annet logge inn, eller sier ifra hvis det er feil passord eller epost. I controller så har vi også AreaChangeController som handler logikken til Area Change, altså endre data. Her blir blant annet lengdegrad, breddegrad, status, hvem som har sent inn og når handlet når en bruker sender inn en melding om endring. 
Dette er altså hvordan vi bruker MVC i vår web applikasjon for å håndtere logikken til de forskjellige delene.  
<br>

<h2>Applikasjonens funksjonalitet</h2>
 - <strong>Brukerregistrering</strong>: Mulighet til å opprette bruker, og logge inn.
 - <strong>Vis kart</strong>: Se kart både med eller uten bruker.
 - <strong>Registrer endringsforslag</strong>: Registrerte brukere kan sende inn forslag til endring i kart.
 - <strong>Oversikt over endringsforslag</strong>: Registrerte brukere kan se oversikt og status over sine innsendte endringer.
 - <strong>Tildele endringsforslag</strong>: Innsendte endringsforslag kan bli tildelt den relevante saksbehandleren.
 - <strong>Behandle endringsforslag</strong>: Saksbehandler kan behandle foreslåtte endringer.  
<br>

Application Logic: Document specific code functionality. Explain any complex logic and how it’s implemented.

Unit Testing and Other Tests: Include test scenarios, code examples, and results. Describe any UI tests or other types of testing used.

Usage Guide: Create a user guide for the application's main functions, aimed at end-users.

Code Changes (Changelog): Document changes to the code, including what was changed, who made the change, and when.
