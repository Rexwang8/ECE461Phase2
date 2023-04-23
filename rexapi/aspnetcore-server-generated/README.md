# IO.Swagger - ASP.NET Core 2.0 Server

API for ECE 461/Spring 2023/Project 2: A Trustworthy Module Registry

## Run

Linux/OS X:

```
sh build.sh
```

Windows:

```
build.bat
```

## Run in Docker

```
cd src/IO.Swagger
docker build -t io.swagger .
docker run -p 8080:8080 io.swagger
```
