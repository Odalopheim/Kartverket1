# Demonstrasjon av applikasjonen 

**GIF** 

<pre>
	## Rett i Kartet (GitHub Repository Root)
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
│   │   ├── **AccountControllers** 
│   │   ├── **BrukerController** 
│   │   ├── **HomeController** 
│   │   ├── **InnmeldingController** 
│   │   ├── **MapCorrectionsController** 

│   ├── ##Data              
│   │   ├── **Ansatt** 
│   │   ├── **ApplicationDbContext** 
│   │   ├── **Bruker** 
│   │   ├── **BrukerId** 
│   │   ├── **GeoChange** 
│   │   ├── **GeoData** 
│   │   ├── **Gjest** 
│   │   ├── **Innmelder** 
│   │   ├── **Innmelding** 
│   │   ├── **Kategori** 
│   │   ├── **PostSted**   

│   ├── ##Migrations        

│   ├── ##Models        
│   │   ├── #Accounts
│   │   │   ├── **ExternalLoginConfirmationViewModel** 
│   │   │   ├── **ForgotPasswordViewModel** 
│   │   │   ├── **LoginViewModel** 
│   │   │   ├── **RegistrerViewModel** 
│   │   │   ├── **ResetPasswordViewModel** 
│   │   │   ├── **SendCodeViewModel** 
│   │   │   ├── **UseRecoveryCodeViewModel** 
│   │   │   ├── **VerifyAuthenticatorCodeViewModel** 
│   │   │   ├── **VerifyCodeViewModel** 
│   │   ├── **ErrorViewModel**
│   │   ├── **KommuneInfoViewModel**
│   │   ├── **MapCorrectionModels**
│   │   ├── **MinSideViewModel**  
│   │   ├── **PositionModel**
│   │   ├── **StedsnavnViewModel**
│   │   ├── **UserData**

│   ├── ##Services  
│   │   ├── **IKommuneInfoService**  
│   │   ├── **IStedsnavnService**
│   │   ├── **KommuneInfoService**  
│   │   ├── **StedsnavnService**  

│   ├── ##Views           
│   │   ├── # Account 
│   │   │   ├── **Login** 
│   │   │   ├── **Register**      
│   │   ├── # Home    
│   │   │   ├── **AreaChangeOverview** 
│   │   │   ├── **Bruker** 
│   │   │   ├── **CorrectionOverview** 
│   │   │   ├── **Index** 
│   │   │   ├── **KommuneInfo** 
│   │   │   ├── **MinSide** 
│   │   │   ├── **Overview** 
│   │   │   ├── **Privacy** 
│   │   │   ├── **RegisterAreaChange** 
│   │   │   ├── **RegistrationForm** 
│   │   │   ├── **Saksbehandler** 
│   │   │   ├── **Stedsnavn** 
│   │   ├── # Innmelding  
│   │   │   ├── **Login** 
│   │   │   ├── **Register**    
│   │   ├── # Shared 
│   │   │   ├── **Login** 
│   │   │   ├── **Register** 
│   │   ├── **_ViewImports**  
│   │   ├── **_ViewStart**     

│   ├── ##appsettings.json

│   ├── ##DockerFile         

│   ├── ##Program.cs
</pre>

Ovenfor vises det arkitektoniske mønsteret til Model-View-Controller (MVC). MVC deler aplikasjonen inn i tre forskjellige hovedkomponenter Model, View og Controller. Det gjør at koden blir emodulær og gjør strukturen enklere å vedlikeholde. 

## * Model 

<pre>

## Models
        
│   ├── #Accounts
│   │   ├── **ExternalLoginConfirmationViewModel** 
│   │   ├── **ForgotPasswordViewModel** 
│   │   ├── **LoginViewModel** 
│   │   ├── **RegistrerViewModel** 
│   │   ├── **ResetPasswordViewModel** 
│   │   ├── **SendCodeViewModel** 
│   │   ├── **UseRecoveryCodeViewModel** 
│   │   ├── **VerifyAuthenticatorCodeViewModel** 
│   │   ├── **VerifyCodeViewModel** 
│   ├── **ErrorViewModel**
│   ├── **KommuneInfoViewModel**
│   ├── **MapCorrectionModels**
│   ├── **MinSideViewModel**  
│   ├── **PositionModel**
│   ├── **StedsnavnViewModel**
│   ├── **UserData**

</pre>

## * View

<pre>
## Views   
        
│   ├── # Account 
│   │   ├── **Login** 
│   │   ├── **Register**      
│   ├── # Home    
│   │   ├── **AreaChangeOverview** 
│   │   ├── **Bruker** 
│   │   ├── **CorrectionOverview** 
│   │   ├── **Index** 
│   │   ├── **KommuneInfo** 
│   │   ├── **MinSide** 
│   │   ├── **Overview** 
│   │   ├── **Privacy** 
│   │   ├── **RegisterAreaChange** 
│   │   ├── **RegistrationForm** 
│   │   ├── **Saksbehandler** 
│   │   ├── **Stedsnavn** 
│   ├── # Innmelding  
│   │   ├── **Login** 
│   │   ├── **Register**    
│   ├── # Shared 
│   │   ├── **Login** 
│   │   ├── **Register** 
│   ├── **_ViewImports**  
│   ├── **_ViewStart**  

</pre>

## * Controller

<pre>  
## Controllers      

│   ├── **AccountControllers** 
│   ├── **BrukerController** 
│   ├── **HomeController** 
│   ├── **InnmeldingController** 
│   ├── **MapCorrectionsController** 

</pre>
