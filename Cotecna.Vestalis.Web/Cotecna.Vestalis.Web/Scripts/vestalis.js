var currentPopupID = "";

function showHiddenAddress(o, a, b) {
    o.innerHTML = a + '@' + b;
}

function OpenMailLink(a, b) { var url = 'm' + 'ailto' + ':' + a + '@' + b; window.location = url; }

function _log(txt) {
    if ($("#debug").length == 0) return;

    $("#debug textarea").text($("#debug textarea").text() + txt + "\r\n");
}

var cache = [];


//Created by Paul
function InitMainMenu() {
    // Arguments are image paths relative to the current page.
    $.preLoadImages = function () {
        var args_len = arguments.length;
        for (var i = args_len; i--;) {
            var cacheImage = document.createElement('img');
            cacheImage.src = arguments[i];
            cache.push(cacheImage);
        }
    }
    $.preLoadImages("../Images/ui/btn-orange-r.png", "../Images/ui/btn-orange-m.png", "../Images/ui/btn-orange-l.png");

    $.hook('show');
    $.hook('hide');

    // fix the search box in the menu
    // get the size left
    var mw = $("#HeadingMenu div.content").width() - $("#HeadingMenu div.content > ul").width();
    // remove padding
    mw -= px_pos($(".search input.text").css("padding-left"));
    mw -= px_pos($(".search input.text").css("padding-right"));
    mw -= 4; // and the border + 2 px for IE
    $(".search input.text").width(Math.min(140, Math.max(100, mw))); // cap the width to 140px;
    $("#HeadingMenu div.search-container").show();


    // main menu
    $("#HeadingMenu div.content ul li").mouseenter(function () {
        $(this).addClass("hover");
        $("div.expand-container").stop();

        if ($(this).children("div.expand-info").length == 0) return;
        if ($(this).children("div.expand-info").children().length == 0) return;

        $(this).children("div.expand-info").remove().appendTo("div.expand-container");

        $("div.expand-container div.freeblock-image-right").children().hide();

        $("div.expand-container").css("height", "auto");
        $("div.expand-container").stop().slideDown(400, function () {
            $("div.expand-container div.freeblock-image-right").children().fadeIn(400);
            invalidateIEPie();
        });

    }).mouseleave(function (e) {
        if (e.relatedTarget == $("div.expand-container").get(0)) {
            $("div.expand-container").get(0).menuowner = $(this);
            $("div.expand-container").mouseleave(function (e) {
                $("div.expand-container").get(0).menuowner.removeClass("hover");
                $("div.expand-container").children("div.expand-info").remove().appendTo($("div.expand-container").get(0).menuowner);
                $("div.expand-container").hide();
                $("div.expand-container").get(0).menuowner = null;
            });
            return;
        }

        $(this).removeClass("hover");

        if ($("div.expand-container").children("div.expand-info").length == 0) return;
        $("div.expand-container").children("div.expand-info").remove().appendTo(this);

        $("div.expand-container").hide()
    });
}

function InitTabs() {
    // tabs
    runVScrollList();

    $(".tabs > li, .tabs div.tab").mouseenter(function () {
        $(this).addClass("hover");
    }).mouseleave(function () {
        $(this).removeClass("hover");
    });

    $(".tabs > li a, .tabs div.tab a").each(function () {
        $($(this).attr("href")).hide();
    });
    $(".tabs > li:first-child a, .tabs div.tab:first-child a").each(function () {
        $(this).parent().addClass("current");
        $($(this).attr("href")).show();
    });

    $(".tabs > li, .tabs div.tab").click(function (e) {
        $($(this).siblings(".current").find("a").attr("href")).hide();

        $(this).siblings().removeClass("current");
        $(this).addClass("current");

        $($(this).children("a").attr("href")).show();
        invalidateIEPie();

        if (e.target.nodeName == "A") {
            e.preventDefault();
            return false;
        }
    });
}

