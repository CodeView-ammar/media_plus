/****  Variables Initiation  ****/
var doc = document;
var docEl = document.documentElement;
var page = $("body").data("page");


/****  Break Points Creation  ****/
if ($.fn.setBreakpoints) {
    $(window).setBreakpoints({
        distinct: true,
        breakpoints: [320, 480, 768, 1200]
    });
}
//******************************** PROGRESS BAR *******************************//

NProgress.configure({
    showSpinner: false
}).start();
setTimeout(function () {
    NProgress.done();
    $('.fade').removeClass('out');
}, 1000);


//******************************** RETINA READY *******************************//

Modernizr.addTest('retina', function () {
    return ( !! navigator.userAgent.match(/Macintosh|Mac|iPhone|iPad/i) && window.devicePixelRatio == 2);
});

Modernizr.load([{
    test: Modernizr.retina,
    yep: '~/plugins/retina.js'
}]);


//******************************** MAIN SIDEBAR ******************************//

var $html = $('html');
var $wrapper = $('#wrapper');
var $sidebar = $('#sidebar');
var $sidebar_toggle = $('.sidebar-toggle');
var $sidebar_submenu = $('.submenu');

function manageSidebar() {

    /* We change sidebar type on resize event */
    $(window).bind('enterBreakpoint1200', function () {
        $html.removeClass().addClass('sidebar-large');
        $('.sidebar-nav li.current').addClass('active');
        $('.sidebar-nav li.current .submenu').addClass('in').height('auto');
        $sidebar_toggle.attr('id', 'menu-medium');
        $sidebar.removeClass('collapse');
        sidebarHeight();
        createSideScroll();
    });

    $(window).bind('enterBreakpoint768', function () {
        $html.removeClass('sidebar-hidden').removeClass('sidebar-large').removeClass('sidebar-thin').addClass('sidebar-medium');
        $('.sidebar-nav li.current').removeClass('active');
        $('.sidebar-nav li.current .submenu').removeClass('in');
        $sidebar_toggle.attr('id', 'menu-thin');
        sidebarHeight();
        $sidebar.removeClass('collapse');
        $("#menu-right").trigger("close");
        destroySideScroll();
    });

    $(window).bind('enterBreakpoint480', function () {
        $html.removeClass('sidebar-medium').removeClass('sidebar-large').removeClass('sidebar-hidden').addClass('sidebar-thin');
        $('.sidebar-nav li.current').removeClass('active');
        $('.sidebar-nav li.current .submenu').removeClass('in');
        $sidebar.removeClass('collapse');
        sidebarHeight();
        destroySideScroll();
    });

    $(window).bind('enterBreakpoint320', function () {
        $html.removeClass('sidebar-medium').removeClass('sidebar-large').removeClass('sidebar-thin').addClass('sidebar-hidden');
        sidebarHeight();
        destroySideScroll();
    });

    /* We change sidebar type on click event */
    $(document).on("click", "#menu-large", function () {
        $html.removeClass('sidebar-medium').removeClass('sidebar-hidden').removeClass('sidebar-thin').addClass('sidebar-large');
        $sidebar_toggle.attr('id', 'menu-medium');
        $('.sidebar-nav li.current').addClass('active');
        $('.sidebar-nav li.current .submenu').addClass('in').height('auto');
        sidebarHeight();
        createSideScroll();
    });

    $(document).on("click", "#menu-medium", function () {
        $html.removeClass('sidebar-hidden').removeClass('sidebar-large').removeClass('sidebar-thin').addClass('sidebar-medium');
        $sidebar_toggle.attr('id', 'menu-thin');
        $('.sidebar-nav li.current').removeClass('active');
        $('.sidebar-nav li.current .submenu').removeClass('in');
        sidebarHeight();
        destroySideScroll();
    });

    $(document).on("click", "#menu-thin", function () {
        $html.removeClass('sidebar-medium').removeClass('sidebar-large').removeClass('sidebar-thin').addClass('sidebar-thin');
        $sidebar_toggle.attr('id', 'menu-large');
        $('.sidebar-nav li.current').removeClass('active');
        $('.sidebar-nav li.current .submenu').removeClass('in');
        sidebarHeight();
        if ($('body').hasClass('breakpoint-768')) $sidebar_toggle.attr('id', 'menu-medium');
        destroySideScroll();
    });

    function destroySideScroll() {
        $sidebar.mCustomScrollbar("destroy");
    }

    function createSideScroll() {
        destroySideScroll();
        if (!(/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))) {
            $sidebar.mCustomScrollbar({
                scrollButtons: {
                    enable: false
                },
                autoHideScrollbar: true,
                scrollInertia: 150,
                theme: "light-thin",
                advanced: {
                    updateOnContentResize: true
                }
            });
        }
    }
}

