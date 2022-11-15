using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using PTI.Microservices.Library.Configuration;
using PTI.Microservices.Library.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PTI.Microservices.Library.Services
{
    /// <summary>
    /// Service in charge of exposing access to Azure Cognitive Search functionality
    /// </summary>
    public sealed class AzureCognitiveSearchService
    {
        private ILogger<AzureCognitiveSearchService> Logger { get; }
        private AzureCognitiveSearchConfiguration AzureCognitiveSearchConfiguration { get; }
        private CustomHttpClient CustomHttpClient { get; }
        private SearchClient SearchClient { get; }

        /// <summary>
        /// Creates a new instance of <see cref="AzureCognitiveSearchService"/>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="azureCognitiveSearchConfiguration"></param>
        /// <param name="customHttpClient"></param>
        public AzureCognitiveSearchService(ILogger<AzureCognitiveSearchService> logger,
            AzureCognitiveSearchConfiguration azureCognitiveSearchConfiguration, CustomHttpClient customHttpClient)
        {
            this.Logger = logger;
            this.AzureCognitiveSearchConfiguration = azureCognitiveSearchConfiguration;
            this.CustomHttpClient = customHttpClient;
            this.SearchClient = new Azure.Search.Documents.SearchClient(
                new Uri(this.AzureCognitiveSearchConfiguration.Endpoint), this.AzureCognitiveSearchConfiguration.IndexName,
                new Azure.AzureKeyCredential(this.AzureCognitiveSearchConfiguration.Key));
        }

        /// <summary>
        /// Uploads a document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IndexDocumentsResult> UploadDocumentAsync<T>(T data, CancellationToken cancellationToken = default)
        {
            try
            {
                IndexDocumentsBatch<T> indexDocumentsBatch = new IndexDocumentsBatch<T>();
                indexDocumentsBatch.Actions.Add(new IndexDocumentsAction<T>(IndexActionType.Upload, data));
                var result = await this.SearchClient.IndexDocumentsAsync<T>(indexDocumentsBatch, cancellationToken: cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Search type
        /// </summary>
        public enum SearchType
        {
            /// <summary>
            /// Exact match
            /// </summary>
            Exact = 0,
            /// <summary>
            /// Similar
            /// </summary>
            Fuzzy
        }
        /// <summary>
        /// Performs a serch using the specified term
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchTerm"></param>
        /// <param name="searchType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Pageable<SearchResult<T>>> SearchAsync<T>(string searchTerm, SearchType searchType,
            CancellationToken cancellationToken = default)
        {
            string searchText = searchTerm;
            SearchOptions searchOptions = new SearchOptions();
            switch (searchType)
            {
                case SearchType.Fuzzy:
                    searchOptions.QueryType = SearchQueryType.Full;
                    searchText += "~";
                    break;
                default: break;
            }
            try
            {
                SearchResults<T> response = await this.SearchClient.SearchAsync<T>(searchText,
                    options: searchOptions);
                var result = response.GetResults();
                return result;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
