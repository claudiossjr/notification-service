# Rate-Limited Notification Service
This project shows an implementation of a Rate-limited notification service.
It was developed using a local cache and has no persistence for the rate limit rules. Please do not use it in Production since it was only created as an MVP.

Running the project can be done as described below.

## !! Important Note
In order to help customers to write expiration time expression easily, it has a time expression parser. Expiration time expression should be written as describred below:
- 1 hour -> "1h"
- 1 hour and 30 minutes -> "1h:30m"
- 2 days and 2 hours -> "2d:2h"

**Association Table**:
- Days -> d
- hours -> h
- minutes -> m
- seconds -> s

Example with every symbol: "1d:2h:30m:14s" 

> Note that the time expression should not contain a repeated identifier. Otherwise it will not be able to register the rule.



## Running using dotnet command

> **Pre-reqs**:
<br> - dotnet 8 installed

If you have dotnet 8 installed on your machine, you can just run `dotnet run --project src/notification/api/Notification.Api.csproj` command. And, it will run the project. Swagger can be found on `http://localhost:5281/swagger/index.html`.


## Running using **MakeFile**

> **Pre-reqs**:
<br> - make installed
<br> - dotnet 8 installed

A Makefile was created to make it easy to run locally. If you have dotnet 8 installed on your machine, you can just run `make run-api` command. And, it will run the project. Swagger can be found on `http://localhost:5281/swagger/index.html`.

## Running using **docker**

> **Pre-reqs**:
<br> - docker installed

To run using docker you have to follow the following steps:

### Build docker image
You can run using `docker build . -t <you choose the name>`.

### Running the image
To run the image please use `docker run -d -p <you choose a port number>:80 -e ASPNETCORE_ENVIRONMENT=Development <your image chosen name>`. After that, you can find swagger on the following route `http://localhost:<chosen port number>/swagger/index.html`. A docker port can be either **80** if it is not being used in your machine or another you want, since it's available.

