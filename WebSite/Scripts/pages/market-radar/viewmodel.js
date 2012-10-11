/// <reference path="../../_references.js" />

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
        // The default provider is sufficient since it knows how to talk to an IQueryable
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

    // Whether to show quotes
    self.showQuotes = ko.observable(false);

    // Whether to show a color coding depending on the performance of the stock
    self.showColorCoding = ko.observable(false);

    // List of quotes to obtain from yahoo
    self.symbols = new Array();

    // Hash table of quotes obtained
    self.quotes = {};

    // ID of the timer used to obtain quotes from Yahoo
    self.quoteTimerIntervalId = 0;

    // Frequency of updating quotes from Yahoo
    self.quoteUpdateFrequency = 2000;

    // Query to talk to Yahoo with
    self.quoteQuery = "";

    // Filtering
    var filters = ["Hot Stocks"];
    self.textFilter = "";

    self.newsFilter = function (entity)
    {
        var entity = ($.isFunction(entity) ? entity() : entity);
        var category = entity.Category();

        for (var i = 0; i < filters.length; i++)
        {
            // If the category of the item being inspected matches that of the allowed filters, then allow this
            // item to be shown.
            if (filters[i].toUpperCase() == category.toUpperCase())
            {
                // If we don't have any text search, then return the item, otherwise ensure the item's text contains
                // what is being searched for
                if (self.textFilter == "")
                {
                    return true;
                }
                else
                {
                    return entity.Symbol().toLowerCase().indexOf(self.textFilter) != -1 || entity.Text().toLowerCase().indexOf(self.textFilter) != -1;
                }
            }
        }

        return false;
    }

    self.addFilter = function (filterText)
    {
        if ($.inArray(filterText, filters) < 0)
        {
            filters.push(filterText);
        }
    }

    self.removeFilter = function (filterText)
    {
        var filterIndex = $.inArray(filterText, filters);

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
        }
    }

    self.setTextFilter = function (filterText)
    {
        self.textFilter = filterText.toLowerCase();
    };

    self.clearTextFilter = function ()
    {
        self.textFilter = "";
    };

    self.getFilters = function ()
    {
        return filters;
    };

    // Initialize the set of elements
    self.newsElements = self.remoteDataSource.getEntities();
    self.filteredElements = self.localDataSource.getEntities();

    // Add any necessary symbol to the list that we query
    self.filteredElements.subscribe(function (newElements)
    {
        $.each(newElements, function (index, element)
        {
            var symbol = element.Symbol().toString();

            // Only request quotes for news elements that have a single symbol associated with them
            if (symbol.split(";").length == 1 && symbol.length <= 5)
            {
                self.addQuote(symbol);
            }
        });
    });

    // Operations
    self.addNewsElement = function (properties)
    {
        self.newsElements.unshift(new NewsElement(properties));
        self.localDataSource.refresh();
    };

    self.refresh = function ()
    {
        self.localDataSource.refresh();
    };

    // Recreates the YQL used to obtain quotes from Yahoo servers
    self.reconstructYQL = function ()
    {
        var queryStart = "select Symbol,LastTradePriceOnly,Change,LastTradeTime from yahoo.finance.quotes where symbol in (";
        var quotes = self.symbols.map(function (value, index, array) { return "'" + value + "'";}).join(",");

        self.quoteQuery = "http://query.yahooapis.com/v1/public/yql?q=" + encodeURIComponent(queryStart + quotes) + ")&format=json&env=" + encodeURIComponent("store://datatables.org/alltableswithkeys") + "&callback=?";
    };

    // Grabs quotes from Yahoo servers
    self.fetchQuotes = function ()
    {
        $.getJSON(self.quoteQuery, function (data)
        {
            if (data.query.results)
            {
                var firstQuote = null;

                var parseQuote = function (value)
                {
                    var change = value.Change / (value.LastTradePriceOnly - value.Change);

                    // Update observable with new change value
                    self.quotes[value.Symbol.toLowerCase()](change);
                };

                // Parse response
                if (data.query.count == 1)
                {
                    firstQuote = data.query.results.quote;
                    parseQuote(data.query.results.quote);
                }
                else
                {
                    $.each(data.query.results.quote, function (index, value)
                    {
                        if (index == 0)
                        {
                            firstQuote = value;
                        }

                        parseQuote(value);
                    });
                }

                $("#div-exchanges").show();

                // If the market is closed, stop requesting quotes
                if (self.quoteTimerIntervalId != 0 && Date.parse("01/01/2012 4:00PM") <= Date.parse("01/01/2012 " + firstQuote.LastTradeTime))
                {
                    clearInterval(self.quoteTimerIntervalId);
                }
            }
        });
    };

    // Refreshes quotes based on changes to list of quotes required
    self.updateQuotes = function ()
    {
        // Cancel any currently ongoing timers
        clearInterval(self.quoteTimerIntervalId);

        // If nobody is using the data, don't bother calculating it
        if ((self.showQuotes || self.showColorCoding) && self.symbols.length > 0)
        {
            // Schedule a timer to grab quotes
            self.quoteTimerIntervalId = window.setTimeout(self.fetchQuotes, self.quoteUpdateFrequency);
        }
    };

    self.addQuote = function (symbol)
    {
        symbol = symbol.toLowerCase();

        // If the symbol is not already in the list, then add it
        if ($.inArray(symbol,self.symbols) < 0)
        {
            // Put a placeholder observable for percent change
            self.quotes[symbol] = ko.observable(undefined);

            self.symbols.push(symbol);

            self.reconstructYQL();

            self.updateQuotes();
        }
    };

    self.resetItems = function ()
    {
        // Clear all elements
        self.remoteDataSource.getEntities().removeAll();
        self.localDataSource.refresh();

        // Clear all quotes
        self.quotes = {};
        self.symbols = [];
        self.reconstructYQL();
        self.updateQuotes();

        // Requery the servers
        self.remoteDataSource.refresh(/* options */ null, /* success */function (entities, totalCount)
        {
            // Populate the local data source once the remote data source has gotten all its data
            self.localDataSource.refresh();
        });
    };

    // Activate the filter on the local data source and read the data from the remote one.
    self.localDataSource.setFilter(self.newsFilter);
    self.remoteDataSource.refresh(/* options */ null, /* success */function (entities, totalCount)
    {
        // Populate the local data source once the remote data source has gotten all its data
        self.localDataSource.refresh();
    },
    /* error */ null);
}