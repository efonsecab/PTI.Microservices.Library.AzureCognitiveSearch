# PTI.Microservices.Library.AzureCognitiveSearch

This is part of PTI.Microservices.Library set of packages

The purpose of this package is to facilitate the calls to Azure Cognitive Search APIs, while maintaining a consistent usage pattern among the different services in the group

**Examples:**

## Upload Document
    AzureCognitiveSearchService azureCognitiveSearchService = new AzureCognitiveSearchService(null,
        this.AzureCognitiveSearchConfiguration, 
        new Microservices.Library.Interceptors.CustomHttpClient(
            new Microservices.Library.Interceptors.CustomHttpClientHandler(null)));
    CustomModel customModel = new CustomModel()
    {
        id=Guid.NewGuid().ToString(),
        PersonName="Test",
        ImageUrl= [IMAGEURL]
    };
    var result=await azureCognitiveSearchService.UploadDocumentAsync<CustomModel>(customModel);

## Search
    AzureCognitiveSearchService azureCognitiveSearchService = new AzureCognitiveSearchService(null,
        this.AzureCognitiveSearchConfiguration,
        new Microservices.Library.Interceptors.CustomHttpClient(
            new Microservices.Library.Interceptors.CustomHttpClientHandler(null)));
    var result = await azureCognitiveSearchService.SearchAsync<CustomModel>("Test", AzureCognitiveSearchService.SearchType.Exact);
    Assert.IsTrue(result.Count() > 0);
    result = await azureCognitiveSearchService.SearchAsync<CustomModel>("Tes", AzureCognitiveSearchService.SearchType.Fuzzy);
    Assert.IsTrue(result.Count() > 0);