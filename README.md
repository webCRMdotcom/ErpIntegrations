# webCRM ERP Integrations

## Prerequisites

- Visual Studio 2017.
- .NET Core 2.1.
- The Azure Function and Web Jobs Tools extension for Visual Studio. Used to run locally and to deploy.
- [Install and Configure Azure PowerShell](https://docs.microsoft.com/en-us/powershell/azure/install-azurerm-ps?view=azurermps-5.7.0).
- ReSharper is recommended.
- Not sure if this is still required: Node.js version 8.x. Version 9 and 10 are not supported.

## Development

### Building and Hosting Locally

Use the usual `Run` command in Visual Studio.

### Running the Tests

Use Visual Studio or ReSharper's test runner.

### Generate the webCRM SDK

Choose the `GenerateWebcrmSdk` build configuration and build the project.

The webCRM SDK is generated from [the Swagger JSON file that the webCRM API exposes](https://api.webcrm.com/swagger/v1/swagger.json).

(It should only have been the `WebcrmApiClient` project that is build when running this command, but Visual Studio keeps adding the other projects to the `GenerateWebcrmSdk` configuration whenever it touches the solution file, and it became too annoying to remove them.)

## Publishing

Publishing is only possible once the Azure rources have been created.

### Creating the Azure Resource Group

Deployment information from this article: [Deploy resources with Resource Manager templates and Azure PowerShell](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy).

#### Only Once: Connect to Azure

Connect to Azure. This only has to be run once.

    # Run in the deployment folder.
    .\Connect.ps1

#### Create Resource Group and Resources

Create the resource group and the resources inside it. The postfix of the name is specified as a parameter. It has to be unique. The example below creates are resource group with the name `erp-int-test4`.

    # Run in the deployment folder.
    .\Create-Resources.ps1 test4

#### Debugging the ARM Template

Un-comment the `-Debug` parameter in `Create-Resource.ps1`.

### Publish the Code

Publish the code through Visual Studio 2017, but right clicking on the `FunctionApps` project and selecting `Publish...`. Profiles for the staging and production have been created.

The password for publishing in stored in a `.pubxml.user` file. The file is generated when creating a publishing account. If you change your password you can generate a new password file by recreating the publishing profile - don't know if there is a smarter way to do this.

#### Publishing Using the Command Line

The [npm packages with Azure Functions Core Tools](https://www.npmjs.com/package/azure-functions-core-tools) can be used to publish the code using the command line. Add `@core` to get the .NET Core version.

    # Install the tools.
    npm install --global azure-functions-core-tools@core

    # Build and publish.
    func azure functionapp publish {appName} --dotnet-cli-params -- "--configuration Release"

There is bit more info publishing in this [issue on GitHub](https://github.com/Azure/azure-functions-core-tools/issues/670).

## Configuration Settings

The configruation settings are stored in three places,  `local.settings.json`, `TestSettings.cs`, and in the Azure Portal. If a key is added, it should generally be added all three places.

`local.settings.json` is used when hosting the environment locally, that is running the code within Visual Studio.

`TestSettings.cs` is just a convinient duplicate of `local.settings.json`, that makes it a lot simpler to read the settings in the automated tests.

When hosting on Azure the settings in `local.settings.json` are completely ignored, so all settings have to be duplicated in the Azure Portal.

## Documentation

### Azure Storage Message Queue

It is a good idea to install the [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/). This allows you to view the Azure Storage Queues and to enqueue and dequeue messages manually.

Walkthroughs showing how to [create an Azure Message Queue](https://docs.microsoft.com/en-us/azure/azure-functions/functions-integrate-storage-queue-output-binding) and to [create a triggered function by Azure Queue Storage](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-storage-queue-triggered-function).

We are favouring Azure Storage Queues and opposed to Service Bus.

Each ERP system will have its own queue e.g. one for Fortnox and another for PowerOffice. The queue is shared by all webCRM customers.

We cannot at this point in time have seperate queues per webCRM Client as the triggered queue name needs to be defined at compile time e.g.

```csharp
[QueueTrigger("Fortnox")] string queueItem ...
```

### Azure Timer Triggers

For the `[TimerTrigger]` there is an online [Azure Functions - Time Trigger Cheat sheet](https://codehollow.com/2017/02/azure-functions-time-trigger-cron-cheat-sheet/).

## Synchronisation Overview

The ERP system is the owner of the data. It is IDs in the ERP system that are used to identify elements and the synchronisation is mostly from the ERP system to webCRM. The IDs in the ERP system are stored in custom fields in webCRM. The custom fields used for this have to configurable for each webCRM system.

Synchronisations are based polling regulary for updates, even if an API has webhooks. Details about this in the [PowerOffice documenation](./PowerOffice.md).

Synchronisation operations are idempotent so that it doesn't matter if we retry, if something fails.

### Full Synchronisation

It should be possible to start a full synchronisation from the ERP system to webCRM. This will be used for the initial synchronisations and for catching up.

Order history (archived orders) should be able to be batch updated from the ERP system to webCRM.

## Notes

`GeneralUtilities` and `TestUtilities` are kept as separate projects, so that the NuGet packages required by `TestUtilities` don't get included into main code.

Prefer douplicated code over the wrong abstraction. Some parts of the code can probably be shared between PowerOffice and Fortnox, but it has deliberately been postponed to create the shared code.

When we fetch upserted items from PowerOffice (exception invoices) we get full items back. When we fetch upserted items from Fortnox we get items that have just a subset of the fields, and we have to make an extra call to get the full item.

Fetching persons from PowerOffice requires making an API call for each organisation in that system. Making all these calls takes a lot of time, and this is the major hurdle in the solution's ability to scale. A ticket has been created about this. The best solution is to convince PowerOffice to add an endpoint to their API where persons can be fetched across all organisations.

If we create new entitites too fast in webCRM using the API, some of the creates will fail because of the way we generate new IDs for them. Fortunately this means that the whole queue message fails, and ususally it will work again on the second try.

We have to turn off webCRM's ability to number the deliveries because if the system accidentally creates two deliveries with the same number, the creation will succeed, but it will not be possible to edit either delivery afterwords. The result is that both deliveries are locked from being edited by the API until the number on one of them is changed or one of them is deleted.

Azure Functions will automatically scale up and dequeue multiple messages at a time, if there are a lot of messages in the queue. This can result in the a 'create person' is dequeued before the associated 'created organisation' has been handled, resulting in a failure the 'create person'. The retry should automatically fix this.

The default queue retry is to retry 5 times with 10 seconds interval, so messages are only retried for a minute before they are put on the poinson queue.

There are a couple of places in the code where we throttled requests to the APIs. Search the code base for `millisecondsDelayBetweenCalls`.

If there was only one heartbeat and the same entity is edited in both systems at the same time, the result would be that this entity would hereafter by copied back and forth between the two systems on each heartbeat. This back and forth synchronization is avoided by using separate heartbeats for each synchronization direction, with the drawback that if an entity is edited in both systems at once, one of the edits will be discarded.

When an item in copied from source to target system, that item will then be picked up the next time the target system is scanned for recently upserted items. To avoid that items are then copied back and forth on every heartbeat we check if an upserted item contains any relevant changes before updating it.

Synchronizing an item is split in two parts: 1) Find the upserted item and enqueue it and 2) dequeue the item and copy it to the target system. If something goes wrong in the second part we simply throw an exception and trust that the message will eventually be put on the poison queue. We do not have that luxury when errors happen in the first part, so we have to avoid throwing exceptions in the first part, and generally put as much as possible of the logic in the second part.

We only have one queue for each ERP system, so when making a catch up synchronization we are slowing down the synchronization all the other customers. One day this might become an issue, and the solution is probably to add separte queues for these big catch up synchronizations.

## Debugging a Message from the Poinson Queue

1. Make sure the PowerOffice URLs on the development environment point to the correct PowerOffice environemnt.
2. Copy the message from the poison queue to development environemnt queue.
3. Dequeue the message using the the dequeue test method.

## Why C#?

- C# is the main language for Azure Functions. The new [Durable Functions](https://docs.microsoft.com/en-us/azure/azure-functions/durable-functions-overview) are currently only available for C#.
- Everybody on our team knows C#.
- No descriminated unions, though. Missing them because REST APIs often return a different JSON when they fail.
- It seems that C# is the primary language for SDKs (PowerOffice Go, Fortnox).
- JavaScript was dismissed because it is untyped.
- TypeScript was dismissed because the support is still in beta.
- F# was dismissed because it was considered too risky to switch to a functional language.`