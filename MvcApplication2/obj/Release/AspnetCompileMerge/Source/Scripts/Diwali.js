/******************************************************************* 
* File    : JSFX_Layer.js  © JavaScript-FX.com
* Created : 2001/04/11 
* Author  : Roy Whittle  (Roy@Whittle.com) www.Roy.Whittle.com 
* Purpose : To create a cross browser dynamic layers.
* History 
* Date         Version        Description 
* 2001-03-17	3.0		Completely re-witten for use by javascript-fx
* 2001-09-08	3.1		Added the ability for child layers
* 2001-09-23	3.2		Save a reference so we can use a self referencing 
*					timer (may not be required)
* 2001-09-28	3.3		Add a width for Netscape 4.x
* 2001-09-28	3.4		Remove width for Netscape 4.x (Not needed)
***********************************************************************/
var ns4 = (navigator.appName.indexOf("Netscape") != -1 && !document.getElementById);

if (!window.JSFX)
    JSFX = new Object();

JSFX.layerNo = 0;
/**********************************************************************************/
JSFX.createLayer = function (htmlStr, parent) {
    var elem = null;

    if (document.layers) {
        var xName = "xLayer" + JSFX.layerNo++;
        if (parent == null)
            elem = new Layer(2000);
        else
            elem = new Layer(2000, parent.elem);

        elem.document.open();
        elem.document.write(htmlStr);
        elem.document.close();
        elem.moveTo(0, 0);
        elem.innerHTML = htmlStr;
    }
    else
        if (document.all) {
            if (parent == null)
                parent = document.body;
            else
                parent = parent.elem;

            var xName = "xLayer" + JSFX.layerNo++;
            var txt = '<DIV class="xlayer" ID="' + xName + '"'
                + ' STYLE="position:absolute;left:0;top:0;visibility:hidden">'
                + htmlStr
                + '</DIV>';

            parent.insertAdjacentHTML("BeforeEnd", txt);

            elem = document.all[xName];
        }
        else
            if (document.getElementById) {
                if (parent == null)
                    parent = document.body;
                else
                    parent = parent.elem;

                var xName = "xLayer" + JSFX.layerNo++;
                var txt = ""
                    + "position:absolute;left:0px;top:0px;visibility:hidden";

                var newRange = document.createRange();

                elem = document.createElement("DIV");
                elem.setAttribute("style", txt);
                elem.setAttribute("class", "xlayer");
                elem.setAttribute("id", xName);

                parent.appendChild(elem);

                newRange.setStartBefore(elem);
                strFrag = newRange.createContextualFragment(htmlStr);
                elem.appendChild(strFrag);
            }

    return elem;
}
/**********************************************************************************/
JSFX.Layer = function (newLayer, parent) {
    if (!newLayer)
        return;

    if (typeof newLayer == "string")
        this.elem = JSFX.createLayer(newLayer, parent);
    else
        this.elem = newLayer;

    if (document.layers) {
        this.images = this.elem.document.images;
        this.parent = parent;
        this.style = this.elem;
        if (parent != null)
            this.style.visibility = "inherit";
    }
    else {
        this.images = document.images;
        this.parent = parent;
        this.style = this.elem.style;
    }
    window[this.elem.id] = this;	//save a reference to this
}
/**********************************************************************************/
JSFX.findLayer = function (theDiv, d) {
    if (document.layers) {
        var i;
        if (d == null) d = document;
        var theLayer = d.layers[theDiv];
        if (theLayer != null)
            return (theLayer);
        else
            for (i = 0 ; i < d.layers.length ; i++) {
                theLayer = JSFX.findLayer(theDiv, d.layers[i].document);
                if (theLayer != null)
                    return (theLayer);
            }
        return ("Undefined....");
    }
    else
        if (document.all)
            return (document.all[theDiv]);
        else
            if (document.getElementById)
                return (document.getElementById(theDiv));
            else
                return ("Undefined.....");
}

/**********************************************************************************/
/*** moveTo (x,y) ***/
JSFX.Layer.prototype.moveTo = function (x, y) {
    this.style.left = x + "px";
    this.style.top = y + "px";
}
if (ns4)
    JSFX.Layer.prototype.moveTo = function (x, y) { this.elem.moveTo(x, y); }