function run(isPageEditor) {   

    blockFix();
    runIndexBanner();
    runIndexSectors();
    setupSelects($('select'));
    $('.filterform select').addClass("grey-border-select")

    $('input[type="radio"]').ezMark();

    $(".expand-info").each(function () {
        $(this).find("ul").filter(":last").addClass("last");
    });
    

    // Left menu
    if (isPageEditor) {
        $("#LeftMenu ul").show();
    }
    else {
        /*
  	$("#LeftMenu > ul li:not('.section')").click(function(e){
			var $ul = $(this).find("ul");

			if ( $ul.length == 0 ) return;

			if ( !$(this).hasClass("hover") ) {
				$(this).addClass("hover");
				$ul.stop(true, true).slideDown(400).parent().siblings("li:not('.section')").removeClass("hover").find('ul').slideUp(400);
			}
			else {
				$(this).removeClass("hover");
				$("#LeftMenu > ul li:not('.section')").find("ul").slideUp(400);
			}

			e.preventDefault();
			return false;
		}).siblings('.section').find('ul').show();
  	*/
        $("#LeftMenu > ul li.section, #LeftMenu > ul li.current").find("ul").show();

        $(".btn-small,.btn-large,.btn-icon").live("mouseenter", function () {
            $(this).addClass("hover");
        }).live("mouseleave", function () {
            $(this).removeClass("hover");
        });
    }

    setupButtons();

    // fix some form things
    $('div.scfForm fieldset:last').addClass("last");
      

    // file upload fix
    $(".filefield").live("change", function (e) {
        // ie adds "fakepath", remove this
        var p = $(this).val();
        if (p.lastIndexOf('\\') != -1)
            p = p.substr(p.lastIndexOf('\\') + 1);

        $(this).siblings().find('input.textfield').val(p);
    });

    $("#popupBackground").click(function () {
        closePopup();
    });

    // loginbutton
    $("li.loginButton").mouseenter(function () {
        $("#LoginPopup").show();
        $("#LoginPopup").css("left", $(this).addClass("loginButton-hover").offset().left);
        invalidateIEPie();
    }).mouseleave(function (e) {
        if (e.relatedTarget == $("#LoginPopup").get(0)) {
            $("#LoginPopup").mouseleave(function (e) {

                $("li.loginButton").removeClass("loginButton-hover");
                $("#LoginPopup").hide();
            });
            return;
        }

        $("#LoginPopup").hide();
        $(this).removeClass("loginButton-hover");
    });

    runBlockGallery();
    photoLibrary();
    runSlideshow();
    initBoard();

    $(window).resize(function () {
        invalidateIndexBanner();
    });

    if (typeof (uicornerfix_js) == 'function')
        uicornerfix_js();
}

function setupSelects($selects) {
    if (!$selects) return;

    $selects.each(function () {
        $(this).selectmenu("destroy");

        var w = $.browser.safari ? $(this).outerWidth() : $(this).width();
        var brdr = $.browser.safari || $.browser.webkit ? 2 : 4;
        var mw = w < 50 ? w + brdr : w + 2;
        var pos = {
            my: "left top",
            at: "left bottom",
            offset: $.browser.mozilla ? "1 0" : "0 0"
        };

        $(this).selectmenu({
            width: w,
            menuWidth: mw,
            maxHeight: 150,
            positionOptions: pos,
            wrapperElement: "<span />"
        });

    });

    if (typeof (uicornerfix_js) == 'function')
        uicornerfix_js();
}

function setupButtons() {
    // build and convert the scfForm buttons
    $(".scfSubmitButtonBorder").addClass("btn-small blue right")
				.prepend('<div class="left"></div>')
				.append('<div class="right"></div>')
				.children('input[type="submit"]')
				.wrap('<div class="middle"></div>');

    // convert the general buttons
    $(".btn-small,.btn-large,.btn-icon").each(function (e) {
        if ($(this).find(".overlay").length > 0) return;

        $toClone = $($(this).find(".middle").children().get(0));
        $lone = $toClone.clone().prependTo($(this)).addClass("overlay transparent0");
        $lone.val("");

        val = $(this).find(".middle").children("input").val();
        $(this).find(".middle").children("input").replaceWith('<a href="#">' + val + '</a>');
    });
}

