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
Use RNN or CNN or graph to be able to handle vary length of the input layer.
Compare slow-learn neural network with regular one by using the same dataset.
Create more specific dataset and ingest data to the service from javascript of WPF.
Add signalR endpoint to feed service faster.

# Reqs
1. Single api call execution time limit: 500ms
2. Loss value less than 0.3
   
# Implementation order
1. Create adaptive neural networks.
2. Compare networks and reimplement by using GPU.
3. Prepare data sets: training, test. Shuffle. Merge more than two data sets, shaffle and split into test and train.
4. Slow learn networks.
5. Compare their features (loss, predictions) by using test data set.
6. Create mechanism for recreation model triggered manually or automatically based on official CI/CD for neural networks. Recreate the concept by custom tool.
7. Create neural network B for automatic recreation neural network A.
