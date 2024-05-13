Documentation to access CosmosDB from a client side BlazorWASM

Code is documented - main components are the `Callback.razor`, `DatabaseAPIService.cs`, and `AzureOAuth.cs` files.

Steps to setup in Azure - 

I. CosmosDB

1. Create a CosmosDB instance
2. Copy the instance URI and configure that in both `DatabaseAPIService.cs` and `AzureOAuth.cs`
3. In the CosmosDB instance under Settings -> CORS add the domain of the site to the list of allowed origins (e.g. "http://localhost:3000")
4. Configure Permissions
	
	This is a mess, and needs to be done from the Azure CLI.
	Bash files and role JSONs will be at the bottom of this file.
	
	A. Open a new Azure Command Line
	
	B. Run `code .` to open a new file editor
	
	C. In a new file create `role-definition-rw.json` (see below)
	
	D. In another file create the `createRole.sh` script using your resource group and CosmosDB account name
	
	E. Run the script using `bash createRole.sh`
	
	F. Get the **roleDefinitionId** for the role you just created by running 
		`az cosmosdb sql role definition list --account-name $accountName -g $resourceGroupName`
		and substituting `$accountName` and `$resourceGroupName` for their correspong values
	
	G. Note down the **roleDefinitionId** which can be found in the name property.
	
	H. Create the `applyRole.sh` script as seen below, configuring the values as seen below. The object ID you wish to assign to can be accessed via Microsoft Entra ID -> Users -> (YourUser) -> Overview

	I. Run the script using `bash applyRole.sh`
5. Create the Data

	A. Under the CosmosDB instance, in the Data Explorer create a new database (`test_db` is what I used) and a collection called `oauth`
	
	B. This collection should have only 1 document, with an `id` of 0, looking like so:

	```json
	{
    "id": "0",
    "client_id": "YourCanvasClientIDHere",
    "client_secret": "YourCanvasClientSecretHere",
    }
	```


II. App Registration
This is needed for the Authorization using Microsoft EntraID.

1. In the Microsoft EntraID portal -> App registrations hit NEW REGISTRATION and enter an application name and hit register.
2. Note down the Application (client) ID and the Directory (tenant) ID.
3. Under the app registration -> Manage -> Authentication hit **Add a platform** and choose **Single-page application**, giving it the redirect URL you want to use - then hit **Configure**.
4. Configure AzureOAuth.cs with the tenant ID, client ID and the redirect URI





External Files for configuring permissions:

role-definition-rw.json:

```json
{
    "RoleName": "CosmosDBReadWriteRole",
    "Type": "CustomRole",
    "AssignableScopes": ["/"],
    "Permissions": [{
        "DataActions": [
            "Microsoft.DocumentDB/databaseAccounts/readMetadata",
            "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/*",
            "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/*"
        ]
    }]
}
```

createRole.sh:

```sh
resourceGroupName='YourResourceGroupName'
accountName='CosmosDBAccountName'
az cosmosdb sql role definition create -a $accountName -g $resourceGroupName -b @role-definition-rw.json
```

applyRole.sh:

```sh
resourceGroupName='YourResourceGroupName'
accountName='CosmosDBAccountName'
roleDefinitionId='GET THIS USING THE COMMAND MENTIONED ABOVE'
principalId='YourUserObjectId'
az cosmosdb sql role assignment create -a $accountName -g $resourceGroupName -s "/" -p $principalId -d $roleDefinitionId
```