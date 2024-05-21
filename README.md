# Token Testing Blazor (Feature Branch)

This is a Demo for Canvas Authentication using a Lean Blazor server and WASM.
It uses an Azure CosmosDB instance to store credentials, meaning it also uses Microsoft Entra ID to Authenticate.

This branch of the project includes code to refresh the access tokens, as well as tell when they expire.

The actual login process is much less clear, so the main branch will simply be the login process, with this branch containing more of Authentication features, as it is harder
to tell what is going on.

## About

This project has two parts, the Blazor Server and the Client. Most of the work is done on the client, however
server exposes a single API Endpoint which can be used to exchange the client authentication code for an access token, that way
credentials are not ever accessed on the client, which would be insecure.

## Auth Flow

When initializing the authorization flow by hitting "Login", the client directs the user to the Microsoft Login Page, 
where they login using Microsoft Entra ID. From there they are redirected back to the WASM which exchanges the auth code for an Azure Active Directory Access Token.
After that the user is once again redirected to the Canvas login, where they grant permission for the application to access their account, redirecting to the WASM with an authentication code.
The client then makes a call to the Blazor server with the auth code, while the server accesses the CosmosDB instance to get the Canvas OAuth Client Secret which is used to exchange said auth code for
a Canvas Access Token.


## Refresh Token

This branch actually contains the logic in order to refresh the Access Tokens. It also includes an `<Auth />` component which can be wrapped around a web page to have it redirect the user if they aren't logged in.

## Setup

To get this project working see [Azure Setup](AzureSetup.md) and [Project Configuration](Configuration.md)
