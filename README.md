# tcp server test

This is the server-side component of a chat application, designed to run in a Docker container locally or on a remote host.

The Docker images are published on [DockerHub](https://hub.docker.com/repository/docker/jimmynostar/tcpservertest/general), allowing you to deploy the server on various computers or servers easily.

## Features
Listens on port 5000 by default.
Lightweight and easy to deploy with Docker Compose.

## Usage
## Example 1docker-compose.yml1:
```
services:
  tcp_server:
    image: jimmynostar/tcpservertest
    ports:
      - '7893:5000' # Map container's port 5000 to host's port 7893
    restart: unless-stopped
```


## Steps to Run:
1. Save the example `docker-compose.yml` file in your project directory.
2. Start the container
```
docker-compose up -d
```
3. The server will be accessible on the host machine at port `7893`.