/**********************************************************************************/
/*** show()/hide() Visibility ***/
JSFX.Layer.prototype.show = function () { this.style.visibility = "visible"; }
JSFX.Layer.prototype.hide = function () { this.style.visibility = "hidden"; }
JSFX.Layer.prototype.isVisible = function () { return this.style.visibility == "visible"; }
if (ns4) {
    JSFX.Layer.prototype.show = function () { this.style.visibility = "show"; }
    JSFX.Layer.prototype.hide = function () { this.style.visibility = "hide"; }
    JSFX.Layer.prototype.isVisible = function () { return this.style.visibility == "show"; }
}
/**********************************************************************************/
/*** zIndex ***/
JSFX.Layer.prototype.setzIndex = function (z) { this.style.zIndex = z; }
JSFX.Layer.prototype.getzIndex = function () { return this.style.zIndex; }
/**********************************************************************************/
/*** ForeGround (text) Color ***/
JSFX.Layer.prototype.setColor = function (c) { this.style.color = c; }
if (ns4)
    JSFX.Layer.prototype.setColor = function (c) {
        this.elem.document.write("<FONT COLOR='" + c + "'>" + this.elem.innerHTML + "</FONT>");
        this.elem.document.close();
    }
/**********************************************************************************/
/*** BackGround Color ***/
JSFX.Layer.prototype.setBgColor = function (color) { this.style.backgroundColor = color == null ? 'transparent' : color; }
if (ns4)
    JSFX.Layer.prototype.setBgColor = function (color) { this.elem.bgColor = color; }
/**********************************************************************************/
/*** BackGround Image ***/
JSFX.Layer.prototype.setBgImage = function (image) { this.style.backgroundImage = "url(" + image + ")"; }
if (ns4)
    JSFX.Layer.prototype.setBgImage = function (image) { this.style.background.src = image; }
/**********************************************************************************/
/*** set Content***/
JSFX.Layer.prototype.setContent = function (xHtml) { this.elem.innerHTML = xHtml; }
if (ns4)
    JSFX.Layer.prototype.setContent = function (xHtml) {
        this.elem.document.write(xHtml);
        this.elem.document.close();
        this.elem.innerHTML = xHtml;
    }

/**********************************************************************************/
/*** Clipping ***/
JSFX.Layer.prototype.clip = function (x1, y1, x2, y2) { this.style.clip = "rect(" + y1 + " " + x2 + " " + y2 + " " + x1 + ")"; }
if (ns4)
    JSFX.Layer.prototype.clip = function (x1, y1, x2, y2) {
        this.style.clip.top = y1;
        this.style.clip.left = x1;
        this.style.clip.bottom = y2;
        this.style.clip.right = x2;
    }
/**********************************************************************************/
/*** Resize ***/
JSFX.Layer.prototype.resizeTo = function (w, h) {
    this.style.width = w + "px";
    this.style.height = h + "px";
}
if (ns4)
    JSFX.Layer.prototype.resizeTo = function (w, h) {
        this.style.clip.width = w;
        this.style.clip.height = h;
    }
/**********************************************************************************/
/*** getX/Y ***/
JSFX.Layer.prototype.getX = function () { return parseInt(this.style.left); }
JSFX.Layer.prototype.getY = function () { return parseInt(this.style.top); }
if (ns4) {
    JSFX.Layer.prototype.getX = function () { return this.style.left; }
    JSFX.Layer.prototype.getY = function () { return this.style.top; }
}
/**********************************************************************************/
/*** getWidth/Height ***/
JSFX.Layer.prototype.getWidth = function () { return this.elem.offsetWidth; }
JSFX.Layer.prototype.getHeight = function () { return this.elem.offsetHeight; }
if (!document.getElementById)
    JSFX.Layer.prototype.getWidth = function () {
        //Extra processing here for clip
        return this.elem.scrollWidth;
    }

