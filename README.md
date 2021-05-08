# Nautilus Mongo Client *(experiment)*
An experiment project to connect and operate on a mongo database CRUD operations
This experiment repo using the official [MongoDB C# Driver](https://docs.mongodb.com/drivers/)

## Getting started
Install MongoDB on your dev machine. The best way is to use [Docker Desktop](https://www.docker.com/products/docker-desktop). Of course you can opt to install MongoDB directly into your machine. Personally I choose docker. 

Once docker is installed open up your terminal of choice. 
Run one of the commands below:

    $ sudo docker pull mongo (latest)

Start a mongo database by getting the specific version

    $ sudo docker pull mongo:4.2.2 (specific version)

Start a mongo database in docker having the data directory persisted /data/db. This container does not expose ports to the outside world

    $ docker run --name mongodb -v /data/db mongodb

Start a mongo database in docker having the data directory persisted /data/db. This container exposes port 27017 so that a 3rd party client can connect to it.

    $ docker run --name mongodb -p 27017:27017 -v /data/db mongo

... to set the port manually.


Once the MongoDB server starts running in a container, check the status by typing:

    $ sudo docker ps

Always check the Docker log to see the chain of events after making changes:

    $ sudo docker logs mongodb

The container is currently running in detached mode. Connect to the container using the interactive terminal instead:

    $ sudo docker exec -it mongodb bash

Instead of just typing mongo, you can additionally define a specific host and port by typing:

    $ mongo -host localhost -port 27017

Stop mongodb

    $ sudo docker stop mongodb

Start mongodb

    $ sudo docker start mongodb

## References
- https://itnext.io/stop-installing-db-locally-use-docker-for-local-development-for-mongodb-or-postgresql-ff560893f38e
