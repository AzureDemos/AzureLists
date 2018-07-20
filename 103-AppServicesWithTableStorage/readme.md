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
Price | Table storage relatively in expense and is the cheapest of the three data stores

## Table Storage Database Design

Table Storage is relatively in expense and is designed to support high transaction volumes, but before creating your schema you should consider how you want your solution to scale and stay performant. As a NoSQL key/value store your design may be quite different to that of a traditional relational database. 

This example will not go into all concepts surrounding Table Storage design, so more information on the subject can be found on the
[Microsoft Docs](https://docs.microsoft.com/en-gb/azure/storage/tables/table-storage-design).

The context of our domain is individual users who can create lists that contain tasks. So, we need to model our database in the most efficient way for a single user to query lists and the tasks within them. 

### Table Storage Considerations
Table Storage is made up of Partitions that contains rows of data. There is no limit to the number of partitions or rows within a partition so long as the total size of the database doesn’t exceed 500TB. 
The most efficient ways to query Table Storage is to make the most use of the partition key and row keys rather than scanning the data within the table. More information on [querying table storage](https://docs.microsoft.com/en-gb/azure/storage/tables/table-storage-design-for-query).

#### Partition Key
As our user interface is based on a single user’s lists, then it would make sense to use something for the user to determine which partition their data resides in. 
A very basic example of this could be to generate 26 partitions (A-Z) and user the first letter of the user’s email address to dictate the partition. This won’t give a very even distribution as some letters will be more popular than others. 

Choosing an appropriate [partition strategy](https://docs.microsoft.com/en-us/rest/api/storageservices/designing-a-scalable-partitioning-strategy-for-azure-table-storage) will largely depend on your solutions data. As we don’t have authentication built into this first demo, our users are evenly allocated a partition on creation and stored in a UserCredentials class. 

#### Row Key
A user in our application will need to query all the lists they have and also access an individual list by its Id. The best way to achieve this for Table Storage is to use a composite key such as ‘UserID_ListID’ for our list entity.
We can then write a query to find all list entities in a partition where the RowID starts with ‘UserID_‘ or ends with ‘-ListId’ for a specific list entity. 

#### Storing Tasks
Where to store the tasks is the hardest decision and is dependent on how you plan to access the data, how many lists a user has and how many tasks within a list a user may have. For the most part, our application can query a list and get the tasks back within the list itself. 
However, the API does have a Tasks controller that contains the ability to search tasks across multiple lists based on whether they are marked as important or their due date. 
There are two options to solve this, and both have positives and negatives: 
1.	We create a second entity called ‘Task’ with a row key ‘UserId_ListID_TaskId’. This will enable us to use a ‘Partition Scan’ query to find all tasks that are import or due. We can also find all tasks for a user, a list, or and individual task using the row key. The downside is that we cannot join query’s together like we can in SQL, so we will need to perform two queries to retrieve a list and its tasks. 

2.	We store the tasks inside the list object. Table Storage doesn’t support nesting, so the tasks will need to be serialized before saving and then de-serialized on each read. This will reduce the number of queries we need to do for most of the applications functionality, but it will require our API to do some of the Task queries in code after pulling back a list. Any single entity can be up to 1MB, so we could potentially store thousands of tasks within a list before reaching any limitations. 

As discussed earlier, we need to think about how our application may be used. How many lists may a user have and how many tasks within a list would they have. As our application is intended for personal users who want to organise their daily tasks, we have made a couple of assumptions:

*	An average user won’t have more than 30 lists. 
*	As completed tasks could be purged after a set amount of time, we don’t imagine a user having more than 100 tasks per list. 

Based off our assumptions, we have chosen to go with option two and store the tasks within the list. In nearly all of the features of our application, we want the tasks to be returned with a list, so this will reduce the number of queries to the data store. If the applications functionality or use of the app by the users was to change in such a way that performing a task search feature in code caused performance issues, then we would switch to option one. 

## How to Deploy the Example
The [101 example](https://azurelists.azurewebsites.net/architectures/app-services-with-sql-db) covers the specifics of running the website and API both locally and deploying to Azure App Service. The only difference in this example is the config options for Table Storage. 

### Local Development
For local development you can use the [Azure Storage Emulator] (https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator) and set the 'StorageConnectionString' app setting in the web config to equal 'UseDevelopmentStorage=true;'

The code creates the tables and partitions, so nothing needs to be created before running the application. 

[![Table Storage Local Connection String](https://www.azurelists.com/images/tablestorage_webconfig.png)](https://www.azurelists.com/images/architecture103.png)

### Deploy to Azure

To connect the API to Azure Table storage, simply create a storage account and then put the connection string located under 'Access Keys' in the 'StorageConnectionString' app setting in the web config.

| Create Storage Account | Storage Account Connection String |
| --- | ---	|
| [![Create Storage Account](https://www.azurelists.com/images/create-storage-account.png)](https://www.azurelists.com/images/create-storage-account.png) | [![Get Connection String](https://www.azurelists.com/images/StoageAccountConnectionString.png)](https://www.azurelists.com/images/StoageAccountConnectionString.png) |

## Links

*   [Source Code on GitHub](https://github.com/AzureDemos/AzureLists/tree/master/103-AppServicesWithTableStorage)
*   [View Live Demo](https://demo.azurelists.com)
*   [101-Example Covering the Website & Design Choices](http://www.azurelists.com/architectures/app-services-with-sql-db)

## References

*   [Azure App Service](https://azure.microsoft.com/en-gb/services/app-service/) - Quickly create powerful cloud apps using a fully managed platform
*   [Azure Table Storage](https://azure.microsoft.com/en-gb/services/storage/tables/) - A NoSQL key-value store for rapid development using massive semi-structured datasets
*    [Table Storage Design Patterns](https://docs.microsoft.com/en-gb/azure/storage/tables/table-storage-design-patterns) - Practically address some of the issues and trade-offs of Table Storage designs patterns

## Disclaimer
The implementation in this project is intended for reference purpose only.

## Minimum Requirements
.Net 4.6.2

