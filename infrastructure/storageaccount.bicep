@minLength(3)
@maxLength(24)
param storageName string

param location string


resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: storageName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties:{
    accessTier:'Hot'
  }
}

var key = first(listKeys(storageaccount.id, storageaccount.apiVersion).keys).value

var storageAccountEndpoint ='DefaultEndpointsProtocol=https;AccountName=${storageaccount.name};AccountKey=${key};EndpointSuffix=core.windows.net'

output storageAccountEndpoint string = storageAccountEndpoint
