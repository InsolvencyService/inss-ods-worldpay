param (
     [string] $resg = 'nomensa-rg',
     [string] $parametersfile = 'parameters.json',
     [string] $location ='eastus'
)

$exists = az group exists -n $resg

if ($exists  -eq 'false') {
     az group create --location $location --resource-group $resg;az deployment group create --resource-group $resg --template-file .\script.bicep --parameters $parametersfile --mode complete
} else {
     az deployment group create --resource-group $resg --template-file .\script.bicep --parameters $parametersfile --mode complete
}