function blockFix() {

    // fix the middle gradient stretch
    $(".block-want > .middle").each(function () {
        $(this).children('.middle-img').attr("height", $(this).height());
    });

    $(".block-tools > .top").each(function () {
        var l = $(this).children('.top-tl').width() - 1;
        $(this).css({ backgroundPosition: l + 'px 0' });
    });
}

function runSlideshow() {
    $("#slideshowContainer").each(function () {
        var _imageIndex = 0;
        var _maxImageIndex = 0;
        var _slideshow = $(this);

        _loadImageData(_slideshow.find("table td:first-child a"));

        _maxImageIndex = _slideshow.find("table td").children().length - 4;

        _slideshow.find(".thumbs a.next").click(function (e) {
            e.preventDefault();
            if ($(this).hasClass("disabled")) return;
            _showSlideshowIndex(_imageIndex + 1);
        });
        _slideshow.find(".thumbs a.prev").click(function (e) {
            e.preventDefault();
            if ($(this).hasClass("disabled")) return;
            _showSlideshowIndex(_imageIndex - 1);
        });

        _slideshow.find("table td a").click(function (e) {
            e.preventDefault();
            _loadImageData($(this));
        });

        function _showSlideshowIndex(index) {
            if (_slideshow.find("table").is(':animated')) return;

            _imageIndex = index;

            var pos = _slideshow.find("table td:eq(" + _imageIndex + ")").position();

            _slideshow.find("table").animate({
                left: -pos.left,
                duration: 600
            });

            _invalidateSlideshowButtons();
        }

        function _loadImageData($a) {
            _slideshow.find("table td a").removeClass("current");
            $a.addClass("current");

            var src = $a.find("img").attr("src")
            src = src.substr(0, src.lastIndexOf("?"));
            src += "?bc=White&amp;h=242&amp;w=371";
            _slideshow.find(".largeImage img").attr("src", src);

            $.get($a.attr("href") + "?inframe=1", function (data) {
                $('#slideshowTypography').html(data);
            });
        }

        function _invalidateSlideshowButtons() {
            _slideshow.find(".thumbs a.next").addClass("disabled");
            _slideshow.find(".thumbs a.prev").addClass("disabled");
            if (_imageIndex > 0) {
                _slideshow.find(".thumbs a.prev").removeClass("disabled");
            }
            if (_imageIndex < _maxImageIndex && _maxImageIndex > 0) {
                _slideshow.find(".thumbs a.next").removeClass("disabled");
            }
        }

        _invalidateSlideshowButtons();
    });
}

function runBlockGallery() {
    $(".block-gallery").each(function () {
        var _blockGIndex = 0;
        var _maxblockGIndex = 0;
        var _block = $(this);

        _block.find("table td:first-child a img").addClass("current");

        _maxblockGIndex = _block.find("table td").children().length - 4;

        _block.find(".thumbs a.next").click(function (e) {
            e.preventDefault();
            if ($(this).hasClass("disabled")) return;
            _showBlockGalleryIndex(_blockGIndex + 1);
        });
        _block.find(".thumbs a.prev").click(function (e) {
            e.preventDefault();
            if ($(this).hasClass("disabled")) return;
            _showBlockGalleryIndex(_blockGIndex - 1);
        });

        _block.find("table td a").click(function (e) {
            e.preventDefault();
            _block.find("table td a img").removeClass("current");
            $(this).children("img").addClass("current");
            _block.find(".large img").attr("src", $(this).attr("href"));
        });

        function _showBlockGalleryIndex(index) {
            if (_block.find("table").is(':animated')) return;

            _blockGIndex = index;

            var pos = _block.find("table td:eq(" + _blockGIndex + ")").position();

            _block.find("table").animate({
                left: -pos.left,
                duration: 600
            });

            _invalidateBlockGalleryButtons();
        }

        function _invalidateBlockGalleryButtons() {
            _block.find(".thumbs a.next").addClass("transparent50 disabled");
            _block.find(".thumbs a.prev").addClass("transparent50 disabled");
            if (_blockGIndex > 0) {
                _block.find(".thumbs a.prev").removeClass("transparent50 disabled");
            }
            if (_blockGIndex < _maxblockGIndex && _maxblockGIndex > 0) {
                _block.find(".thumbs a.next").removeClass("transparent50 disabled");
            }
        }

        _invalidateBlockGalleryButtons();

    });
}


