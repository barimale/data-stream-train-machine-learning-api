# slow-train-machine-learning-api [POC]
Adaptable deep neural networks to learn directly from data streams
## Prereqs
- Docker Desktop
- .NET 8.0 SDK

## SQL server:
```
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password_123#" -p 1500:1433 --name sql_server_container mcr.microsoft.com/mssql/server

```
## Example input data for TrainNetwork method
```
Xs: 1,2,3,4,5
Ys: 1,1,1,1,1,1,1,1,1,1
```
