using System;
using SearchEngine.API.Models;

namespace SearchEngine.API.Interfaces;

/// <summary>
/// This interface is contract for the LuceneSearchEngineService to provide the indexing and searching of content.
/// IndexContent gets the contents and return bool based on success or failure of the opertaion.
/// SearchContent gets the search crteria and returns the hits of contents.
/// </summary>
public interface ILuceneSearchEngineService
{
    bool IndexContent(IEnumerable<ContentModel> contents);
    IEnumerable<ContentModel> SearchContent(string searchKey, DateTime createdDate);
}

