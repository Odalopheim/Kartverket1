# Demonstrasjon av applikasjonen 

	Her kommer det en GIF 
## Rett i Kartet
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
│   │   ├── **BrukerController** 
│   │   ├── **HomeController**  
│   │   ├── **SaksbehandlerController**  

│   ├── ##Data              
│   │   ├── **ApplicationDbContext** 
│   │   ├── **GeoChange** 
│   │   ├── **GeoChangeService**
│   │   ├── **RoleInitializer** 
│   │   ├── **UserDetails** 

│   ├── ##Migrations        

│   ├── ##Models        
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
│   │   │   ├── **Login** 
│   │   │   ├── **MinSide**   
│   │   │   ├── **Register**    
│   │   │   ├── **RegistrationSuccess** 
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
│   │   ├── **Login** 
│   │   ├── **MinSide**  
│   │   ├── **Register**    
│   │   ├── **RegistrationSuccess**   
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
	- `"3306:3306"` Mapper port 3306 i containeren til port 3306 på 	vertsmaskinen.
- `volumes`
	- Bruker et volum kalt `mariadb_data` for å persistere data.
- `networks`
	- Bruker et nettverk kalt `backend`.

#### Kartverket tjeneste 
- `build`:
  - `context: .`  
	- Byggekonteksten er satt til gjeldende katalog.
  - `dockerfile: Kartverket/Dockerfile`  
	- Spesifiserer Dockerfile i `Kartverket`-katalogen for bygging av 	avbildningen.

- `container_name:`
	- kartverket Setter navnet på containeren til `kartverket`.

- `environment:`
	- Setter miljøvariabler, inkludert databasetilkoblingsstrengen 	(`ConnectionStrings__DefaultConnection`).

- `ports:`
	- `"5000:80"` Mapper port 80 i containeren til port 5000 på 	vertsmaskinen.

- `depends_on:`
	- Angir en avhengighet til `mariadb`-tjenesten. Docker Compose vil 	starte `mariadb` først.

- `networks:`
	- Bruker et nettverk kalt backend.
### Volumes
- `mariadb_data:` 
	- Et navngitt volum for MariaDB-tjenesten, hvor MariaDB lagrer sine 	datafiler.

### Nettverk
- `backend:` 
	- Et tilpasset nettverk for å koble sammen tjenestene.
