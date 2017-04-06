﻿
function ReStart() {
    $.blockUI({ message: null });
    var opts = {
        lines: 11 // The number of lines to draw
, length: 16 // The length of each line
, width: 4 // The line thickness
, radius: 20 // The radius of the inner circle
, scale: 1 // Scales overall size of the spinner
, corners: 1 // Corner roundness (0..1)
, color: '#fff' // #rgb or #rrggbb or array of colors
, opacity: 0.25 // Opacity of the lines
, rotate: 0 // The rotation offset
, direction: 1 // 1: clockwise, -1: counterclockwise
, speed: 1 // Rounds per second
, trail: 60 // Afterglow percentage
, fps: 20 // Frames per second when using setTimeout() as a fallback for CSS
, zIndex: 2e9 // The z-index (defaults to 2000000000)
, className: 'spinner' // The CSS class to assign to the spinner
, top: '50%' // Top position relative to parent
, left: '50%' // Left position relative to parent
, shadow: false // Whether to render a shadow
, hwaccel: false // Whether to use hardware acceleration
, position: 'absolute' // Element positioning
    }
    var target = document.getElementById('foo')
    var spinner = new Spinner(opts).spin(target);

    setTimeout(function () {
        spinner.stop();
        location.href = "index.html";
    }, 10000);
    try {
        jQuery.ajax({
            type: 'post',
            url: 'api/StoreManager/OperateService',
            dataType: 'json',
            cache: false,
            error: function () { }
        });
    } catch (e) { }
}