function runVScrollList() {
    $("#tab-news,#tab-events").each(function () {
        var _currentIndex = 0;
        var _maxIndex = 0;
        var _container = $(this);
        var _intervalId = 0;

        _container.bind('onaftershow', function (e) {
            _calcMaxHeight();
            _showListIndex(0);
            _startAutoPlay();
        }).bind('onhide', function (e) {
            _stopAutoPlay();
        });

        _container.mouseenter(function () {
            _stopAutoPlay();
        }).mouseleave(function () {
            _startAutoPlay();
        });

        _maxIndex = Math.ceil(_container.find(".content ul").children().length / 2);

        _container.find(".readMore").click(function (e) {
            e.preventDefault();
            _showListIndex(_currentIndex + 1);
        });

        _container.find(".pagination ul li a").click(function (e) {
            e.preventDefault();
            _showListIndex($(this).attr("href").replace("#", "") - 1);
        });

        _showListIndex(0);

        function _calcMaxHeight() {
            // set the height of the main container to match the max pair size
            var maxHeight = 186;
            for (var i = 0; i < _maxIndex; ++i) {
                h = _container.find(".content ul li:eq(" + (i * 2) + ")").outerHeight() + _container.find(".content ul li:eq(" + ((i * 2) + 1) + ")").outerHeight() - 1;
                if (h > maxHeight)
                    maxHeight = h;
            }
            _container.find(".content").height(maxHeight);
        }

        function _startAutoPlay() {
            _stopAutoPlay();
            if (newsAutoplayDelay > 0)
                _intervalId = setInterval(function () { _autoPlayTick(); }, newsAutoplayDelay);
        }

        function _stopAutoPlay() {
            clearInterval(_intervalId);
            _intervalId = 0;
        }

        function _autoPlayTick() {
            _showListIndex(_currentIndex + 1);
        }

        function _showListIndex(index) {
            if (_container.find(".content ul").is(':animated'))
                return;

            if (index < 0) index = _maxIndex - 1;
            if (index > _maxIndex - 1) index = 0;

            var childNum = index * 2;

            // get the position of the index
            var pos = _container.find(".content ul li:eq(" + childNum + ")").position();

            // set the height of the wrapper so the news items fit exactly
            var wrapperHeight = _container.find(".content ul li:eq(" + childNum + ")").outerHeight() + _container.find(".content ul li:eq(" + (childNum + 1) + ")").outerHeight() - 1;
            _container.find(".wrapper").height(wrapperHeight);

            _container.find(".pagination ul li a").each(function () {
                var href = $(this).attr("href").replace("#", "");
                if (href == index + 1)
                    $(this).addClass("current");
                else
                    $(this).removeClass("current");
            });

            if (_currentIndex == index)
                return; // make sure height is correct, but we might not need to animate

            _currentIndex = index;

            _container.find(".content ul").animate({
                top: -pos.top,
                duration: 600
            });
        }

    });
}

