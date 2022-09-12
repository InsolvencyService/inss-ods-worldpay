
var resGroupName  = resourceGroup().name

@description('Location where the resource should be deployed.')
param location string

// STORAGE ACCOUNT PARAMETRES
@description('Storage account name.')
param storageAccountName string


module storageModule 'storageaccount.bicep' = {
  name:'storageAccount'
  scope: resourceGroup(resGroupName)
  params: {
    storageName: storageAccountName
    location:  location
  }
}

module appInsightsModule 'appinsight.bicep' = {
  name:'appInsight'
  scope: resourceGroup(resGroupName)
  params: {
    appInsigintName:'nomensaappinsight'
    location: location
  }
}

var appInsigntInstrumentationKey = appInsightsModule.outputs.instrumetationKey

module functionApppModule 'function.bicep' = {
  name:'functionApp' 
  scope: resourceGroup(resGroupName)
  params: {
    appName:'WorldPayFunctionApp'
    location:location
    storageAccountEndpoint: storageModule.outputs.storageAccountEndpoint
    instrumentationKey: appInsigntInstrumentationKey
    runtime:'dotnet'
  }
  dependsOn:[appInsightsModule,storageModule]
}


