# PortfolioAPI

A RESTful API for managing customers, portfolios, assets (stocks, bonds, funds), and transactions in an investment portfolio system.

This API is built using **ASP.NET Core**, **Entity Framework Core**, and **SQLite**, with full support for CRUD operations and integration tests.



## Features

- Manage **Customers** with their Portfolios
- Track **Portfolios** and their linked Assets
- Handle multiple Asset types: **Stocks, Bonds, Funds**
- Record and manage **Transactions** for each Asset
- Retrieve Portfolio **Performance Data** over time
- Integration tested using **xUnit** and **InMemoryDatabase**
- Logging with **Serilog**



## Technologies

- ASP.NET Core 8 Web API
- Entity Framework Core 8
- SQLite
- AutoMapper
- Serilog
- xUnit for unit/integration testing
- InMemory Database for testing



## Setup

1. Clone the repository
   
   git clone https://github.com/davidzhangsg/PortfolioAPI.git
   
   cd PortfolioAPI
   
3. Run the API   
   dotnet run --project PortfolioAPI

   

## License
This project is licensed under the MIT License.
