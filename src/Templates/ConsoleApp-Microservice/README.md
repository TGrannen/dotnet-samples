# "Console" Based Microservice

This project is designed to be a example template for creating a background process application with .Net. However, this is still an ASP.NET project as a microservice application might need to have some way to expose Health Metrics about how it's performing so that other mechanisms can decide if any action needs to be taken. Therefore, this is not a `Console` purely for that reason. It instead has health endpoints exposed and a dummy Background service that illustrates how to configure an actual service to perform some set of tasks.

I got this idea originally from this [Video](https://youtu.be/FGsx7o7e-HU) and this project is based very closely off of what is shared and discussed there.
