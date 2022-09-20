# 1) Connect to Azure. You will be prompted to login. Successful connection returns with the account connected.
Connect-AzAccount -EnvironmentName AzureUSGovernment

# 2) Create Azure Contrainer Registry, params:
#    - Resource Group
#    - Bicep Template File
#    - Name of the container
az deployment group create --resource-group rg-cpf-dev-usgovva-001 --template-file AzureContainerRegistry.bicep --parameters acrName=containerRegistryCPF