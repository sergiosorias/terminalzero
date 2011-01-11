function getMyNav() {
    if (window.navigator.userAgent.indexOf("MSIE 6") > -1)
        return "IE6";
    if (window.navigator.userAgent.indexOf("MSIE 7") > -1)
        return "IE7";
    if (window.navigator.userAgent.indexOf("MSIE 8.0") > -1)
        return "IE8";
    else if (window.navigator.userAgent.indexOf("Firefox") > -1)
        return "FF";
    else if (window.navigator.userAgent.indexOf("Opera") > -1)
        return "OP";
    else if (window.navigator.userAgent.indexOf("AppleWebKit") > -1)
        return "SA";

    return "";
}

function getClientWindowHeight() {
    if (document.documentElement)
        if (getMyNav() == "IE7" || getMyNav() == "IE8")
            return document.documentElement.offsetHeight;
        else if (getMyNav() == "FF" || getMyNav() == "SA" || getMyNav() == "IE6")
        //return document.documentElement.clientHeight;
            return document.body.clientHeight;
        else if (getMyNav() == "OP")
            return document.body.clientHeight - 16;

    return 400;
}

function getClientWindowWidth() {

    if (document.documentElement)
        if (getMyNav() == "IE7" || getMyNav() == "IE8" || getMyNav() == "IE6")
            return document.documentElement.offsetWidth;
        else if (getMyNav() == "FF" || getMyNav() == "SA")
            return document.documentElement.clientWidth;
        else if (getMyNav() == "OP")
            return document.body.clientWidth;

    return 400;
}

function getQuerystring(key, default_) {
    if (default_ == null)
        default_ = "";
    key = key.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + key + "=([^&#]*)");
    var qs = regex.exec(window.location.href);
    if (qs == null)
        return default_;
    else
        return qs[1];
}

function getXYpos(elem) {
    if (!elem) {
        return { "x": 0, "y": 0 };
    }
    var xy = { "x": elem.offsetLeft, "y": elem.offsetTop }
    var par = getXYpos(elem.offsetParent);
    for (var key in par) {
        xy[key] += par[key];
    }
    return xy;
}


function changeClass(object, cssclass) {
    if (object.className) {
        object.className = cssclass;
    }
}