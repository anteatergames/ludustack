![ludustack](https://github.com/anteatergames/ludustack/blob/master/LuduStack.Web/wwwroot/images/logo-horizontal-black-1024w.png?raw=true)

## Table Of Contents
- [Technical Specifications](#technical-specifications)
- [How to run](#how-to-run)
- [License](#license)

## Technical specifications
- DDD oriented
- ASP.NET Core 3.1
- Mongodb

## How to run
To run the whole application inside a container, run the following command:
*docker-compose up*


To run a local container of the mongodb database, run the following command:
*docker-compose -f ./docker-compose.db.yml up*


If running inside Visual Studio, it will set the needed environment variables.

## License
This project is licensed under the [Apache License 2.0](https://github.com/anteatergames/ludustack/blob/master/LICENSE)