if (ns4) {
    JSFX.Layer.prototype.getWidth = function () { return this.style.clip.right; }
    JSFX.Layer.prototype.getHeight = function () { return this.style.clip.bottom; }
}
/**********************************************************************************/
/*** Opacity ***/
if (ns4) {
    JSFX.Layer.prototype.setOpacity = function (pc) { return 0; }
}
else if (document.all) {
    JSFX.Layer.prototype.setOpacity = function (pc) {
        if (this.style.filter == "")
            this.style.filter = "alpha(opacity=100);";
        this.elem.filters.alpha.opacity = pc;
    }
}
else {
    /*** Assume NS6 ***/
    JSFX.Layer.prototype.setOpacity = function (pc) { this.style.MozOpacity = pc + '%' }
}
/**************************************************************************/
/*** Event Handling - Start ***/
/*** NS4 ***/
if (ns4) {
    JSFX.eventmasks = {
        onabort: Event.ABORT, onblur: Event.BLUR, onchange: Event.CHANGE,
        onclick: Event.CLICK, ondblclick: Event.DBLCLICK,
        ondragdrop: Event.DRAGDROP, onerror: Event.ERROR,
        onfocus: Event.FOCUS, onkeydown: Event.KEYDOWN,
        onkeypress: Event.KEYPRESS, onkeyup: Event.KEYUP, onload: Event.LOAD,
        onmousedown: Event.MOUSEDOWN, onmousemove: Event.MOUSEMOVE,
        onmouseout: Event.MOUSEOUT, onmouseover: Event.MOUSEOVER,
        onmouseup: Event.MOUSEUP, onmove: Event.MOVE, onreset: Event.RESET,
        onresize: Event.RESIZE, onselect: Event.SELECT, onsubmit: Event.SUBMIT,
        onunload: Event.UNLOAD
    };
    JSFX.Layer.prototype.addEventHandler = function (eventname, handler) {
        this.elem.captureEvents(JSFX.eventmasks[eventname]);
        var xl = this;
        this.elem[eventname] = function (event) {
            event.clientX = event.pageX;
            event.clientY = event.pageY;
            event.button = event.which;
            event.keyCode = event.which;
            event.altKey = ((event.modifiers & Event.ALT_MASK) != 0);
            event.ctrlKey = ((event.modifiers & Event.CONTROL_MASK) != 0);
            event.shiftKey = ((event.modifiers & Event.SHIFT_MASK) != 0);
            return handler(xl, event);
        }
    }
    JSFX.Layer.prototype.removeEventHandler = function (eventName) {
        this.elem.releaseEvents(JSFX.eventmasks[eventName]);
        delete this.elem[eventName];
    }
}
    /**************************************************************************/
    /** IE 4/5+***/
else
    if (document.all) {
        JSFX.Layer.prototype.addEventHandler = function (eventName, handler) {
            var xl = this;
            this.elem[eventName] = function () {
                var e = window.event;
                e.cancelBubble = true;
                if (document.getElementById) {
                    e.layerX = e.offsetX;
                    e.layerY = e.offsetY;
                }
                else {
                    /*** Work around for IE 4 : clone window.event ***/
                    ev = new Object();
                    for (i in e)
                        ev[i] = e[i];
                    ev.layerX = e.offsetX;
                    ev.layerY = e.offsetY;
                    e = ev;
                }

                return handler(xl, e);
            }
        }
        JSFX.Layer.prototype.removeEventHandler = function (eventName) {
            this.elem[eventName] = null;
        }
    }
        /**************************************************************************/
        /*** Assume NS6 ***/
    else {
        JSFX.Layer.prototype.addEventHandler = function (eventName, handler) {
            var xl = this;
            this.elem[eventName] = function (e) {
                e.cancelBubble = true;
                return handler(xl, e);
            }
        }
        JSFX.Layer.prototype.removeEventHandler = function (eventName) {
            this.elem[eventName] = null;
        }
    }
/*** Event Handling - End ***/
/**************************************************************************/
JSFX.Layer.prototype.setTimeout = function (f, t) {
    setTimeout("window." + this.elem.id + "." + f, t);
}

/******************************************************************* 
* 
* File    : JSFX_Browser.js © JavaScript-FX.com
* 
* Created : 2000/07/15 
* 
* Author  : Roy Whittle www.Roy.Whittle.com 
* 
* Purpose : To create a cross browser "Browser" object.
*		JSFX.Browser library will allow scripts to query parameters
*		about the current browser window.
* 
* History 
* Date         Version        Description 
* 2001-03-17	2.0		Converted for javascript-fx
***********************************************************************/
if (!window.JSFX)
    JSFX = new Object();

if (!JSFX.Browser)
    JSFX.Browser = new Object();