var closeBoardTimeoutId = 0;
var openBoardTimeoutId = 0;
function initBoard() {
    $(".board .board-row div:not(.clear)").filter(":nth-child(3)").addClass("last");


    $(".board .board-row div:not(.clear)").mouseenter(function () {
        clearTimeout(openBoardTimeoutId);

        var $li = $(this);
        var fireOpenBoard = function () { showBoardDetailsFrame($li.find("a").attr("href"), $li); }

        openBoardTimeoutId = setTimeout(fireOpenBoard, 600);
    }).mouseleave(function (e) {
        clearTimeout(openBoardTimeoutId);
    });

    $(".board .board-row div:not(.clear) a").click(function (e) {
        e.preventDefault();
        return false;
    });

    $("#boardPopup").mouseenter(function () {
        clearTimeout(closeBoardTimeoutId);
        currentPopupID = "";

    }).mouseleave(function () {
        currentPopupID = "#boardPopup";
        closeBoardTimeoutId = setTimeout(closePopup, 200);
    });

}
function showBoardDetailsFrame(url, $li) {
    $("#boardPopup iframe").attr("src", url);

    $("#boardPopup").show();
    $("#popupBackground").show();

    var $pop = $("#boardPopup");
    var popW = $pop.width();
    var popH = $pop.height();
    var liW = $li.width();
    var liH = $li.height();

    $pop.css({
        "left": $li.position().left - popW / 2 + liW / 2,
        "top": $li.position().top - popH / 2 + liH / 2
    });

    var ydiff;
    $w = $("html, body");
    // scroll up?
    if ((ydiff = $pop.position().top - $w.scrollTop()) < 0) {
        $w.animate({ "scrollTop": $pop.position().top });
    }

    // scroll down?
    var bottom = $pop.position().top + $pop.outerHeight();
    var wbottom = $w.scrollTop() + $(window).height();
    if ((ydiff = wbottom - bottom) < 0) {
        $w.animate({ "scrollTop": $w.scrollTop() + Math.abs(ydiff) });
    }
}

var closeGalleryTimeoutId = 0;
var openGalleryTimeoutId = 0;
function photoLibrary() {
    $(".photoLibrary li:nth-child(4n)").addClass("fourth");
    $(".photoLibrary li").mouseenter(function () {

        clearTimeout(openGalleryTimeoutId); openGalleryTimeoutId = -1;

        var $li = $(this);
        var fireOpenGallery = function () { showPhotoLibraryFrame($li.find("a").attr("href"), $li); }

        openGalleryTimeoutId = setTimeout(fireOpenGallery, 600);

    }).mouseleave(function (e) {
        clearTimeout(openGalleryTimeoutId); openGalleryTimeoutId = -1;
    });

    $(".photoLibrary li a").click(function (e) {
        e.preventDefault();
        return false;
    });

    $("#photoLibraryPopup").mouseenter(function () {
        clearTimeout(closeGalleryTimeoutId); openGalleryTimeoutId = -1;
        currentPopupID = "";

    }).mouseleave(function () {
        currentPopupID = "#photoLibraryPopup";
        closeGalleryTimeoutId = setTimeout(closePopup, 200);
    });
}

function showPhotoLibraryFrame(url, $li) {
    $("#photoLibraryPopup iframe").attr("src", url);

    $("#photoLibraryPopup").show();
    $("#popupBackground").show();

    var $pop = $("#photoLibraryPopup");
    var popW = $pop.width();
    var popH = $pop.height();
    var liW = $li.width();
    var liH = $li.height();

    $pop.css({
        "left": $li.position().left - popW / 2 + liW / 2,
        "top": $li.position().top - popH / 2 + liH / 2
    });
}

/** popup utils **/
function closePopup() {
    if (currentPopupID == "")
        return;

    $(currentPopupID + " iframe").attr("src", "");
    $(currentPopupID).hide();
    $("#popupBackground").hide();

    currentPopupID = "";
}

