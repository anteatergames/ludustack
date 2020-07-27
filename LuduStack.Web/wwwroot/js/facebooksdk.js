$(document).ready(function () {
    $.ajax({
        url: 'https://connect.facebook.net/en_US/sdk.js',
        dataType: 'script',
        cache: true,
        success: function (script, textStatus, jqXHR) {
            FB.init(
                {
                    appId: $('meta[property="fb:app_id"]').attr('content'),
                    version: 'v2.8',
                    autoLogAppEvents: true,
                    xfbml: true
                }
            );
        }
    });
});