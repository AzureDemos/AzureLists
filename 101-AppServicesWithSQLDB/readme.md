# Example 101 - App Services with SQL DB
1 Apr 2018

A traditional enterprise web application architecture, consisting of a website, Restful API and SQL. Both the website and API are deployed into Azure App Service, a fully managed platform to run and scale applications on Windows or Linux. Supported Languages include NET, .NET Core, Java, Ruby, Node.js, PHP and Python. Azure’s fully managed Azure SQL Database service, offers built-in intelligence that learns app patterns and adapts to maximise performance, reliability and data protection.

![](http://www.azurelists.com/images/architecture101.png)


Key Features | App Service Offering	
-- | --	
Support Languages | .NET, .NET Core, Java, Ruby, Node.js, PHP, or Python	
Platforms |	Windows, Linux
Maintenance | Fully Managed, high availability with auto-patching	
Tiers |	Shared Compute, Dedicated Compute, Isolated Environment	
Deployment |	Deployment Slots feature and continuous deployment with Git, TFS, GitHub & VSTS		
Scaling |	Automatically scale vertically and horizontally with customisable rules
Monitoring | View application performance and health. Integrates with Application Insights for deeper analysis
Complexity | Low barrier to entry, majority of existing web applications can be hosted with minimal adjustments * 
Portability | Can be hosted on traditional web servers such as IIS, Apache	
Security | Built-in authentication and authorization support. Additionally, if required App Service Environments provide network isolation and improved scale capabilities.
Developer Productivity | A fast and simple way to host existing or new applications. APIs, connectors and services through the Azure Marketplace. Quickly implement SSL, SSO
Enterprise | Complies with ISO standards, SOC2 accounting standards, PCI security standards and many more
Open Source | Supports WordPress, Umbraco, Joomla, Drupal and more


**If the app requires specific thing to be installed on the machine then this may not be appropriate. A few things to consider like making sure log files don’t write to local drives may be required.*
				

## Containers				
You can also deploy and run containerised web apps and take advantage of all the additional features of App Service. More information can be found at [https://azure.microsoft.com/en-gb/services/app-service/containers/](https://azure.microsoft.com/en-gb/services/app-service/containers/)	

## Links

*   [Source Code on GitHub](https://github.com/AzureDemos/AzureLists/)
*   [View Live Demo](#)


## References

*   [Azure App Service](https://azure.microsoft.com/en-gb/services/app-service/) - Quickly create powerful cloud apps using a fully managed platform
*   [Azure SQL DB](https://azure.microsoft.com/en-gb/services/sql-database/) - The intelligent relational cloud database service
*   [Swagger Open API](https://swagger.io/) - API developer tools for the OpenAPI Specification (OAS)
