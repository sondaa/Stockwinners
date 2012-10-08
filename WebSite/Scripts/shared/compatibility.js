// Ensure that the client's browser supports all the features we require
Modernizr.addTest("compatiblebrowser", function ()
{
    // Check that the browser supports the html 5 dataset feature
    var elem = document.createElement('div');
    return !!elem.dataset;
});

// If the browser does not support what we need, then show the incompatible browser box
$(function ()
{
    if (!Modernizr.compatiblebrowser)
    {
        $("#browser-warning").show();
    }
});