if (navigator.appName.indexOf("Netscape") != -1) {
    JSFX.Browser.getCanvasWidth = function () { return innerWidth; }
    JSFX.Browser.getCanvasHeight = function () { return innerHeight; }
    JSFX.Browser.getWindowWidth = function () { return outerWidth; }
    JSFX.Browser.getWindowHeight = function () { return outerHeight; }
    JSFX.Browser.getScreenWidth = function () { return screen.width; }
    JSFX.Browser.getScreenHeight = function () { return screen.height; }
    JSFX.Browser.getMinX = function () { return (pageXOffset); }
    JSFX.Browser.getMinY = function () { return (pageYOffset); }
    JSFX.Browser.getMaxX = function () { return (pageXOffset + innerWidth); }
    JSFX.Browser.getMaxY = function () { return (pageYOffset + innerHeight); }

}
else if (document.all) {
    JSFX.Browser.getCanvasWidth = function () { return document.body.clientWidth; }
    JSFX.Browser.getCanvasHeight = function () { return document.body.clientHeight; }
    JSFX.Browser.getWindowWidth = function () { return document.body.clientWidth; }
    JSFX.Browser.getWindowHeight = function () { return document.body.clientHeight; }
    JSFX.Browser.getScreenWidth = function () { return screen.width; }
    JSFX.Browser.getScreenHeight = function () { return screen.height; }
    JSFX.Browser.getMinX = function () { return (document.body.scrollLeft); }
    JSFX.Browser.getMinY = function () { return (document.body.scrollTop); }
    JSFX.Browser.getMaxX = function () {
        return (document.body.scrollLeft
            + document.body.clientWidth);
    }
    JSFX.Browser.getMaxY = function () {
        return (document.body.scrollTop
            + document.body.clientHeight);
    }
}
/*** End  ***/

/*******************************************************************
*
* File    : JSFX_Fireworks3.js © JavaScript-FX.com
*
* Created : 2001/10/26
*
* Author  : Roy Whittle www.Roy.Whittle.com
*
* Purpose : To create a fire like mouse trailer. Can be customized to
*		look like a "Comet", "Rocket", "Sparkler" or "Flaming Torch"
*
* Requires	: JSFX_Layer.js - for layer creation, movement
*		: JSFX_Mouse.js - to track the mouse x,y coordinates
*
* History
* Date         Version        Description
*
* 2001-10-26	1.0		Created for javascript-fx
***********************************************************************/

var hexDigit = new Array("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F");
function dec2hex(dec) {
    return (hexDigit[dec >> 4] + hexDigit[dec & 15]);
}
function hex2dec(hex) {
    return (parseInt(hex, 16))
}