/** index banner **/
var _groups;
var _buttons;
var _indexIntervalId;
function runIndexBanner() {
    if ($("#IndexBanner").length == 0) return;

    $("#Heading").after($(".banner-buttons").detach());

    $("#IndexBanner").show();
    $(".banner-buttons").show();

    if (bannerAutoplayDelay > 0)
        _indexIntervalId = setInterval('indexAnimateNext();', bannerAutoplayDelay);

    $(".banner-buttons").mouseenter(function () {
        clearInterval(_indexIntervalId);
        _indexIntervalId = -1;
    }).mouseleave(function () {
        if (bannerAutoplayDelay > 0)
            _indexIntervalId = setInterval('indexAnimateNext();', bannerAutoplayDelay);
    });


    $(".banner-buttons a.prev").click(function (e) {
        indexAnimatePrevious();
    });
    $(".banner-buttons a.next").click(function (e) {
        indexAnimateNext();
    });

    _groups = $("#IndexBanner .banner-group");
    _groups.find(".back, .front").css({ 'opacity': 1 });
    _groups.each(function (i, item) {
        $button = $('<a href="#">' + (i + 1) + '</a>');
        $(".banner-buttons .numbers").append($button);
        if (i == 0) $button.addClass("current");
        $button.click(function () {
            indexAnimateIndex($(this).index())
        });
    });

    invalidateIndexBanner();
    switchButtonParent($("#IndexBanner .banner-group:first-child"), false);
}
function indexAnimatePrevious() {
    if (_groups.children().is(':animated')) return false;
    var current = _groups.not(":hidden");
    var next = _groups.eq(indexWrap(current.index() - 1));

    indexHide(current);
    indexShow(next);

    $(".banner-buttons .numbers a").removeClass("current").eq(next.index()).addClass("current");
}
function indexAnimateNext() {
    if (_groups.children().is(':animated')) return false;
    var current = _groups.not(":hidden");
    var next = _groups.eq(indexWrap(current.index() + 1));

    indexHide(current);
    indexShow(next);

    $(".banner-buttons .numbers a").removeClass("current").eq(next.index()).addClass("current");
}
function indexAnimateIndex(index) {
    if (_groups.children().is(':animated')) return false;
    var current = _groups.not(":hidden");
    var next = _groups.eq(indexWrap(index));

    indexHide(current);
    indexShow(next);

    $(".banner-buttons .numbers a").removeClass("current").eq(next.index()).addClass("current");
}
function indexShow($item) {
    $item.show();
    $item.find(".back, .front").css({ 'opacity': 0 });
    var windowWidth = $(window).width();
    var bWidth = $item.find(".back").width();
    var fWidth = $item.find(".front").width();

    $item.find(".back").css('left', (windowWidth / 2 - bWidth / 2) + 220);
    $item.find(".front").css('left', (windowWidth / 2 - fWidth / 2) + 300);

    $item.find(".back").delay(400).animate({
        left: "-=220",
        opacity: 1
    }, 1000, function () {
    })
    $item.find(".front").delay(400).animate({
        left: "-=300",
        opacity: 1
    }, 1000, function () {
        switchButtonParent($(this).parent(), false);
    });
}
function indexHide($item) {

    var bPos = $item.find(".back").position();
    var fPos = $item.find(".front").position();
    switchButtonParent($item, true);

    $item.find(".back").animate({
        left: bPos.left - 220,
        opacity: 0
    }, 1000);
    $item.find(".front").animate({
        left: bPos.left - 500,
        opacity: 0
    }, 1000, function () {
        $item.hide();
    });
}
function switchButtonParent($banner, hiding) {
    var o1 = $(".banner-buttons").offset();
    var o2 = $banner.find(".front").offset();

    if (hiding) { // move back to banner image
        $banner.find(".front").append(($b = $(".banner-buttons").find(".btn-small").detach()));
        $b.offset({
            left: $b.offset().left - (o2.left - o1.left),
            top: $b.offset().top - (o2.top - o1.top)
        });
    }
    else { // move to banner buttons
        $(".banner-buttons").append(($b = $banner.find(".btn-small").detach()));
        // recalc position
        $b.offset({
            left: $b.offset().left + (o2.left - o1.left),
            top: $b.offset().top + (o2.top - o1.top)
        });
    }
}
function indexWrap(index) {
    if (index < 0) return _groups.length - 1;
    else if (index > _groups.length - 1) return 0;
    return index;
}
function invalidateIndexBanner() {
    if ($("#IndexBanner").length == 0) return;

    _groups.find(".front, .back").each(function () {
        var windowWidth = $(window).width();
        var itemWidth = $(this).width();
        //centering
        $(this).css({ "left": windowWidth / 2 - itemWidth / 2 });
    });
}

