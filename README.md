# Nautilus Mongo Client *(experiment)*
An experiment project to connect and operate on a mongo database CRUD operations
This experiment repo using the official [MongoDB C# Driver](https://docs.mongodb.com/drivers/)

## Getting started
Install MongoDB on your dev machine. The best way is to use [Docker Desktop](https://www.docker.com/products/docker-desktop). Of course you can opt to install MongoDB directly into your machine. Personally I choose docker. 

Once docker is installed open up your terminal of choice. 
run the commands below

    $ sudo docker pull mongo (latest)

OR

    $ sudo docker pull mongo:4.2.2 (specific version)

Start the Docker container with the run command using the mongo image. The /data/db directory in the container is mounted as /mongodata on the host. Additionally, this command changes the name of the container to mongodb:

    $ sudo docker run -it -v mongodata:/data/db --name mongodb -d mongo

OR

    $ sudo docker run -it -v mongodata:/data/db -p 27017:27017 --name mongodb -d mongo

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