/* Toggle submenu open */
function toggleSidebarMenu() {
    var $this = $('.sidebar-nav');
    $this.find('li.active').has('ul').children('ul').addClass('collapse in');
    $this.find('li').not('.active').has('ul').children('ul').addClass('collapse');
    $this.find('li').has('ul').children('a').on('click', function (e) {
        e.preventDefault();
        $(this).parent('li').toggleClass('active').children('ul').collapse('toggle');
        $(this).parent('li').siblings().removeClass('active').children('ul.in').collapse('hide');
    });
}

/* Adjust sidebar height */
function sidebarHeight() {
    var sidebar_height;
    var mainMenuHeight = parseInt($('#main-menu').height());
    var windowHeight = parseInt($(window).height());
    var mainContentHeight = parseInt($('#main-content').height());
    if (windowHeight > mainMenuHeight && windowHeight > mainContentHeight) sidebar_height = windowHeight;
    if (mainMenuHeight > windowHeight && mainMenuHeight > mainContentHeight) sidebar_height = mainMenuHeight;
    if (mainContentHeight > mainMenuHeight && mainContentHeight > windowHeight) sidebar_height = mainContentHeight;
    if ($html.hasClass('sidebar-large') || $html.hasClass('sidebar-hidden')) {
        $sidebar.height('');
    } else {
        $sidebar.height(sidebar_height);
    }
}


/* Sidebar Statistics */
if ($.fn.sparkline) {
    /* Sparklines can also take their values from the first argument passed to the sparkline() function */
    var myvalues1 = [13, 14, 16, 15, 11, 14, 20, 14, 12, 16, 11, 17, 19, 16];
    var myvalues2 = [14, 17, 16, 12, 18, 16, 22, 15, 14, 17, 11, 18, 11, 12];
    var myvalues3 = [18, 14, 15, 14, 15, 12, 21, 16, 18, 14, 12, 15, 17, 19];
    $('.dynamicbar1').sparkline(myvalues1, {
        type: 'bar',
        barColor: '#159077',
        barWidth: 4,
        barSpacing: 1,
        height: '28px'
    });
    $('.dynamicbar2').sparkline(myvalues2, {
        type: 'bar',
        barColor: '#00699e',
        barWidth: 4,
        barSpacing: 1,
        height: '28px'
    });
    $('.dynamicbar3').sparkline(myvalues3, {
        type: 'bar',
        barColor: '#9e494e',
        barWidth: 4,
        barSpacing: 1,
        height: '28px'
    });
};


