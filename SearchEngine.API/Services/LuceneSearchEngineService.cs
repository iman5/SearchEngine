using System;
using System.ComponentModel;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using SearchEngine.API.Helpers;
using SearchEngine.API.Interfaces;
using SearchEngine.API.Models;

namespace SearchEngine.API.Services;

/// <summary>
/// This class implements the ILuceneSearchEngineService interface and provides services for the controllers.
/// </summary>
public class LuceneSearchEngineService : ILuceneSearchEngineService
{
    /// <summary>
    /// Defining and initializing immutatable variables for the class via the ctor.
    /// </summary>
    private const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
    private readonly StandardAnalyzer standardAnalyzer;
    private readonly SimpleFSDirectory simpleFSDirectory;
    private readonly IndexWriter indexWriter;
    private readonly ILogger<LuceneSearchEngineService> logger;

    /// <summary>
    /// LuceneSearchEngineName will be the name of the created folder in the root of the dir to provide indexing.
    /// directoryPath will be the current root dir.
    /// StandardAnalyzer will be initialized with the latest version of LUCENE_48 and using an EMPTY_SET to tokenize all the input contents and not use any stopwords.
    /// SimpleFSDirectory will be initialized with the directoryPath.
    /// IndexWriterConfig is created using the StandardAnalyzer to pass to the IndexWriter.
    /// 
    /// This service will depend on the logger service with loose coupling provided by the DI container.
    /// </summary>
    /// <param name="logger"></param>
    public LuceneSearchEngineService(ILogger<LuceneSearchEngineService> logger)
    {
        string LuceneSearchEngineName = "LuceneSearchEngineIndices";
        string directoryPath = Path.Combine(Environment.CurrentDirectory, LuceneSearchEngineName);
        
        standardAnalyzer = new StandardAnalyzer(luceneVersion, Lucene.Net.Analysis.Util.CharArraySet.EMPTY_SET);
        simpleFSDirectory = new SimpleFSDirectory(directoryPath);

        var indexWriterConfig = new IndexWriterConfig(luceneVersion, standardAnalyzer);
        indexWriter = new IndexWriter(simpleFSDirectory, indexWriterConfig);

        this.logger = logger;
    }

    /// <summary>
    /// This method will get an IEnumerable of contents and will write and index the content with indexWriter.
    /// It will return a flag to indicate the success or failure of the operation.
    /// </summary>
    /// <param name="contents"></param>
    /// <returns>bool</returns>
    public bool IndexContent(IEnumerable<ContentModel> contents)
    {
        try
        {
            //Adding all the contents to the indexWriter 
            foreach (var content in contents)
            {
                Document document = new Document
                {
                    //DateTimeHelper is used to convert the datetime to string
                    new StringField(nameof(ContentModel.CreatedDate), DateTimeHelper.DateTimeToString(content.CreatedDate), Field.Store.YES),
                    new TextField(nameof(ContentModel.Content), content.Content, Field.Store.YES)
                };
                indexWriter.AddDocument(document);
            }

            //commit and respond to the request
            indexWriter.Commit();
            return true;
        }
        catch (Exception ex)
        {
            //Log exceptions and respond accordingly to the request
            logger?.LogError(ex.ToString());
            return false;
        }
    }

    /// <summary>
    /// This method will get a searchKey and createdDate to search in the indexed contents and will respond with an IEnumerable of found contents.
    /// </summary>
    /// <param name="searchKey"></param>
    /// <param name="createdDate"></param>
    /// <returns>IEnumerable<ContentModel></returns>
    public IEnumerable<ContentModel> SearchContent(string searchKey, DateTime createdDate)
    {
        try
        {
            //Opening the dir and reading the content, while setting the searchFields of the query
            var directoryReader = DirectoryReader.Open(simpleFSDirectory);
            var indexSearcher = new IndexSearcher(directoryReader);
            string[] searchFields = { nameof(ContentModel.Content), nameof(ContentModel.CreatedDate) };

            //Filtering the contents based on datetime with the provided createdDate to this datetime including the lower and upper hits.
            var filter = FieldCacheRangeFilter.NewStringRange(
                field: nameof(ContentModel.CreatedDate),
                lowerVal: DateTimeHelper.DateTimeToString(createdDate),
                includeLower: true,
                upperVal: DateTimeHelper.DateTimeToString(DateTime.Now),
                includeUpper: true);

            //Parsing the query with the created searchFields and the current standardAnalyzer.
            var queryParser = new MultiFieldQueryParser(luceneVersion, searchFields, standardAnalyzer);
            var query = queryParser.Parse(searchKey);

            //Searching the indices for matches.
            var hits = indexSearcher.Search(query, filter, 10000).ScoreDocs;

            //Re-creating the list of ContentModels to return to the requester.
            var contents = new List<ContentModel>();
            foreach (var hit in hits)
            {
                var document = indexSearcher.Doc(hit.Doc);
                contents.Add(new ContentModel
                {
                    Content = document.Get(nameof(ContentModel.Content)),
                    CreatedDate = DateTimeHelper.StringToDateTime(document.Get(nameof(ContentModel.CreatedDate)))
                });
            }

            return contents;
        }
        catch (Exception ex)
        {
            //Log exceptions and respond accordingly to the request
            logger?.LogError(ex.ToString());
            return Enumerable.Empty<ContentModel>();
        }
    }
}

