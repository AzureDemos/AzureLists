# Example 103 - App Services with Table Storage
1 July 2018

In this example we have switched the Azure SQL DB for Azure Table Storage, a NoSQL key-value store designed for rapid development using massive semi-structured datasets.
Table storage is very affordable and enables you to store petabytes of data and scale without having to manually shard your dataset. 
It’s geo-redundant storage is replicated across regions and embraces a strong consistency model, ensuring all users accessing the data will always see the latest update.

[![App Service with SQL Architecture](https://www.azurelists.com/images/architecture103.png)](https://www.azurelists.com/images/architecture103.png)

> NOTE - this example covers the topic of Azure Table Storage as the data store. Information on Azure App Service, performance monitoring using Application Insights, the website user interface, setup/deployment and the general design choices such as MVC or SPA's can be found in the [101 example](https://azurelists.azurewebsites.net/architectures/app-services-with-sql-db).

Key Features | Azure Table Storage
-- | --	
Data | Flexible data schema, with OData-based queries
Scaling |  Store petabytes of semi-structured data
Cosmos |  Azure Cosmos DB Table API is a premium offering for table storage that offers throughput-optimized tables, global distribution, and automatic secondary indexes
Redundancy | Geo-Redundant storage, stored data is replicated three times within a region, and an additional three times in another region hundreds of miles away
Consistency | Embraces a strong consistency model. Handles systems with multiple users who are updating data stores simultaneously.
Price | Table storage is the cheapest of the three data stores

## Table Storage Database Design

todo...

## Links

*   [Source Code on GitHub](https://github.com/AzureDemos/AzureLists/tree/master/103-AppServicesWithTableStorage)
*   [View Live Demo](https://demo.azurelists.com)
*   [101-Example Covering the Website & Design Choices](https://azurelists.azurewebsites.net/architectures/app-services-with-sql-db)

## References

*   [Azure App Service](https://azure.microsoft.com/en-gb/services/app-service/) - Quickly create powerful cloud apps using a fully managed platform
*   [Azure Table Storage](https://azure.microsoft.com/en-gb/services/storage/tables/) - A NoSQL key-value store for rapid development using massive semi-structured datasets
*    [Table Storage Design Patterns](https://docs.microsoft.com/en-gb/azure/storage/tables/table-storage-design-patterns) - Practically address some of the issues and trade-offs of Table Storage designs patterns
*   [Application Insights](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-overview) - Rich performance monitoring and analytics
*   [Swagger Open API](https://swagger.io/) - API developer tools for the OpenAPI Specification (OAS)

## Disclaimer
The implementation in this project is intended for reference purpose only.

## Minimum Requirements
.Net 4.6.2

