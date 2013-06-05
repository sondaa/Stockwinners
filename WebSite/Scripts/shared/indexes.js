// Show market indexes
$(function ()
{
    var intervalId = 0;
    var url = "http://query.yahooapis.com/v1/public/yql?q=select%20LastTradePriceOnly%2C%20Change%2C%20PercentChange%2C%20LastTradeTime%20from%20yahoo.finance.quotes%20where%20symbol%20in%20('QQQ'%2C%20'SPY'%2C%20'DIA')&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=?";

    updateIndexQuote = function (data, indexName)
    {
        $("#span-" + indexName + "-price").html(data.LastTradePriceOnly);
        $("#span-" + indexName + "-change").html(data.Change);
        $("#span-" + indexName + "-percent").html(data.PercentChange);

        var positive = data.Change >= 0;

        $("#span-" + indexName + "-change").toggleClass("green-quote", positive);
        $("#span-" + indexName + "-change").toggleClass("red-quote", !positive);
        $("#span-" + indexName + "-percent").toggleClass("green-quote", positive);
        $("#span-" + indexName + "-percent").toggleClass("red-quote", !positive);
    };

    fetchQuotes = function ()
    {
        $.getJSON(url, function (data)
        {
            if (data.query.results)
            {
                updateIndexQuote(data.query.results.quote[0], "nasdaq");
                updateIndexQuote(data.query.results.quote[1], "sp");
                updateIndexQuote(data.query.results.quote[2], "djia");

                $("#div-exchanges").show();

                // If the market is closed, stop requesting quotes
                if (intervalId != 0 && Date.parse("01/01/2012 4:00PM") <= Date.parse("01/01/2012 " + data.query.results.quote[0].LastTradeTime))
                {
                    clearInterval(intervalId);
                }
            }
        });
    };

    // Get quotes on lead
    fetchQuotes.call();

    // Update the quotes every two seconds
    intervalId = window.setInterval(fetchQuotes, 2000);
});