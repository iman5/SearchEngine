# IC Search Engine Documentation
This documentation of the Search Engine Service outlines and explains each step of its creation and configuration, in addition to expansion suggestions and future avenues.

### Project Overview
This project is a streamlined search engine capable of indexing content and facilitating multi-factor searches.

### Project Objectives
The project includes two microservices interacting with each other to respond to user indexing and searching requirements using the latest .NET 7.0.11 framework.
- The API application will incorporate two endpoints: "Index Content" and "Search Content" using a WebAPI project. 
- The UI is developed using the Single Page Application (SPA) implementation as a Blazor WASM project to connect to the backend API and consume the provided services.
### Project Research
By reviewing the current status of Search Engines available to use for this project, Lucene, ElasticSearch and Solr are the primary options, but Full-Text Search engines like MS SQL Server could be used for simpler scenarios. ElasticSearch and Solr are both based on Lucene and provide high-level functionality, while Lucene itself provides the core engine and can be customized to meet every business need. Therefore, Lucene was chosen as the primary option for the search engine.

There are various types of Analyzer objects in Lucene and other relevant objects that are used during the analysis process including Token, WhitespaceAnalyzer, SimpleAnalyzer, StopAnalyzer, and StandardAnalyzer.
StandardAnalyzer is used for this program, which is the most sophisticated analyzer and is capable of handling names, email addresses, and many more contexts. It lowers each token and removes common words and punctuations, if there are any to be found. To meet the needs of our API, and hit the purpose of the IndexContent API endpoint to index “text and metadata” and the SearchContent API endpoint to search “One or several words to be found within the content” according to the provided outline, we will configure the StandardAnalyzer to not use any stop words, as this might influence the search results, but keep the lowercase filter for normalization purposes. This approach is explained in the Lucene Search Engine Service code annotation summary in the API service on how to achieve this. A single StandardAnalyzer instance will be used here for both indexing and searching, to have a consistent result output.

Specialized searches, as mentioned in the “SearchContent” section, like a date range for the "Created Date", are supported as well, as requested by the SearchContent endpoint. From Date, a criterion to show content created after this date is present in the API and the endpoint is set to the current time, which can be replaced to either support a static date or a dynamic date via the API call to provide a requested date time range.

In the “Tech Stack” section of the outline, it was mentioned that it is encouraged to enhance functionality. Based on this premise, I looked further and found that each analyzer is appropriate for a specific domain. Therefore, I have created a table for the latest version of Lucene.NET, which is LUCENE_48, highlighted their best features, and implemented one of them called WhitespaceAnalyzer that can be added to the presented Search Engine Service to be used if requested by the API. Please let me know if this is of interest, and I can add it to the API endpoints as a parameter. With this approach, any analyzer engine can be selected at runtime by the user or API to be used on a specific set of documents. Plus, a pre-analysis engine can be added to do this task automatically, so the pre-analysis (simple or AI) will choose the appropriate Lucene Analyzer to do the indexing and searching of documents.
- UAX29URLEmailTokenizerFactory for fields that store the URLs and Email addresses.
- KeywordAnalyzer for fields that store Zip codes, Ids and product names
- WhitespaceAnalyzer for fields that store text as a document, based on whitespaces. Useful for bypassing the reserved characters of Lucene.
- SimpleAnalyzer for fields that store text as a document, based on non-letter characters and puts the - text in lowercase.
- StopAnalyzer for fields that store text as a document, based on non-letter characters, but also removes common words like 'a', 'an', 'the', etc.
- CustomAnalyzer This can be designed to meet the special needs of application documents.

### Project API & UI Testing
The project was tested extensively using the provided outline and observed relevant search results with no issues.
- Using all forms of testing from various sources, including search keys with "one or several words" required by the outline,  with uppercase, lowercase, camelcase, upper camelcase, punctuation marks, etc.
- Using metadata in different formats with the same formatting as the text testing.
- Using many date ranges with the accuracy of year, month, or day to hours, minutes, and seconds. 
- Using reserved characters of Lucene, like * to find words with a wildcard. 

