All the configuration for this project. I really should put it in a central config file, but blazor is weird and that is a future step.
This is just a proof of concept.

Various things need to be configured in this project, including:
- Canvas OAuth Client ID
- Canvas Redirect URI
- Canvas Auth URI (If using different Canvas Domain)
- Azure tenant ID
- Azure Application Client ID
- Azure Application Redirect URI
- Azure CosmosDB URI
- Domain of host

## Locations to Configuration

#### Canvas OAuth Client ID

`CanvasOAuth.cs`

`Pages/Callback.razor`

#### Canvas Redirect URI

`Pages/Callback.razor`

`CanvasOAuth.cs`

#### Canvas Auth URI (If using different Canvas Domain)

`Pages/Callback.razor`

`CanvasOAuth.cs`

#### Azure tenant ID

`AzureOAuth.cs`



#### Azure Application Client ID

`AzureOAuth.cs`



#### Azure Application Redirect URI

`AzureOAuth.cs`



#### Azure CosmosDB URI

`AzureOAuth.cs`

`DatabaseAPIService.cs`

`CanvasOAuth.cs`

#### Host Domain

`Pages/CanvasCallback.razor`