/*** Class FireworkSpark extends Layer ***/
JSFX.FireworkSpark = function (x, y) {
    //Call the superclass constructor
    this.superC = JSFX.Layer;
    this.superC("X");

    this.dx = Math.random() * 4 - 2;
    this.dy = Math.random() * 4 - 2;
    this.ay = .09;
    this.x = x;
    this.y = y;
    this.type = 0;
}
JSFX.FireworkSpark.prototype = new JSFX.Layer;
/*** END Class FireworkSpark Constructor - start methods ***/
JSFX.FireworkSpark.prototype.fire0 = function () {
    var a = Math.random() * 6.294;
    var s = Math.random() * 2;
    if (Math.random() > .6) s = 2;
    this.dx = s * Math.sin(a);
    this.dy = s * Math.cos(a) - 2;
}
JSFX.FireworkSpark.prototype.fire1 = function () {
    var a = Math.random() * 6.294;
    var s = Math.random() * 2;
    this.dx = s * Math.sin(a);
    this.dy = s * Math.cos(a) - 2;
}
JSFX.FireworkSpark.prototype.fire2 = function () {
    var a = Math.random() * 6.294;
    var s = 2;
    this.dx = s * Math.sin(a);
    this.dy = s * Math.cos(a) - 2;
}
JSFX.FireworkSpark.prototype.fire3 = function () {
    //	this.dx = Math.random()*2 - 1;
    //	this.dy = - 2 - Math.random()*4;
    var a = Math.random() * 6.294;
    var s = a - Math.random();
    this.dx = s * Math.sin(a);
    this.dy = s * Math.cos(a) - 2;
}
JSFX.FireworkSpark.prototype.fire4 = function () {
    var a = Math.random() * 6.294;
    var s = (Math.random() > 0.5) ? 2 : 1;
    if (s == 1)
        this.setFwColor = this.setFwColorR;
    else
        this.setFwColor = this.setFwColorG;
    s -= Math.random() / 4;
    this.dx = s * Math.sin(a);
    this.dy = s * Math.cos(a) - 2;
}
JSFX.FireworkSpark.prototype.fire = function (sx, sy, fw, cl) {
    if (cl == 1)
        this.setFwColor = this.setFwColorR;
    else if (cl == 2)
        this.setFwColor = this.setFwColorC;
    else if (cl == 3)
        this.setFwColor = this.setFwColorG;
    else if (cl == 4)
        this.setFwColor = this.setFwColorW;
    else
        this.setFwColor = this.setFwColorY;

    if (fw == 1)
        this.fire1();
    else if (fw == 2)
        this.fire2();
    else if (fw == 3)
        this.fire3();
    else if (fw == 4)
        this.fire4();
    else
        this.fire0();

    this.x = sx;
    this.y = sy;
    this.moveTo(sx, sy);
}
JSFX.FireworkSpark.prototype.setFwColor = function (color) {
    this.setFwColorY(color);
}
JSFX.FireworkSpark.prototype.setFwColorR = function (color) {
    var hex = dec2hex(color);
    var str = "#" + hex + "0000";
    this.setBgColor(str);
}
JSFX.FireworkSpark.prototype.setFwColorG = function (color) {
    var hex = dec2hex(color);
    var str = "#" + "00" + hex + "00";
    this.setBgColor(str);
}
JSFX.FireworkSpark.prototype.setFwColorC = function (color) {
    var hex = dec2hex(color);
    var str = "#" + "00" + hex + hex;
    this.setBgColor(str);
}
JSFX.FireworkSpark.prototype.setFwColorY = function (color) {
    var hex = dec2hex(color);
    var str = "#" + hex + hex + "00";
    this.setBgColor(str);
}
JSFX.FireworkSpark.prototype.setFwColorW = function (color) {
    var hex = dec2hex(color);
    var str = "#" + hex + hex + hex;
    this.setBgColor(str);
}
JSFX.FireworkSpark.prototype.animate = function (step) {
    var color = (step < 30) ? 255 - (step * 4) : Math.random() * (356 - (step * 4));
    this.setFwColor(color);

    this.dy += this.ay;
    this.x += this.dx;
    this.y += this.dy;
    this.moveTo(this.x, this.y);
}
/*** END Class FireworkSpark Methods***/

/*** Class FireObj extends Object ***/
JSFX.FireObj = function (numStars, x, y) {
    this.id = "JSFX_FireObj_" + JSFX.FireObj.count++;
    this.sparks = new Array();
    for (i = 0 ; i < numStars; i++) {
        this.sparks[i] = new JSFX.FireworkSpark(x, y);
        this.sparks[i].clip(0, 0, 3, 3);
        this.sparks[i].setBgColor("yellow");
        this.sparks[i].show();
    }
    this.step = 0;
    window[this.id] = this;
    this.animate();
}
JSFX.FireObj.count = 0;
JSFX.FireObj.prototype.explode = function () {
    var x = 50 + (Math.random() * (JSFX.Browser.getMaxX() - 200));
    var y = 50 + (Math.random() * (JSFX.Browser.getMaxY() - 200));
    var fw = Math.floor(Math.random() * 5);
    var cl = Math.floor(Math.random() * 5);

    for (i = 0 ; i < this.sparks.length ; i++)
        this.sparks[i].fire(x, y, fw, cl);
}
JSFX.FireObj.prototype.animate = function () {
    setTimeout("window." + this.id + ".animate()", 40);

    if (this.step > 50) this.step = 0;
    if (this.step == 0) this.explode();

    this.step++;

    for (i = 0 ; i < this.sparks.length ; i++)
        this.sparks[i].animate(this.step);

}
/*** END Class FireObj***/

/*** Create a static method for creating fire objects ***/
JSFX.Fire = function (numStars, x, y) {
    return new JSFX.FireObj(numStars, x, y);
}

/*** If no other script has added it yet, add the ns resize fix ***/
if (navigator.appName.indexOf("Netscape") != -1 && !document.getElementById) {
    if (!JSFX.ns_resize) {
        JSFX.ow = outerWidth;
        JSFX.oh = outerHeight;
        JSFX.ns_resize = function () {
            if (outerWidth != JSFX.ow || outerHeight != JSFX.oh)
                location.reload();
        }
    }
    window.onresize = JSFX.ns_resize;
}


function JSFX_StartEffects() {
    JSFX.Fire(100, 100, 100);
}
