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

    // Construct a local data source based off of what the server provides
    self.datasource = upshot.RemoteDataSource({
        // Location of WebAPI returning the data
        providerParameters: { url: "/api/activetraders", operationName: "GetNewsElements" },
        provider: upshot.DataProvider,
        // Type of entity the server returns
        entityType: "ActiveTradersNewsElement:#WebSite.Models",
        // Constructor to create a news element from data added manually to the data source
        mapping: NewsElement,
        // Don't update the remote server unless manually asked to
        bufferChanges: true
    }).refresh();

    self.newsElements = self.datasource.getEntities();

    // Operations
    self.addNewsElement = function (properties)
    {
        self.newsElements.unshift(ko.observable(new NewsElement(properties)));
    };
}