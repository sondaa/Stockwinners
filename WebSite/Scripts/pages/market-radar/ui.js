$(function ()
{
    var activeTradersHub = $.connection.activeTradersHub;
    var dataModel = new ActiveTradersViewModel();

    activeTradersHub.addNewsElement = function (data)
    {
        dataModel.addNewsElement(data);
    };

    activeTradersHub.resetState = function (data)
    {
        dataModel.resetItems();
    };

    // Turn on "Hot Stocks" by default
    $("#hot-stocks").attr("checked", true);
    dataModel.addFilter("Hot Stocks");

    // Create buttons out of all categories
    $(".category").button().click(function ()
    {
        if ($(this).attr("checked"))
        {
            dataModel.addFilter($(this).attr("data-filter-name"));
        }
        else
        {
            dataModel.removeFilter($(this).attr("data-filter-name"));
        }

        // Refresh the data model to update the news elements based on new filters
        dataModel.refresh();
    });

    // Create buttons out of all display options
    $(".option").button();

    // Settings menu toggler
    $("#settings-toggle-input").button({
        icons: {
            secondary: "ui-icon-triangle-1-s"
        }
    }).click(function ()
    {
        if ($("#settings-toggle-input").attr("checked"))
        {
            $(this).button("option", "icons", { primary: null, secondary: "ui-icon-triangle-1-n" });
            $("#settings-div").show("fast");
        }
        else
        {
            $(this).button("option", "icons", { primary: null, secondary: "ui-icon-triangle-1-s" });
            $("#settings-div").hide("fast");
        }
    });

    // Clear search button
    $("#clear-search-button").button().click(function ()
    {
        $("#search-text-input").val("");
    });

    var hubConnection = $.connection.hub;

    // When the client reconnects, ensure that we start off of a clean state
    hubConnection.reconnected(function ()
    {
        dataModel.resetItems();

        // Notify the server that we want to listen to items
        activeTradersHub.clientInitialize();
    });

    hubConnection.stateChanged(function (change)
    {
        if (change.newState === $.signalR.connectionState.reconnecting)
        {
            $("#status-div").html("Reconnecting...");
        }
        else if (change.newState === $.signalR.connectionState.connected)
        {
            $("#status-div").html("Connected");
        }
        else if (change.newState === $.signalR.connectionState.disconnected)
        {
            $("#status-div").html("Disconnected");
        }
        else if (change.newState === $.signalR.connectionState.connecting)
        {
            $("#status-div").html("Connecting...");
        }
    });

    // Start connection to server
    hubConnection.start().done(function ()
    {
        // This custom handler will change the way symbols are presented in the UI so that a ; delimited text 
        // text is space delimited so that the text can be wrapped. Also, we change NOSYMBOL to the empty string so that the
        // user does not see NOSYMBOL.
        ko.bindingHandlers.symbols = {
            init: function (element, valueAccessor)
            {
                var dataValue = valueAccessor()().toString();

                if (dataValue == "NOSYMBOL")
                {
                    $(element).text("");
                }
                else
                {
                    var separatedSymbols = dataValue.split(";");

                    if (separatedSymbols.length <= 2)
                    {
                        // If up to 2 symbols are being shown, then show the entire text
                        $(element).html("<a href='http://finance.yahoo.com/q?s=" + separatedSymbols.join(" ") + "' target='_blank'>" + separatedSymbols.join(" ") + "</a>");
                    }
                    else
                    {
                        // If more than 2 symbols exist, add a "..." and show the complete list on hover
                        $(element).html("<a href='http://finance.yahoo.com/q?s=" + separatedSymbols[0] + "' target='_blank'>" + separatedSymbols[0] + "</a> ...");
                        $(element).attr("title", separatedSymbols.join(" "));
                    }
                }
            },
            update: this.init
        };

        // Custom binding handler to show the quote of the stock
        ko.bindingHandlers.quote =
        {
            init: function (element, valueAccessor)
            {
                var symbol = valueAccessor()().toString();

                // Show quote if the symbol is a single stock name
                if (dataValue == "NOSYMBOL")
                {
                    $(element).text("");
                }
                else
                {
                    var separatedSymbols = dataValue.split(";");

                    if (separatedSymbols.length <= 2)
                    {
                        // If up to 2 symbols are being shown, then show the entire text
                        $(element).html("<a href='http://finance.yahoo.com/q?s=" + separatedSymbols.join(" ") + "' target='_blank'>" + separatedSymbols.join(" ") + "</a>");
                    }
                    else
                    {
                        // If more than 2 symbols exist, add a "..." and show the complete list on hover
                        $(element).html("<a href='http://finance.yahoo.com/q?s=" + separatedSymbols[0] + "' target='_blank'>" + separatedSymbols[0] + "</a> ...");
                        $(element).attr("title", separatedSymbols.join(" "));
                    }
                }
            },
            update: this.init
        };

        // Start binding our UI data
        ko.applyBindings(dataModel);

        // Notify the server that we want to listen to items
        activeTradersHub.clientInitialize();
    });
});