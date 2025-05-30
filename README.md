# data-stream-train-machine-learning-api [POC]
Adaptable deep neural networks to learn directly from data streams
## Prereqs
- Docker Desktop
- .NET 8.0 SDK

## Dockers:
```
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password_123#" -p 1500:1433 --name sql_server_container mcr.microsoft.com/mssql/server
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
docker run -p 8888:8888 -p 7173:7173 jupyter/minimal-notebook start-notebook.sh --NotebookApp.token=''
```
## Example input data for TrainNetwork method
```
Xs: 1,2,3,4,5
Ys: 1,1,1,1,1,1,1,1,1,1
```
# Todos

https://github.com/SciSharp/Numpy.NET
https://codeberg.org/Michieal/TorchSharp

# Ideas
Use RNN or CNN or graph to be able to handle vary length of the input layer.
Compare dynamic-learn neural network with regular one by using the same dataset.
Create more specific dataset.
   
# Implementation order
1. Create adaptive neural networks.
3. Prepare data sets: training, test. Shuffle. Merge more than two data sets, shaffle and split into test and train.
5. Compare their features (loss, accuracy) by using test data set.
6. Specify metadata of the model (version, date, time, loss, accuracy, etc.)
8. Hosted service to separate service.
9. many models in parallel?

 # Books
 https://www.oreilly.com/library/view/uczenie-gebokie-od/9788328365971/

 https://www.oreilly.com/library/view/uczenie-maszynowe-z/9788328360020/

Pick up Thu
Uczenie maszynowe z użyciem Scikit-Learn, Keras i TensorFlow - książka
116,99 PLN
Empik.com
(3)
