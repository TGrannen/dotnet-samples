# Http REPL

This project is just a simple Web API that can be used to play around with the [Http REPL](https://docs.microsoft.com/en-us/aspnet/core/web-api/http-repl) tool. There is nothing special about this project to use the tool, but this is just a simple stand alone project that can be used for experimentation.

Http REPL Microsoft Documentation on how to use it from the terminal: https://aka.ms/http-repl-doc

### Example Terminal commands
```shell
httprepl https://localhost:5001

# Http REPL print top level apis
https://localhost:5001/> ls
.                 []
api               []
WeatherForecast   [GET]

https://localhost:5001/> cd WeatherForecast
/WeatherForecast    [GET]

https://localhost:5001/WeatherForecast> get
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Sat, 24 Sep 2022 20:11:57 GMT
Server: Kestrel
Transfer-Encoding: chunked

[
  {
    "date": 9/25/2022 4:11:58 PM,
    ...
  }
]

https://localhost:5001/WeatherForecast>
```

### Launch from Visual Studio
Follow the instruction [Here](https://devblogs.microsoft.com/aspnet/httprepl-a-command-line-tool-for-interacting-with-restful-http-services/) to install HttpRepl via the dotnet CLI and set it up to work in Visual Studio.
