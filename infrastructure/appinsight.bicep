param appInsigintName string
param location string = resourceGroup().location

resource appInsighht 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsigintName
  location: location
  kind:'web'
  properties: {
    Application_Type:'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery:'Enabled'
  }
}

output instrumetationKey string = appInsighht.properties.InstrumentationKey
