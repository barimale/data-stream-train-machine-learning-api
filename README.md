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
3. Any time model is trained, put it to the nosql.
4. Scan nosql and retrieve data with not-yet-merge piece od model and do the merge.
5. Create versioning pattern: day month year - version A,B,C...
6. Use 4 for Predict endpoint. Build the whole model from pieces, execute the predict method and return the value.
7. Build one model per day.
8. Save train entries in DB.
9. Mark saved train entries MainModelContainsEntry = true.
10. Use middleware to prepare input data of use validation to exisitng solution.
# Implementation order
1. Create adaptive neural networks.
2. Create model merge tool -> execute train logic in parallel , merge models as follows:
   This repository contains a script, py_merge.py, that can be used to merge two PyTorch model .bin files into a single model file. This can be useful when you need to combine the weights of two models that have the same architecture and are compatible.
3. https://www.bing.com/search?pglt=297&q=merge+model+pytorh&cvid=c563f2341a904a8c94bcdcd1d13cc67c&gs_lcrp=EgRlZGdlKgYIABBFGDkyBggAEEUYOdIBCDYyNzRqMGoxqAIAsAIA&FORM=ANNTA1&PC=ACTS
4. Compare networks and reimplement by using GPU.
5. Prepare data sets: training, test. Shuffle. Merge more than two data sets, shaffle and split into test and train.
6. Slow learn networks.
7. Compare their features (loss, predictions) by using test data set.
8. Create mechanism for recreation model triggered manually or automatically based on official CI/CD for neural networks. Recreate the concept by custom tool.
9. Create neural network B for automatic recreation neural network A.
10. Create many models once a day. Use vary amount of epochs, steps, layers and choose the leader.
11. Detect under and over fitting - modify amount of steps.
12. Use the leader all day long, in case of restart service, relearn the model on demand.
13. Save the request to the DB any time the call is executed by using middleware.
14. Force model creation from the first data, of between startDate and endDate.
15. Is the timeseries database usefull here? QuestDb
