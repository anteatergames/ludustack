![ludustack](https://github.com/anteatergames/ludustack/blob/master/LuduStack.Web/wwwroot/images/logo-horizontal-black-1024w.png?raw=true)

[![License](https://img.shields.io/github/license/anteatergames/indievisible)](https://github.com/anteatergames/ludustack/blob/master/LICENSE)


| Name|Deploy|Code Factor|Codacy|
|:-|:-:|:-:|:-:|
|development|[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?branch=development&project=anteatergames_ludustack&metric=alert_status)](https://sonarcloud.io/dashboard?id=anteatergames_ludustack&branch=development)|[![CodeFactor](https://www.codefactor.io/repository/github/anteatergames/ludustack/badge)](https://www.codefactor.io/repository/github/anteatergames/ludustack)|
|master|[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?branch=master&project=anteatergames_ludustack&metric=alert_status)](https://sonarcloud.io/dashboard?id=anteatergames_ludustack&branch=master)|[![CodeFactor](https://www.codefactor.io/repository/github/anteatergames/ludustack/badge)](https://www.codefactor.io/repository/github/anteatergames/ludustack)|[![Codacy Badge](https://app.codacy.com/project/badge/Grade/d3db41f42938477499a591178186d3da)](https://www.codacy.com/gh/anteatergames/ludustack/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=anteatergames/ludustack&amp;utm_campaign=Badge_Grade)|

## Table Of Contents
- [Technical Specifications](#technical-specifications)
- [How to run](#how-to-run)
- [Preparing the environment](#preparing-the-environment)
- [License](#license)

## Technical specifications
- DDD oriented
- ASP.NET Core 3.1
- Mongodb

## Preparing the environment

Create an account at https://cloudinary.com and get the CLOUDINARY_URL provided on your dashboard.

You need the following Environment Variables on your system:
| Name           |                           Value                           |
|----------------|:---------------------------------------------------------:|
| CLOUDINARY_URL | cloudinary://{yourapikey}:{yourapisecret}@{yourcloudname} |

If you need to clean the certificate mess on your machine, run this:

*dotnet dev-certs https --clean*

If any window pops up asking if you want to delete the certificate from the root store, confirm it.

To generate a new dev certificate, run this:

*dotnet dev-certs https --trust -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p SECRETPASSWORD*

If any window pops up asking if you want to install the certificate, confirm it.

Now you have a certificate file named *aspnetapp.pfx* at *%USERPROFILE%\.aspnet\https\*.

You may see a message like *"A valid HTTPS certificate with a key accessible across security partitions was not found"*. Don't worry, this is a [known issue](https://github.com/dotnet/aspnetcore/issues/21948) and will be fixed by the dotnet team as soon as possible.

## How to run
To run the whole application inside a container, run the following command:

*docker-compose up*

This command will download the needed images from the docker hub and start two containers:

- ludustack-db;
- luduscack-web;

To run a local container of the mongodb database, run the following command:

*docker-compose -f ./docker-compose.db.yml up*

This command will start only the ludustack-db, allowing you to connect to the database by running the project using *dotnet run* command or the solution on Visual Studio.

## License
This project is licensed under the [MIT License](https://github.com/anteatergames/ludustack/blob/master/LICENSE)