//******************************** CHAT MENU SIDEBAR ******************************//
function chatSidebar() {

    /* Manage the right sidebar */
    if ($.fn.mmenu) {
        var $menu = $('nav#menu-right');
        $menu.mmenu({
            position: 'right',
            dragOpen: true,
            counters: false,
            searchfield: {
                add: true,
                search: true,
                showLinksOnly: false
            }
        });
    }

    /* Open / Close right sidebar */
    $('#chat-toggle').on('click', function () {
        $menu.hasClass('mm-opened') ? $menu.trigger("close") : $menu.trigger("open");
        $('#chat-notification, #chat-popup').hide();
        setTimeout(function () {
            $('.mm-panel .badge-danger').each(function () {
                $(this).removeClass('hide').addClass('animated bounceIn');
            });
        }, 1000);
    });

    /* Remove current message when opening */
    $('.have-message').on('click', function () {
        $(this).removeClass('have-message');
        $(this).find('.badge-danger').fadeOut();
    });

    /* Send messages */
    $('.send-message').keypress(function (e) {
        if (e.keyCode == 13) {
            var chat_message = '<li class="img">' +
                '<span>' +
                '<div class="chat-detail chat-right">' +
                '<img src="~/img/avatars/avatar1.png" data-retina-src="~/img/avatars/avatar1_2x.png"/>' +
                '<div class="chat-detail">' +
                '<div class="chat-bubble">' +
                $(this).val() +
                '</div>' +
                '</div>' +
                '</div>' +
                '</span>' +
                '</li>';
            $(chat_message).hide().appendTo($(this).parent().parent()).fadeIn();
            $(this).val("");
        }
    });
}



//*********************************** CUSTOM FUNCTIONS *****************************//

/****  Custom Scrollbar  ****/
function customScroll() {
    $('.withScroll').each(function () {
        $(this).mCustomScrollbar("destroy");
        var scroll_height = $(this).data('height') ? $(this).data('height') : 'auto';
        var data_padding = $(this).data('padding') ? $(this).data('padding') : 0;
        if ($(this).data('height') == 'window') scroll_height = $(window).height() - data_padding;

        $(this).mCustomScrollbar({
            scrollButtons: {
                enable: false
            },
            autoHideScrollbar: true,
            scrollInertia: 150,
            theme: "dark-2",
            set_height: scroll_height,
            advanced: {
                updateOnContentResize: true
            }
        });
    });
}

/****  Animated Panels  ****/
function liveTile() {
    $('.live-tile').each(function () {
        $(this).liveTile("destroy", true); /* To get new size if resize event */
        tile_height = $(this).data("height") ? $(this).data("height") : $(this).find('.panel-body').height() + 52;
        $(this).height(tile_height);
        $(this).liveTile({
            speed: $(this).data("speed") ? $(this).data("speed") : 500, // Start after load or not
            mode: $(this).data("animation-easing") ? $(this).data("animation-easing") : 'carousel', // Animation type: carousel, slide, fade, flip, none
            playOnHover: $(this).data("play-hover") ? $(this).data("play-hover") : false, // Play live tile on hover
            repeatCount: $(this).data("repeat-count") ? $(this).data("repeat-count") : -1, // Repeat or not (-1 is infinite
            delay: $(this).data("delay") ? $(this).data("delay") : 0, // Time between two animations
            startNow: $(this).data("start-now") ? $(this).data("start-now") : true, //Start after load or not
        });
    });
}

/****  Full Screen Toggle  ****/
function toggleFullScreen() {
    if (!doc.fullscreenElement && !doc.msFullscreenElement && !doc.webkitIsFullScreen && !doc.mozFullScreenElement) {
        if (docEl.requestFullscreen) {
            docEl.requestFullscreen();
        } else if (docEl.webkitRequestFullScreen) {
            docEl.webkitRequestFullscreen();
        } else if (docEl.webkitRequestFullScreen) {
            docEl.webkitRequestFullScreen();
        } else if (docEl.msRequestFullscreen) {
            docEl.msRequestFullscreen();
        } else if (docEl.mozRequestFullScreen) {
            docEl.mozRequestFullScreen();
        }
    } else {
        if (doc.exitFullscreen) {
            doc.exitFullscreen();
        } else if (doc.webkitExitFullscreen) {
            doc.webkitExitFullscreen();
        } else if (doc.webkitCancelFullScreen) {
            doc.webkitCancelFullScreen();
        } else if (doc.msExitFullscreen) {
            doc.msExitFullscreen();
        } else if (doc.mozCancelFullScreen) {
            doc.mozCancelFullScreen();
        }
    }
}
$('.toggle_fullscreen').click(function () {
    toggleFullScreen();
});

