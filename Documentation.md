# Руководство пользователя

## Требования

- OC Windows или Linux;

- .NET Core v1.1;

## Инструкция по установке

### Инструкция для Windows

- Для разработки требуется Visual Studio 2015 или Visual Studio Code с установленным .NET Core SDK v1.1.x
- В качестве веб-сервера можно использовать IIS или Kestrel [Web server implementations in ASP.NET Core][Kestrel].

### Инструкция для Linux

- Для разработки требуется Visual Studio Code с установленным .NET Core SDK v1.1
- Инструкция развёртывания ASP.NET Core и приложения на Ubuntu можно посмотреть в статье [Разворачиваем и демонизируем ASP.NET Core приложение под Linux в виде фонового сервиса][Linux_Man]
- В качестве веб-сервера можно использовать [Kestrel][Kestrel], Nginx или Apache

### Общие действия и команды

- Перейдите в папку \pilot-web-client-netcorerelease\Ascon.Pilot.WebClient\src\Ascon.Pilot.WebClient

Выполняем команды:

- dotnet build
- dotnet run - для проверки работы приложения, переходим в браузере по адресу localhost:5000/
- dotnet publish - для подготовки дистрибутива к развертыванию на сервере
- Опубликованные файлы находятся по адресу: \pilot-web-client-netcorerelease\Ascon.Pilot.WebClient\src\Ascon.Pilot.WebClient\bin\Debug\netcoreapp1.1\publish\
- Переходим в папку publish
- dotnet Ascon.Pilot.WebClient.dll - проверяем работу дистрибутива
- переносим папку publish на сервер
- выполняем настройку сервера в зависимости от вашей ОС

## Ссылки

1. [Kestrel]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/?tabs=aspnetcore1x "Web server implementations in ASP.NET Core" [Web server implementations in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/?tabs=aspnetcore1x)
2. [Linux_Man]: https://habrahabr.ru/post/332920/ "Разворачиваем и демонизируем ASP.NET Core приложение под Linux в виде фонового сервиса" [Разворачиваем и демонизируем ASP.NET Core приложение под Linux в виде фонового сервиса](https://habrahabr.ru/post/332920/) 