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

    // Whether to show quotes for symbols
    $("#show-quotes").click(function ()
    {
        dataModel.showQuotes($(this).attr("checked"));
    });

    // Whether to show color coding
    $("#color-code").click(function ()
    {
        dataModel.showColorCoding($(this).attr("checked"));
    });

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
        $("#search-text-input").val("").focus();
        dataModel.setTextFilter("");
        dataModel.refresh();
    });

    // Search text input
    $("#search-text-input").keyup("input", function ()
    {
        dataModel.setTextFilter($(this).val());
        dataModel.refresh();
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
            }
        };

        // Custom binding handler to show the quote of the stock
        ko.bindingHandlers.quote =
        {
            init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext)
            {
                var symbol = ko.utils.unwrapObservable(valueAccessor()).toString();

                // If the symbol represents a single stock, then show its change
                if (symbol.length <= 5 && symbol.split(";").length == 1)
                {
                    // This binding is within the context of a foreach on filteredElements
                    // of the data source, as such, we ask for the parent to get to the main
                    // view model
                    var quote = bindingContext.$parent.quotes[symbol.toLowerCase()];

                    // Set the current value
                    if (quote() === undefined)
                    {
                        element.innerText = "Loading";
                    }
                    else
                    {
                        element.innerText = (quote() * 100).toFixed(2) + "%";
                    }

                    // Subscribe for any future updates
                    quote.subscribe(function (newValue)
                    {
                        $(element).text((newValue * 100).toFixed(2) + "%");
                    });
                }
            },
        };

        // Custom binding handler to show the quote of the stock
        ko.bindingHandlers.color =
        {
            init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext)
            {
                var symbol = ko.utils.unwrapObservable(valueAccessor()).toString();

                // If the symbol represents a single stock, then show its change
                if (symbol.length <= 5 && symbol.split(";").length == 1)
                {
                    // Convert stock value to color
                    var getColor = function (stockValue)
                    {
                        var percentMove = (quote() * 100);

                        // We consider +/- 15% to be max of range
                        percentMove = Math.min(15, Math.max(-15, percentMove));

                        var red = 0;
                        var blue = 0;
                        var green = 0;

                        if (percentMove < 0)
                        {
                            red = Math.round(Math.min(255, Math.max(0, ((Math.log(Math.abs(percentMove)) + 2) * 40))));
                        }

                        if (percentMove > 0)
                        {
                            green = Math.round(Math.min(255, Math.max(0, ((Math.log(percentMove) + 2) * 40))));
                        }

                        var redHex = red.toString(16);
                        var greenHex = green.toString(16);
                        var blueHex = blue.toString(16);

                        if (redHex.length == 1)
                        {
                            redHex = '0' + redHex;
                        }

                        if (greenHex.length == 1)
                        {
                            greenHex = '0' + greenHex;
                        }

                        if (blueHex.length == 1)
                        {
                            blueHex = '0' + blueHex;
                        }

                        return '#' + redHex + greenHex + blueHex;
                    };

                    // This binding is within the context of a foreach on filteredElements
                    // of the data source, as such, we ask for the parent to get to the main
                    // view model
                    var quote = bindingContext.$parent.quotes[symbol.toLowerCase()];

                    // Set the current value
                    if (quote() === undefined)
                    {
                        // Leave things unchanged if the value has not loaded yet
                    }
                    else
                    {
                        element.style.backgroundColor = getColor(quote());
                    }

                    // Subscribe for any future updates
                    quote.subscribe(function (newValue)
                    {
                        element.style.backgroundColor = getColor(quote());
                    });
                }
            },
        };

        // Start binding our UI data
        ko.applyBindings(dataModel);

        // Notify the server that we want to listen to items
        activeTradersHub.clientInitialize();
    });
});