/****  Animate numbers onload  ****/
if ($('.animate-number').length && $.fn.numerator) {
    $('.animate-number').each(function () {
        $(this).numerator({
            easing: $(this).data("animation-easing") ? $(this).data("animation-easing") : 'linear', // easing options.
            duration: $(this).data("animation-duration") ? $(this).data("animation-duration") : 700, // the length of the animation.
            toValue: $(this).data("value"), // animate to this value.
            delimiter: ','
        });
    });
}

/****  Custom Select Input  ****/
if ($('select').length && $.fn.selectpicker) {
    $('select').selectpicker();
}

/****  Show Tooltip  ****/
if ($('[rel="tooltip"]').length && $.fn.tooltip) {
    $('[rel="tooltip"]').tooltip();
}

/****  Show Popover  ****/
if ($('[rel="popover"]').length && $.fn.popover) {
    $('[rel="popover"]').popover();
    $('[rel="popover_dark"]').popover({
        template: '<div class="popover popover-dark"><div class="arrow"></div><h3 class="popover-title popover-title"></h3><div class="popover-content popover-content"></div></div>',
    });
}


/* Adjust panel height */
/*
$('[data-equal-height="true"]').each(function(){
  equalHeight($(this).children().children());
}); 

function equalHeight(group) {
   tallest = 0;
   group.each(function() {
    thisHeight = $(this).height();
    if(thisHeight > tallest) {
     tallest = thisHeight;
    }
   });
   group.height(tallest);
}
*/

/****  Improve Dropdown effect  ****/
if ($('[data-hover="dropdown"]').length && $.fn.popover) {
    $('[data-hover="dropdown"]').popover();
}

/****  Add to favorite  ****/
$('.favorite').on('click', function () {
    ($(this).hasClass('btn-default')) ? $(this).removeClass('btn-default').addClass('btn-danger') : $(this).removeClass('btn-danger').addClass('btn-default');
});

/****  Add to favorite  ****/
$('.favs').on('click', function () {
    event.preventDefault();
    ($(this).hasClass('fa-star-o')) ? $(this).removeClass('fa-star-o').addClass('fa-star c-orange') : $(this).removeClass('fa-star c-orange').addClass('fa-star-o');
});

/****  Custom Checkbox  ****/
if ($('input:checkbox').length && $.fn.iCheck) {
    $('input:checkbox').each(function () {
        // We verify if the checkbox is not a custom switch 
        if (!$(this).hasClass('switch')) {
            var icheck_style = $(this).data('style') ? $(this).data('style') : 'flat-blue';
            $(this).iCheck({
                checkboxClass: 'icheckbox_' + icheck_style
            });
        }
    });

    /* Toggle All Checkbox Function */
    $('.toggle_checkbox').on('ifClicked', function () {
        var icheck_color = $(this).parent().data('color') ? '.icheckbox_flat-' + $(this).parent().data('color') : '.icheckbox_flat-blue';
        if ($(this).parent().hasClass('checked')) {
            $(this).closest('table').find('tr').removeClass('selected');
            $(this).closest('table').find(icheck_color).removeClass('checked');
            $(this).closest('table').find(':checkbox').attr('checked', false);
        } else {
            $(this).closest('table').find('tr').addClass('selected');
            $(this).closest('table').find(icheck_color).addClass('checked');
            $(this).closest('table').find(':checkbox').attr('checked', true);
        }
    });

    /* Toggle Select Class */
    $('.icheck').on('ifClicked', function () {
        if ($(this).parent().hasClass('checked')) {
            $(this).closest('tr').removeClass('selected');
            $(this).closest('tr').find(':checkbox').attr('checked', false);
        } else {
            $(this).parent().addClass('checked');
            $(this).closest('tr').addClass('selected');
            $(this).closest('tr').find(':checkbox').attr('checked', true);
        }
    });
}

