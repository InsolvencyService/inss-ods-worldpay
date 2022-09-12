@description('The name of the function app that you wish to create.')
param appName string = 'fnapp${uniqueString(resourceGroup().id)}'

@description('Location for all resources.')
param location string

@description('Storage account name.')
param storageAccountEndpoint string

@description('Application Insight instrumentation key.')
param instrumentationKey string

@description('The language worker runtime to load in the function app.')
@allowed([
  'node'
  'dotnet'
  'java'
])
param runtime string = 'dotnet'

var functionAppName = appName
var hostingPlanName = appName

var functionWorkerRuntime = runtime

resource appServicePlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: 'nomensa-${hostingPlanName}'
  location: location
  sku: {
    name: 'Y1'
    tier:'Dynamic'
  }
  properties: {
    reserved:true
  }
}

resource functionApp 'Microsoft.Web/sites@2018-11-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: storageAccountEndpoint
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: instrumentationKey
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: functionWorkerRuntime
        }
      ]
    }
  }
}
