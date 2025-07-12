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

## API Endpoints

**Customers**

| Method | Endpoint                         | Description                  |
| ------ | -------------------------------- | ---------------------------- |
| GET    | `/api/customers`                 | Get all customers            |
| GET    | `/api/customers/{id}`            | Get customer by ID           |
| GET    | `/api/customers/{id}/portfolios` | Get customer with portfolios |
| POST   | `/api/customers`                 | Create customer              |
| PUT    | `/api/customers/{id}`            | Update customer              |
| DELETE | `/api/customers/{id}`            | Delete customer              |


**Portfolios**

| Method | Endpoint                                   | Description                   |
| ------ | ------------------------------------------ | ----------------------------- |
| GET    | `/api/portfolios`                          | Get all portfolios            |
| GET    | `/api/portfolios/{id}`                     | Get portfolio by ID           |
| GET    | `/api/portfolios/by-customer/{customerId}` | Get portfolios by customer ID |
| POST   | `/api/portfolios`                          | Create portfolio              |
| PUT    | `/api/portfolios/{id}`                     | Update portfolio              |
| DELETE | `/api/portfolios/{id}`                     | Delete portfolio              |


**Assets**

| Method | Endpoint                                 | Description                |
| ------ | ---------------------------------------- | -------------------------- |
| GET    | `/api/assets`                            | Get all assets             |
| GET    | `/api/assets/{id}`                       | Get asset by ID            |
| GET    | `/api/assets/by-portfolio/{portfolioId}` | Get assets by portfolio ID |
| POST   | `/api/assets/stock`                      | Create stock               |
| POST   | `/api/assets/bond`                       | Create bond                |
| POST   | `/api/assets/fund`                       | Create fund                |
| PUT    | `/api/assets/{id}`                       | Update asset               |
| DELETE | `/api/assets/{id}`                       | Delete asset               |


**Transactions**

| Method | Endpoint                                                       | Description                      |
| ------ | -------------------------------------------------------------- | -------------------------------- |
| GET    | `/api/transactions/search?startDate=&endDate=&page=&pageSize=` | Get all transactions (paged)     |
| GET    | `/api/transactions/{id}`                                       | Get transaction by ID            |
| GET    | `/api/transactions/by-asset/{assetId}`                         | Get transactions by asset ID     |
| POST   | `/api/transactions`                                            | Create transaction (Buy or Sell) |
| PUT    | `/api/transactions/{id}`                                       | Update transaction               |
| DELETE | `/api/transactions/{id}`                                       | Delete transaction               |


**Performance**

| Method | Endpoint                                                        | Description               |
| ------ | --------------------------------------------------------------- | ------------------------- |
| GET    | `/api/portfolios/{portfolioId}/performance?startDate=&endDate=` | Get portfolio performance |



## Setup

1. Clone the repository
   
   git clone https://github.com/davidzhangsg/PortfolioAPI.git
   
   cd PortfolioAPI
   
3. Run the API   
   dotnet run --project PortfolioAPI

   

## License
This project is licensed under the MIT License.