/****  Custom Radio Button  ****/
$('input:radio').each(function () {
    var icheck_style = $(this).data('style') ? $(this).data('style') : 'flat-blue';
    $(this).iCheck({
        radioClass: 'iradio_' + icheck_style,
    });
});

/****  Form Validation with Icons  ****/
if ($('.icon-validation').length && $.fn.parsley) {
    $('.icon-validation').each(function () {
        $(this).parsley({
            listeners: {
                onFieldSuccess: function (elem, constraints) {
                    elem.prev().removeClass('fa-exclamation c-red').addClass('fa-check c-green');
                },
                onFieldError: function (elem, constraints) {
                    elem.prev().removeClass('fa-check c-green').addClass('fa-exclamation c-red');
                }
            }
        });
    });
}

/****  Custom Range Slider  ****/
if ($('.range-slider').length && $.fn.rangeSlider) {
    $('.range-slider').each(function () {
        $(this).rangeSlider({
            delayOut: $(this).data('slider-delay-out') ? $(this).data('slider-delay-out') : '0',
            valueLabels: $(this).data('slider-value-label') ? $(this).data('slider-value-label') : 'show',
            step: $(this).data('slider-step') ? $(this).data('slider-step') : 1,
        });
    });
}

/****  Custom Slider  ****/
function handleSlider() {
    if ($('.slide').length && $.fn.slider) {
        $('.slide').each(function () {
            $(this).slider();
        });
    }
}

/**** Custom Switch  ****/
if ($('.switch').length && $.fn.bootstrapSwitch) {
    $('.switch').each(function () {
        var switch_size = $(this).data('size') ? $(this).data('size') : '';
        var switch_on_color = $(this).data('on-color') ? $(this).data('on-color') : 'primary';
        var switch_off_color = $(this).data('off-color') ? $(this).data('off-color') : 'primary';
        $(this).bootstrapSwitch('size', switch_size);
        $(this).bootstrapSwitch('onColor', switch_on_color);
        $(this).bootstrapSwitch('offColor', switch_off_color);
    });
}

/****  Datepicker  ****/
if ($('.datepicker').length && $.fn.datepicker) {
    $('.datepicker').each(function () {
        var datepicker_inline = $(this).data('inline') ? $(this).data('inline') : false;
        $(this).datepicker({
            inline: datepicker_inline
        });
    });
}

/****  Datetimepicker  ****/
if ($('.datetimepicker').length && $.fn.datetimepicker) {
    $('.datetimepicker').each(function () {
        var datetimepicker_inline = $(this).data('inline') ? $(this).data('inline') : false;
        $(this).datetimepicker({
            inline: datetimepicker_inline
        });
    });
}

/****  Pickadate  ****/
if ($('.pickadate').length && $.fn.pickadate) {
    $('.pickadate').each(function () {
        $(this).pickadate();
    });
}

/****  Pickatime  ****/
if ($('.pickatime').length && $.fn.pickatime) {
    $('.pickatime').each(function () {
        $(this).pickatime();
    });
}

/****  Sortable Panels  ****/
if ($('.sortable').length && $.fn.sortable) {
    $(".sortable").sortable({
        connectWith: '.sortable',
        iframeFix: false,
        items: 'div.panel',
        opacity: 0.8,
        helper: 'original',
        revert: true,
        forceHelperSize: true,
        placeholder: 'sortable-box-placeholder round-all',
        forcePlaceholderSize: true,
        tolerance: 'pointer'
    });
}

/****  Sortable Tables  ****/
if ($('.sortable_table').length && $.fn.sortable) {
    $(".sortable_table").sortable({
        itemPath: '> tbody',
        itemSelector: 'tbody tr',
        placeholder: '<tr class="placeholder"/>'
    });
}

