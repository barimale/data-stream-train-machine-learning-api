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

# Todos
https://www.google.com/search?q=vary+input+layer+neural+network&oq=&gs_lcrp=EgZjaHJvbWUqCQgAECMYJxjqAjIJCAAQIxgnGOoCMgkIARAjGCcY6gIyCQgCECMYJxjqAjIJCAMQRRg7GMID0gEGLTFqMGo3qAIEsAIB8QXvRy7F_TusyPEF70cuxf07rMg&client=ms-android-samsung-ga-rev1&sourceid=chrome-mobile&ie=UTF-8

https://code-maze.com/masstransit-rabbitmq-aspnetcore/

https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-9.0&tabs=visual-studio

# Ideas
Reduce amount of epochs based on maximal execution time of the api call.
Reduce amount of steps based on maximal execution time of the api call.
Rewrite the POC by using rabbitMQ. Monitor the queue.
Use adam optimizer.
Use RNN or CNN or graph.
