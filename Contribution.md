# Moving Forward

This page contains details on how to add new pages to the project, as well as specifics for components and services.

## The design philosophy

This project is designed to have as lean of a server as possible, meaning if it can be done on the client, it should be.

## Adding pages

In order to add a new client side page, simply create a new `.razor` file in the `TokenTestingBlazor.Client/Pages` folder.

To specify the path, add a `@page "path"` directive at the top of the razor file. from there, **HTML** can be written in the file, and **C#** in the `@code` block.
To add **CSS** to a page, create a `PageName.razor.css` file in the same directory as the razor component.

## Render modes

Blazor has specific render modes for different tasks, but essentially what we want for this project is `InteractiveWebAssembly` with prerender turned off. To add this in a page,
under the `@page` directive, add `@rendermode @(new InteractiveWebAssemblyRenderMode(prerender: false))`. This makes it so that when a page is loaded, dependencies can be injected before it tries to render the page (It would throw an error otherwise). It also makes it so the client renders the components, not the server (results in some initial latency when loading the page).

# Components

Below is a list and description of each component that has been created

## Login Header

This is simply the default blazor header, with a link to login/logout depending on the application state. It has been moved to the client so that it can access cookies.
For a page to use this header, it should be wrapped in the `<LoginHeader />` component. If using the Auth component, the LoginHeader is already included.

## Auth

This component ensures that before rendering the page content, there is an Access Token for both Azure and Canvas. It will redirect the user to the appropriate page depending on the combination of tokens, and refresh any tokens that have expired.

# API Endpoints

The server exposes three API endpoints for use with Canvas Authentication (They are pretty self-explanitory)

- `api/auth/getToken`
- `api/auth/refreshToken`
- `api/auth/canvasLogout`

# Services

This contains a list and description of each service that has been created, as well as their functions and uses.

To use a service on a page, we use dependency injection (DI). In `Program.cs` the service is registered using `builder.Services.` `AddScoped`/`AddSingleton`/`AddTransient` depending on the service lifetime.
Then, on the page you want to use the service, add the `@inject` directive to inject said service.

## CanvasOAuth.cs (Server)

This is a class used with Canvas Authentication.

#### Methods

- GetCanvasTokenAsync
    - Given the Authorization code, it exchanges the code for the Canvas Access Token asyncrounously
- RefreshCanvasTokenAsync
    - Given the refresh token, refreshes the Canvas Access Token in an async manner
- CanvasLogout
    - Given the access token, it logs a user out of the canvas session

## AzureOAuth.cs (Client)

Class used with Azure authentication

#### Methods

- GetAuthCode
    - Given the navigation manager, it navigates to the appropirate Microsoft endpoint to being Azure authentication
- GetAccessToken
    - Given the state variable and the authorization code passed in the callback function it exchanges the code for an Azure Access Token
- RefreshAccessTokne
    - Given the refresh token, refreshed the Azure Access Token

## CanvasAuthAccessor.cs (Client)

A class used to access the API endpoints exposed by the server

#### Methods

- GetAccessTokenAsync
    - Accesses the `/api/auth/getToken` endpoint, exchanging the authorization code for the Canvas Access Token
- RefreshAccessTokenAsync
    - Accesses the `/api/auth/refreshToken` endpoint, refreshing the Canvas Access Token
- CanvasLogout
    - Access the `/api/auth/canvasLogout` endpoint, logging out the canvas user

## CookieStorageAccessor.cs (Client)

A class used to interact with cookies.

#### Methods

- GetValueAsync<T>
    - Gets a cookie with a specified key, returning data of type `T`
- SetValueAsync<T>
    - Creates a cookie with value of type `T` with specified key, expiring in the specified number of seconds
- DeleteValueAsync
    - Deletes a cookie with the specified key