/****  Nestable List  ****/
if ($('.nestable').length && $.fn.nestable) {
    $(".nestable").nestable();
}

/****  Sparkline Inline Charts  ****/
if ($('.sparkline').length && $.fn.sparkline) {
    $('.sparkline').each(function () {
        $(this).sparkline(
            $(this).data("sparkline-value"), {
                type: $(this).data("sparkline-type") ? $(this).data("sparkline-type") : 'bar',
                barWidth: $(this).data("sparkline-bar-width") ? $(this).data("sparkline-bar-width") : 5,
                barSpacing: $(this).data("sparkline-bar-spacing") ? $(this).data("sparkline-bar-spacing") : 2,
                height: $(this).data("sparkline-height") ? $(this).data("sparkline-height") : '20px',
                barColor: $(this).data("sparkline-color") ? $(this).data("sparkline-color") : '#7BB2B4',
                enableTagOptions: true
            });
    });
}

/****  Animation CSS3  ****/
if ($('.animated').length) {
    $('.animated').each(function () {
        delay_animation = parseInt($(this).attr("data-delay") ? $(this).attr("data-delay") : 500);
        $(this).addClass('hide').removeClass('hide', delay_animation);
    });
}

/****  Summernote Editor  ****/
if ($('.summernote').length && $.fn.summernote) {
    $('.summernote').each(function () {
        $(this).summernote({
            height: 300,
            toolbar: [
                ["style", ["style"]],
                ["style", ["bold", "italic", "underline", "clear"]],
                ["fontsize", ["fontsize"]],
                ["color", ["color"]],
                ["para", ["ul", "ol", "paragraph"]],
                ["height", ["height"]],
                ["table", ["table"]]
            ]
        });
    });
}

/****  CKE Editor  ****/
if ($('.cke-editor').length && $.fn.ckeditor) {
    $('.cke-editor').each(function () {
        $(this).ckeditor();
    });
}

/****  Tables Dynamic  ****/
if ($('.table-dynamic').length && $.fn.dataTable) {
    $('.table-dynamic').each(function () {
        var opt = {};
        // Tools: export to Excel, CSV, PDF & Print
        if ($(this).hasClass('table-tools')) {
            opt.sDom = "<'row-fluid'<'span6'T><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
            opt.oTableTools = {
                "sSwfPath": "~/plugins/datatables/swf/copy_csv_xls_pdf.swf",
                "aButtons": ["csv", "xls", "pdf", "print"]
            };
        }
        if ($(this).hasClass('no-header')) {
            opt.bFilter = false;
            opt.bLengthChange = false;
        }
        if ($(this).hasClass('no-footer')) {
            opt.bInfo = false;
            opt.bPaginate = false;
        }
        var oTable = $(this).dataTable(opt);
        oTable.fnDraw();
    });
}

/****  Gallery Images  ****/
if ($('.gallery').length && $.fn.mixItUp) {
    $('.gallery').each(function () {
        $(this).mixItUp({
            animation: {
                duration: 400,
                effects: "fade translateZ(-360px) stagger(34ms)",
                easing: "ease",
                queueLimit: 3,
                animateChangeLayout: true,
                animateResizeTargets: true
            }
        });
    });
}


/****  Initiation of Main Functions  ****/
jQuery(document).ready(function () {

    manageSidebar();
    toggleSidebarMenu();
    chatSidebar();
    customScroll();
    liveTile();
    handleSlider();

});


/****  On Resize Functions  ****/
$(window).bind('resize', function (e) {
    window.resizeEvt;
    $(window).resize(function () {
        clearTimeout(window.resizeEvt);
        window.resizeEvt = setTimeout(function () {
            sidebarHeight();
            liveTile();
            customScroll();
            handleSlider();
        }, 250);
    });
});