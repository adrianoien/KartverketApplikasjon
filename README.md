## Oversikt over applikasjon

**Prosjektnavn**: Kartverket Applikasjon

**Beskrivelse**: Denne applikasjonen utvikles etter Kartverket's behover. Det skal være en webapplikasjon der brukeren kan sende inn forslag til endringer i Kart, og en saksbehandler kan behandle forslagene. Brukeren skal registrere seg for å kunne foreslå endringer. Ved innlogging kan også brukeren se en oversikt med status over foreslåtte endringer.   
<br>

## Applikasjonens arkitektur

Denne applikasjonen følger MVC (Model-View-Controller) arkitekturen:

- **Models** administrerer dataen, logikken og reglene i applikasjonen (for eksempel UserData.cs og MapCorrections.cs).
- **Views** administrerer UI logikken (for eksempel Index.cshtml og Kart.cshtml).
- **Controllers** administrerer kommunikasjonen mellom models og views (for eksempel HomeController.cs og AccountController.cs).

Applikasjonen bruker **MariaDB** til å lagre brukerdata og kartendringsdata.   
<br>

## Hvordan kjøre applikasjonen lokalt

Forutsetningsapplikasjoner:
- Docker Desktop
- Visual studio (eller lignende IDE)  
<br>

**Steg 1 - Klon repository**
- Gå på "clone repository" i visual studio
- Kopier linken til prosjektet på github
- Lim inn i visual studio  
<br>

**Steg 2 - Koble til MariaDB**  
Når du kompilerer fila i Visual Studio, så gjør det igjennom docker compose.

- Trykk på menyen hvor du builder, gå inn på "Configure startup projects"
- Velg docker-compose og kjør denne

Nå skal applikasjonen være koblet til databasen  
<br>

**Steg 2.5 (Hvis nødvendig) - Koble til MariaDB i MySQL Client**  
* docker exec -it mariadb mariadb -u root geochangesdb -p*

Enkle kommandoer for å se om den er koblet til:

*SHOW DATABASES;*

*SHOW TABLES;*  
<br>

**Steg 3 - Kjør applikasjonen**
- Gå inn på applikasjonen
- Lag bruker og test funksjonalitet!
<br>

## Applikasjonskomponenter

Explain the key components (e.g., MVC controllers, repositories, classes) and their roles.
MVC, Model, View og controller. Rammeverket som separer en applikasjon inn i tre logiske komponenter, der vær komponent har som oppgave å handle en spesifikk job i applikasjonen. Med et MVC så separerer du mye av logikken, som for eksempel UI logik, input logikk og business logikk, disse tre har vær sin jobb i applikasjonen.

Model har som jobb å utføre data logikk samt interakte med databaser, View handler om hvordan dataen blir representert til brukerne og kontroller handler om å koble sammen model og view til en fungerende applikasjon. I vår applikasjon så bruker vi MVC for å lett opprettholde orden i mappe struktøren og for å gjøre det lettere for personer som kommer til å ta i bruk applikasjonen og se hvordan ting er koblet sammen. MVC gjør det også lettere for testing da logikken mellom komponenter er separert. I vår applikasjon så er Model, Models, det er her vi har de forskjellige modellene som bruker data, register view, map corrections, og resten av .cs modelene som applikasjonen bruker. View heter Views og er hvor GUI en er, Views er hva som møter brukerne når de bbruker siden vår, enten det er å for eksempel logge in, registrere seg, egge inn endringer på kart (AreaChange). Til sist så har vi controller, eller da Controllers, det er her Views og model bblir koblet sammen til en fungerende applikasjon. Her har for eksempel AcountController, som lar brukerne blant annet logge inn, eller sier ifra hvis det er feil passord eller epost. I controller så har vi også AreaChangeController som handler logikken til Area Change, altså endre data. Her blir blant annet lengdegrad, breddegrad, status, hvem som har sent inn og når handlet når en bruker sender inn en melding om endring. Dette er altså hvordan vi bruker MVC i vår web applikasjon for å håndtere logikken til de forskjellige delene.   
<br>

## Applikasjonens funksjonalitet

**Brukerregistrering**: Mulighet til å opprette bruker, og logge inn.

**Vis kart**: Se kart både med eller uten bruker.

**Registrer endringsforslag**: Registrerte brukere kan sende inn forslag til endring i kart.

**Oversikt over endringsforslag**: Registrerte brukere kan se oversikt og status over sine innsendte endringer.

**Tildele endringsforslag**: Innsendte endringsforslag kan bli tildelt den relevante saksbehandleren.

**Behandle endringsforslag**: Saksbehandler kan behandle foreslåtte endringer.   
<br>

## 💡 Slik bruker du applikasjonen
For brukere
Registrering og innlogging

- Gå til applikasjonen
- Klikk "Registrer" for ny bruker
- Fyll ut skjema med brukernavn, e-post og passord
- Velg rolle (Bruker/Saksbehandler)

- Melde inn endring

- Klikk "Registrer Områdeendring"
- Velg type endring (punkt eller område)
- Marker på kartet
- Fyll ut beskrivelse
- Send inn endring

## For saksbehandlere
- Logg inn som saksbehandler
- Gå til "Behandle Endringer" → "Dashboard"
- Se oversikt over ventende saker
-  Klikk på en sak for å behandle den
<br>

## 🛠 Teknisk oversikt
Bygget med

- ASP.NET Core MVC
- Entity Framework Core
- Leaflet.js for kart
- Bootstrap for UI
- MariaDB for database
<br>

