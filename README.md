# Demonstrasjon av applikasjonen 

![Gif](https://github.com/user-attachments/assets/df8764cc-2511-4035-93fa-c79145e2f0db)

# Viktig info

## Opprettelse av bruker til saksbehandler 
For å lage saksbehandlerbrukere må man først logge inn på adminbrukeren. 
Alle saksbehandlere må ha @Kartverket.no
Innloggingsinfo admin:
- Email: admin@Kartverket.no
- Passord: Admin123!

Innloggingsinfo saksbehanler:
 - Email: saksbehandler@Kartverket.no
 - Passord: Saksbehandler123!

## Testing
Testene ligger i branchen " " som er identisk til Main bare med testene. Testene førte til feilmeldinger hos enkelte men ikke alle. Vi valgte derfor å ikke legge disse inn i Main da vi ikke ønsket at disse eventuelt skulle ødelegge for resten av applikasjonen hvor alt funker.

## Hovedfunksjon 
### Brukere 
- Registrere feil i kart 
- Sjekke status av egen innmeldinger
- Motta status endringer på innmeldinger

### Saksbehandlerer
- Oversikt over alle innmeldinger sortert etter kategori og dato
- Endre status og kategori på innmeldinger

### Administrator
- Oversikt over alle saksbehandlere
- Mulighet til å opprette og slette saksbehandlere 


# Rett i Kartet 
<pre>
│   ├── ## Root         
│   │   ├── # css         
│   │   │   ├── **mycsstheme.css** 
│   │   │   ├── **site.css** 
│   │   ├── # images     
│   │   ├── # js 
│   │   ├── # lib 

│   ├── ## API Models         
│   │   ├── **ApiSettings**        
│   │   ├── **KommuneInfo**
│   │   ├── **StedsnavnResponse**    

│   ├── ## Controllers      
│   │   ├── **AccountController** 
│   │   ├── **AdminController** 
│   │   ├── **BrukerController** 
│   │   ├── **HomeController**  
│   │   ├── **SaksbehandlerController**  

│   ├── ##Data              
│   │   ├── **ApplicationDbContext** 
│   │   ├── **GeoChange** 
│   │   ├── **GeoChangeService**
│   │   ├── **RoleInitializer** 
│   │   ├── **UserDetails** 
│   │   ├── **UserService** 

│   ├── ##Migrations        

│   ├── ##Models  
│   │   ├── **CreateSaksbehandlerViewModel**
│   │   ├── **EditUserViewModel**
│   │   ├── **ErrorViewModel**
│   │   ├── **KommuneInfoViewModel**
│   │   ├── **LoginViewModel**
│   │   ├── **MinSideViewModel**  
│   │   ├── **RegistrerViewModel**
│   │   ├── **StedsnavnViewModel**

│   ├── ##Services  
│   │   ├── **IKommuneInfoService**  
│   │   ├── **IStedsnavnService**
│   │   ├── **KommuneInfoService**  
│   │   ├── **StedsnavnService**  

│   ├── ##Views           
│   │   ├── # Account 
│   │   │   ├── **EditUserInfo** 
│   │   │   ├── **Login** 
│   │   │   ├── **MinSide**   
│   │   │   ├── **Register**    
│   │   │   ├── **RegistrationSuccess** 
│   │   ├── # Admin 
│   │   │   ├── **AdminHjemmeside** 
│   │   │   ├── **CreateSaksbehandler**   
│   │   │   ├── **DeleteSaksbehandler**    
│   │   ├── # GeoChange 
│   │   │   ├── **Delete** 
│   │   │   ├── **Details**   
│   │   │   ├── **Edit**    
│   │   │   ├── **RegistrerAreaChange**   
│   │   ├── # Home     
│   │   │   ├── **Index** 
│   │   │   ├── **KommuneInfo** 
│   │   │   ├── **Stedsnavn**  
│   │   ├── # Saksbehandler     
│   │   │   ├── **Saksbehandler** 
│   │   │   ├── **Update** 
│   │   ├── # Shared 
│   │   │   ├── **_Layout** 
│   │   │   ├── **ValidationScriptsPartial** 
│   │   │   ├── **Error** 
│   │   ├── **_ViewImports**  
│   │   ├── **_ViewStart**     

│   ├── ##appsettings.json

│   ├── ##DockerFile         

│   ├── ##Program.cs
</pre>

Ovenfor vises det arkitektoniske mønsteret til Model-View-Controller (MVC). MVC deler aplikasjonen inn i tre forskjellige hovedkomponenter Model, View og Controller. Det gjør at koden blir emodulær og gjør strukturen enklere å vedlikeholde. 

## * Model 

Models har ansvar for å representere data og forretningslogikk. Strukturen på data blir definert, kommuniserer med databasen for å hente, lagre og oppdatere data, og inneholder logikk og regler for hvordan data skal behandles. 
<pre>

## Models

│   ├── **CreateSaksbehandlerViewModel**
│   ├── **EditUserViewModel**
│   ├── **ErrorViewModel**
│   ├── **KommuneInfoViewModel**
│   ├── **LoginViewModel**
│   ├── **MinSideViewModel**  
│   ├── **RegistrerViewModel**
│   ├── **StedsnavnViewModel**

</pre>

## * View

Views er ansvarlig for brukergrensesnittet. Den gjengir informasjon, basert på data fra Models, til brukeren. HTML, CSS og JavaScript blir vist gjennom en dynamisk side (Razor Pages). 
<pre>
## Views   
        
│   ├── # Account 
│   │   ├── **EditUserInfo** 
│   │   ├── **Login** 
│   │   ├── **MinSide**  
│   │   ├── **Register**    
│   │   ├── **RegistrationSuccess**   
│   ├── # Admin 
│   │   ├── **AdminHjemmeside** 
│   │   ├── **CreateSaksbehandler**   
│   │   ├── **DeleteSaksbehandler**    
│   ├── # GeoChange 
│   │   ├── **Delete** 
│   │   ├── **Details**  
│   │   ├── **Edit**    
│   │   ├── **RegistrerAreaChange**   
│   ├── # Home    
│   │   ├── **Index** 
│   │   ├── **KommuneInfo** 
│   │   ├── **Stedsnavn**   
│   ├── # Saksbehandler    
│   │   ├── **Saksbehandler** 
│   │   ├── **Update** 
│   ├── # Shared 
│   │   ├── **_Layout** 
│   │   ├── **ValidationScriptsPartial** 
│   │   ├── **Error** 
│   ├── **_ViewImports**  
│   ├── **_ViewStart** 

</pre>

## * Controller

Controller er ansvarlig for å håndtere logikken og styrer flyten mellom Model og View. Den behandler forespørsler fra brukeren, kaller på Models for å oppdatere og hente data og sender disse til Views for å vises til brukeren. 
<pre>  
## Controllers      

│   ├── **AccountController** 
│   ├── **AdminController** 
│   ├── **BrukerController** 
│   ├── **HomeController** 
│   ├── **SaksbehandlerController**  

</pre>

## Docker Compose Konfigurasjon 

### Version
- `version: '3.8'`
	- Angir versjonen av Docker Compose-filformatet. Kompatibel med Docker Engine 17.09.0+.

### Services
- Denne seksjonen definerer to tjenester: `mariadb` og `kartverket`.

#### MariaDB Tjeneste
- `image: mariadb-latest`  
	- Bruker den nyeste MariaDB Docker-avbildningen.
- `container_name: mariadb` 
	- Setter navnet på containeren til `mariadb`.
- `environment`
	- Setter miljøvariabler som `MYSQL_ROOT_PASSWORD`, `MYSQL_DATABASE`, `MYSQL_USER` og `MYSQL_PASSWORD`
- `ports`
	- `"3306:3306"` Mapper port 3306 i containeren til port 3306 på vertsmaskinen.
- `volumes`
	- Bruker et volum kalt `mariadb_data` for å persistere data.
- `networks`
	- Bruker et nettverk kalt `backend`.

#### Kartverket tjeneste 
- `build`:
  - `context: .`  
	- Byggekonteksten er satt til gjeldende katalog.
  - `dockerfile: Kartverket/Dockerfile`  
	- Spesifiserer Dockerfile i `Kartverket`-katalogen for bygging av avbildningen.

- `container_name:`
	- kartverket Setter navnet på containeren til `kartverket`.

- `environment:`
	- Setter miljøvariabler, inkludert databasetilkoblingsstrengen 	(`ConnectionStrings__DefaultConnection`).

- `ports:`
	- `"5000:80"` Mapper port 80 i containeren til port 5000 på vertsmaskinen.

- `depends_on:`
	- Angir en avhengighet til `mariadb`-tjenesten. Docker Compose vil starte `mariadb` først.

- `networks:`
	- Bruker et nettverk kalt backend.
### Volumes
- `mariadb_data:` 
	- Et navngitt volum for MariaDB-tjenesten, hvor MariaDB lagrer sine datafiler.

### Nettverk
- `backend:` 
	- Et tilpasset nettverk for å koble sammen tjenestene.

## Testing 
- Antall tester
- Hva som testes

Testing under EXPO
- Under EXPO 11. november 2024 ble det gjennomført flere brukstester og usability-tester av både applikasjonen og prototypen
- Applikasjonen har også blitt testet av diverse venner og familie som er i alderen 20år til 80år

## MariaDB database
### Dapper
- Bruker dapper til å kjøre SQL-spørringer mot databasen
- Mikro orm 
- Fordeler med dapper
	- Skriver SQL-spørringer direkte
 	- Raskere med dapper
  	- Hjelper med å hindre SQL injections 

### Entity framework
- Fullverdig orm
- Håndterer migrasjoner


### Fordeler med Dapper og Entity Framework
- Fordelene med å bruke begge er at man får den høye ytelsen fra Dapper og produktivitetsfordelene til Entity Framework.


### Tabeller laget 
- `AspNetRolesClaims` : Lagrer tilleggsrettigheter knyttet til roller.
- `AspNetRoles` : Lagrer roller i systemet.
- `AspNetUserClaims` : Lagrer spesifikke rettigheter knyttet til individuelle brukere. 
- `AspNetUserLogins` : Håndterer eksterne pålogginger.
- `AspNetUserRoles` : Knytter brukere til roller i systemet.
- `AspNetUserTokens` : Lagrer tokens brukt for autentisering eller spesifikke handlinger.
- `AspNetUsers` : Lagrer brukerkontoer og deres basisinformasjon. 
- `GeoChanges` : Logger endringer i geografisk informasjon eller posisjon.
- `UserDetails` : Lagrer tilleggsinformasjon om brukere som ikke finnes i `AspNetUsers`.
- `__EFMigrationsHistory` :Sporer hvilke databaseendringer (migrasjoner) som er utført.


## Sikkerhet
### Autentisering og Autorisering
- Custom session-basert autentisering
- CSRF-beskyttelse ved bruk av AntiForgeryToken
- Beskyttelse mot SQL-Injections ved bruk av Dapper og parameteriserte spørringer
- Feilhåndtering og strukturert Logging ved bruk av ILogger
- Rollebasert tilgangkontroll
- Validering av input
- Bruk av HTTPS Redirection
- Bruk av HTTP Strict Transport Security
- Transient Feilresiliens for Databasekobling
- Dapper DB-tilkobling med IDbConnection
  
