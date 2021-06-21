# First run this
#Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope CurrentUser

dotnet sonarscanner begin /k:"GreenFlux" /d:sonar.host.url="http://localhost:9000"  /d:sonar.login="479f247be7d55f3c52f4df9013544038f7aa499c"

dotnet build

dotnet sonarscanner end /d:sonar.login="479f247be7d55f3c52f4df9013544038f7aa499c"

# Set-ExecutionPolicy -ExecutionPolicy Default -Scope CurrentUser