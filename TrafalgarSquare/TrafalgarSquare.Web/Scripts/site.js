$(document).ready(function() {
    if (sessionStorage.success) {
        notify.success(sessionStorage.success);
        //sessionStorage.clear("success");
        console.log("here");
    }
    if (sessionStorage.error) {
        notify.error(sessionStorage.error);
        //sessionStorage.clear("error");
        console.log("here");
    }
});

var notify = (function () {
    function notyShow(message, type) {
        noty({
            text: message,
            animation: {
                open: { height: 'toggle' }, // jQuery animate function property object
                close: { height: 'toggle' }, // jQuery animate function property object
                speed: 500, // opening & closing animation speed
                type: type
            }
        });
    }

    var error = function(message) {
        notyShow(message, "error");
    }
    var success = function(message) {
        notyShow(message, "success");
    }

    return {
        error: error,
        success: success
    }
    
})();