function runIndexSectors() {
    $('input[name="sectorRadioGroup"]').parents('li').mouseenter(function () {

        $(this).find('input[type=radio]').attr("checked", "checked");
        $(this).find('input[type=radio]').trigger('change');
        _selectSector($('input[name="sectorRadioGroup"]:checked').attr("id").split("_")[1]);

    }).mouseleave(function () {
        //_showDefaultSector();
    }).click(function () {
        document.location.href = $(this).find('input[type=hidden]').val();
    });

    function _selectSector(id) {
        $("#DetailSector_" + id).show().siblings(".freeblock-image-top").hide();
        invalidateIEPie();
    }

    function _showDefaultSector() {
        var $defaultChild = $('input[name="sectorRadioGroup"]:first');
        if ($defaultChild.length > 0) {
            $defaultChild.attr("checked", "checked");
            _selectSector($defaultChild.attr("id").split("_")[1]);
        }
    }

    _showDefaultSector();
}

function invalidateIEPie() {
    if (typeof (js_corner_update) == "function") js_corner_update();
}

function px_pos(p) {
    return p ? parseInt(p.replace("px", "")) : 0;
}


/*
 * Smaller jQuery scripts, this is to avoid unneccesary requests
 */
/*!
* jQuery.hook v1.0
*
* Copyright (c) 2009 Aaron Heckmann
* Dual licensed under the MIT and GPL licenses:
* http://www.opensource.org/licenses/mit-license.php
* http://www.gnu.org/licenses/gpl.html
*/
/**
* Provides the ability to hook into any jQuery.fn[method]
* with onbeforeMETHOD, onMETHOD, and onafterMETHOD.
*
* Pass in a string or array of method names you want
* to hook with onbefore, on, or onafter.
*
* Example:
* 	jQuery.hook('show');
*	jQuery(selector).bind('onbeforeshow', function (e) { alert(e.type);});
*   jQuery(selector).show() -> alerts 'onbeforeshow'
*
*   jQuery.hook(['show','hide']);
*   jQuery(selector)
*       .bind('onbeforeshow', function (e) { alert(e.type);})
*       .bind('onshow', function (e) { alert(e.type);})
*       .bind('onaftershow', function (e) { alert(e.type);})
*       .bind('onafterhide', function (e) { alert("The element is now hidden.");});
*   jQuery(selector).show().hide()
*        -> alerts 'onbeforeshow' then alerts 'onshow', then alerts 'onaftershow',
*             then after the element is hidden alerts 'The element is now hidden.'
*
*
* You can also unhook what you've hooked into by calling jQuery.unhook() passing
* in your string or array of method names to unhook.
*
*/
; (function ($) {
    $.hook = function (fns) {
        fns = typeof fns === 'string' ?
			fns.split(' ') :
			$.makeArray(fns)
		;

        jQuery.each(fns, function (i, method) {
            var old = $.fn[method];

            if (old && !old.__hookold) {

                $.fn[method] = function () {
                    this.triggerHandler('onbefore' + method);
                    this.triggerHandler('on' + method);
                    var ret = old.apply(this, arguments);
                    this.triggerHandler('onafter' + method);
                    return ret;
                };

                $.fn[method].__hookold = old;

            }
        });

    };

    $.unhook = function (fns) {
        fns = typeof fns === 'string' ?
			fns.split(' ') :
			$.makeArray(fns)
		;

        jQuery.each($.makeArray(fns), function (i, method) {
            var cur = $.fn[method];

            if (cur && cur.__hookold) {
                $.fn[method] = cur.__hookold;
            }
        });

    };
})(jQuery);