### Project Structure
The project is created using a blank solution named “SearchEngine” to facilitate the use of microservices' architecture.
Two projects are added: a WebAPI project as a backend microservice called “SearchEngine.API” and a Blazor WASM project as a microfrontend service called “SearchEngine.UI”.
Both of these services are linked to the startup project under the name “Multiple Projects of Search Engine,” both running over the HTTPS protocol. With the start of the project, both of these services will run at the same time at the below addresses:
###### SearchEngine.UI Service:
- https://localhost:7120
- http://localhost:5281
###### SearchEngine.API Service:
- https://localhost:7187
- http://localhost:5036
These ports can be modified in the project launchSettings if needed. e.g., in the circumstance of ports being unavailable.
### SearchEngine.API Service Module Overview
Here are listed all the aspects of the API project, with details for each section.
###### Dependencies
Using the latest version of Lucene.net packages as well as Swashbuckle and OpenApi
- PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="7.0.11" 
- PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" 
- PackageReference Include="Lucene.Net" Version="4.8.0-beta00016" 
- PackageReference Include="Lucene.Net.Analysis.Common" Version="4.8.0-beta00016" 
- PackageReference Include="Lucene.Net.QueryParser" Version="4.8.0-beta00016" 
###### Models
- Created folder name: Models
- Included files: ContentModel and ResponseModel.
- Details:
- ContentModel is used to declare the needed properties for the indexing and searching of the content.
- ResponseModel is used in response to the requested data.
###### Controllers
- Created folder name: Controllers
- Included files: ContentController
- Details:
- ContentController is created to serve two API endpoints.
- Token-based routing is used with controller and action tokens.
- API Versioning is used with the first major release of v1.
- This controller has two endpoints of IndexContent to save and index contents via the LuceneSearchEngineService with DI and SearchContent to find hits on the indexed content.
###### Interfaces
- Created folder name: Interfaces
- Included files: ILuceneSearchEngineService
- Details:
- ILuceneSearchEngineService interface is a contract for the LuceneSearchEngineService to provide the indexing and searching of content, as well as the needed interface for the DI method.
###### Services
- Created folder name: Services
- Included files: LuceneSearchEngineService
- Details:
- LuceneSearchEngineService implements the ILuceneSearchEngineService interface to provide services for the controllers.
- This service has two methods of IndexContent to save and index contents via the indexWriter and SearchContent to find hits on the indexed content via the standardAnalyzer.
###### Helpers
- Created folder name: Helpers
- Included files: DateTimeHelper
- Details:
- DateTimeHelper class converts the DateTimeToString of yyyyMMddHHmmss and vice-versa to be consumed by the Search Engine Service.
###### Extensions
- Created folder name: Extensions
- Included files: ServiceCollections
- Details:
- This static class will be used by the program.cs to configure the needed services via ServiceCollections.
It configures Controllers, DI services, CORS policies and dev API tester Swagger.
###### Program.cs:
- Adding and configuring the services is done in this file.
- The needed middlewares are added to the app before running the program.
### SearchEngine.UI Service Module Overview
- Using the latest Bootstrap framework to design the UI elements and components.
###### Dependencies
- PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="7.0.11" 
- PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="7.0.11" PrivateAssets="all" 
###### Models
- Created folder name: Models
- Included files: ContentModel.
- Details:
ContentModel is used to declare the needed properties for the indexing and searching of the content.
Properties are decorated with the Data Annotations to provide runtime validation via type checking in the backend and JQueryValidation on the frontend.
###### Pages
- Created folder name: Pages
- Included files: Index.razor, IndexContent.razor and SearchContent.razor.
- Details:
Index.razor route is the index and home of the application.
IndexContent.razor creates a routing to the IndexContent page to facilitate the creation of new content for indexing.
SearchContent.razor creates a routing to the SearchContent page to facilitate searching of index contents with the provided CreatedDate.
###### Program.cs:
- The builder is created for the WASM in this file.
- BaseURI can be set using a variety of methods, like Azure Vault or any other service.
### Code Status
- Tested both services and no error, warning, or message was generated by the application code.
