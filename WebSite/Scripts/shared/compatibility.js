// Ensure that the client's browser supports all the features we require
Modernizr.addTest("compatiblebrowser", function ()
{
    // So far, we are not dependent on any weird features that would not be supported by any reasonably modern browser
    return true;
});

// If the browser does not support what we need, then show the incompatible browser box
$(function ()
{
    if (!Modernizr.compatiblebrowser)
    {
        $("#browser-warning").show();
    }
});