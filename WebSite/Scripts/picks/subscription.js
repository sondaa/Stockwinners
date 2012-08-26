$(function ()
{
    // On toggling of follow images, change the state on the server side
    $(".follow-image").click(function ()
    {
        var onUri = "/Images/Subscriptions/follow-on.png";
        var offUri = "/Images/Subscriptions/follow-off.png";
        var image = this;
        var isOn = $(image).attr("src") == onUri;
        var ajaxUri = isOn ? "/api/picksubscription/ignore" : "/api/picksubscription/follow";

        // Make the call to the server to change the subscription state
        $.ajax({ url: ajaxUri, data: { pickId: $(image).attr("id") } }).success(function ()
        {
            $(image).attr("src", isOn ? offUri : onUri);
        });
    });
});
