param name string
param serverFarmId string

resource cookerwebappdeployment 'Microsoft.Web/sites@2020-12-01' = {
  name: name
  kind: 'linux'
  location: resourceGroup().location
  tags: {
  }
  properties: {
    serverFarmId: serverFarmId
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|5.0'
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

output id string = cookerwebappdeployment.id