## 👥 Dette er et åpent-kilde prosjekt, som betyr at hvem som helst kan bidra, inkludert deg!

- Fork prosjektet
- Lag en feature branch (git checkout -b feature/MinNyeFunksjon)
- Commit endringene (git commit -m 'Lagt til MinNyeFunksjon')
- Push til branch (git push origin feature/MinNyeFunksjon)
- Åpne en Pull Request
<br>

## Andre ting vi må dokumentere:

Application Logic: Document specific code functionality. Explain any complex logic and how it’s implemented.

Unit Testing and Other Tests: Include test scenarios, code examples, and results. Describe any UI tests or other types of testing used.


## Application Logic: Document specific code functionality. Explain any complex logic and how it’s implemented.
Starter vi fra toppen av mappestrukturen så har vi kontrollere, som forlart over kobler sammen views og models. I kontroller klassen har vi 4 kontroller.cs filer, disse er satt opp som public class kontrollere som betyr at klassene er tilgjenlige for klienter/ webserveren. Vær kontroller har en rolle som den gjør, Account kontroller lar brukere lage kontoer i webapplikasjonen. Areachange kontroller lar brukere utføre område endringer, den har også som funksjon og validere data og lagre dette i en database. Hvis den finner feil så har den muligheten til å catche disse feilene og oppretter en loggmelding.CorrectionManagement har som oppgave og håndtere status på korreksjoner, se en detalgjert beskrivelse av disse korekksjonene, søke etter brukere som har rollen saksbehandler og lagre endringer til databasen. Home kontroller har som oppgave å redirekte autoriserte brukere til area change siden, gi brukerne et overbblikk over mappe korreksjoner, og hente ventede korreksjoner sortert med innsendings dato senest først. Home kontroller gjør at den en autorisert bruker kan se alle innsendingene av den inlogde brukeren. Home kontroller kan også hente kart korreksjoner, kart endringer og kan kombinere disse i en view model. Den kan også vise error sider. 

Videre så har vi Data mappen med applicationDbContext og Geochange cs filer. geochange er en entitetklasse som representerer en entitet i webapplikasjonen mens DbContext klassen blir brukt for å opprette en databasen til Geochange entiteten. Context bruker EntityFramework Core for å administrere modeller og database konfigurasjon. Context filen representerer brukere, geo endringer og kart korreksjoner i tabeller. Gechange representerer geografiske endringer og inneholder ID, beksrivelse, GeoJSON, status, hvem som har sendt inn endirngen og når datoen ble sendt inn, og flere andre strenger som skal gi oversikt over endringene. Dette blir gjort med getter og setter i en public klasse. 
Etter Data mappen så har vi Migrations. Migrations blir laget for å holde databasen synkronisert med modellene i koden. Migrations oppgaver er å opprette databaser og tabeller. Oppdatere strukturen i databasen når en modell blir endret, spore endringer, Migrations er en logg for alle endringene som blir gjort i dataabsen. Migrations kan også tilbakestille endringer eller automatisere endringer i databasen. Migrations gungerer altså som en .cs filer som representerer endringene som blir gjort. 

Models som gjennomgått tidligere er en de av MVC som applikasjonen har blitt laget med, models inneholder modell filene som trengs for å logge inn med passord brukernavn. brukerdata, passord, epost, addresse og den slags. Den har også modeller for å tilordne saker, tilordne prioritenger, endre innsending, korreksjon og dashbord model. Error model som spørr etter ID. Mappen inneholder også resten av moddellene som for applikasjonen til å fungere sammen med Views og kontrollere. 

Views som gjennomgått tidligere er UI logikken. Views mappen inneholder views filer for brukerkonto, area endringer, korreksjon ledelse, hjem og delt og 2 cshtml filer som er Viewimport og View start. 

De siste filene er Appsetting.json som inneholder tilkoblingsstrenger, Program.cs som legger til tjenester i konteineren, legger til Dbcontext konfigurasjon. Konfigurer http anmodning og kjører "APP" som den bygger tidligere i filen. 
De siste filene er Docker filer som gjør det mulig for oss å kjøre webapplikasjonen i en konteiner. Dockerfile bygger prosjektet mens Docker compose gjør at man kan bruke docker compose for å koble webapplikasjonen sammen med MariaDB databasen vår. 


## Code Changes (Changelog): Document changes to the code, including what was changed, who made the change, and when.


## Database modell ![image](https://github.com/user-attachments/assets/7035948c-d78e-491c-ab0c-b81b6ad25088)

Denne database modellen viser en oversikt over strukturen av håndtering av brukerinformasjon "UserData". Modellen viser tre modeller, "UserData", "Mapcorrections" og "GeoChange". UserData inneholder informasjonen til brukeren og hvilke datatyper informasjonen bruker, for eksempel, String =name, Int = id. userData har også UserRole da brukerne av applikasjonen kan velge mellom 2 roller, bruker og saksbehandler. 

MapCorrections håndterer kart korreksjoner, med et bredt utvalg detajer som er viktig for saksbehandlere, for eksempel latitude, reviewed by, priority, id og mer. Mapcorrections lar oss spore hvem som har sendt inn korreksjoner og når, hvem som har tildelt korreksjonen, status og notater på korreksjonen. 

Geochange handler om endringer knyttet til geografisk data. Tabellen har stor likhet med MapCorrections for uten om det at geoChange inkluderer et GeoJson felt.

Begge tabellene har en submit/ reviews relasjon med userdata. 
