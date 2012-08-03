/// <reference path="../Scripts/_references.js" />

function ActiveTradersViewModel()
{
    var self = this;

    // Data

    // Construct meta-data required for Upshot to be able to talk to the server
    upshot.metadata({
        "ActiveTradersNewsElement:#WebSite.Models": {
            key: ["ElementId"],
            fields: {
                ElementId: { type: "Int32:#System" },
                SourceId: { type: "String:#System" },
                Text: { type: "String:#System" },
                Category: { type: "String:#System" },
                Symbol: { type: "String:#System" }
            }
        }
    });

    function NewsElement(properties)
    {
        var newsElement = this;
        newsElement.ElementId = ko.observable(properties.ElementId);
        newsElement.SourceId = ko.observable(properties.SourceId);
        newsElement.Text = ko.observable(properties.Text);
        newsElement.Category = ko.observable(properties.Category);
        newsElement.Symbol = ko.observable(properties.Symbol);

        // add properties managed by upshot
        upshot.addEntityProperties(newsElement);
    };

    // Construct a data source based off of what the server provides
    self.remoteDataSource = upshot.RemoteDataSource({
        // Location of WebAPI returning the data
        providerParameters: { url: "/api/activetraders", operationName: "GetNewsElements" },
        // The default provider is sufficent since it knows how to talk to an IQueryable
        provider: upshot.DataProvider,
        // Type of entity the server returns
        entityType: "ActiveTradersNewsElement:#WebSite.Models",
        // Constructor to create a news element from data added manually to the data source
        mapping: NewsElement,
        // Don't update the remote server unless manually asked to
        bufferChanges: true
    });

    // Create a local data source so that filtering can be done more efficiently
    self.localDataSource = upshot.LocalDataSource({ source: self.remoteDataSource, autoRefresh: false, allowRefreshWithEdits: true });

    // Filtering
    var filters = ["Hot Stocks"];
    
    self.newsFilter = function (entity)
    {
        var category = ($.isFunction(entity) ? entity() : entity).Category();

        for (var i = 0; i < filters.length; i++)
        {
            // If the category of the item being inspected matches that of the allowed filters, then allow this
            // item to be shown.
            if (filters[i].toUpperCase() == category.toUpperCase())
            {
                return true;
            }
        }

        return false;
    }

    self.addFilter = function(filterText)
    {
        if (filters.indexOf(filterText) < 0)
        {
            filters.push(filterText);
            self.localDataSource.refresh();
        }
    }

    self.removeFilter = function(filterText)
    {
        var filterIndex = filters.indexOf(filterText);

        if (filterIndex >= 0)
        {
            if (filters.length == 1)
            {
                filters = [];
            }
            else
            {
                filters.splice(filterIndex, 1);
            }

            self.localDataSource.refresh();
        }
    }

    self.getFilters = function ()
    {
        return filters;
    };

    // Initialize the set of elements
    self.newsElements = self.remoteDataSource.getEntities();
    self.filteredElements = self.localDataSource.getEntities();

    // Activate the filter on the local data source and read the data from the remote one.
    self.localDataSource.setFilter(self.newsFilter);
    self.remoteDataSource.refresh(/* options */ null, /* success */function (entities, totalCount)
    {
        // Populate the local data source once the remote data source has gotten all its data
        self.localDataSource.refresh();
    },
    /* error */ null);

    // Operations
    self.addNewsElement = function (properties)
    {
        self.newsElements.unshift(ko.observable(new NewsElement(properties)));
        self.localDataSource.refresh();
    };

    self.resetItems = function ()
    {
        self.newsElements = [];
        self.localDataSource.refresh();
    };
}