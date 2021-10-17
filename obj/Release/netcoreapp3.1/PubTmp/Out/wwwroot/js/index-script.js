$(window).load(function() {
    // shuffle
    var $main = $('#shuffle-wrapper');
    $main.shuffle({
        itemSelector: '.item' // the selector for the items in the grid
    });


    /* reshuffle when user clicks a filter item */
    $('#filter a').click(function (e) {
        e.preventDefault();

        // set active class
        $('#filter a').removeClass('active');
        $(this).addClass('active');

        // get group name from clicked item
        var groupName = $(this).attr('data-group');

        // reshuffle grid
        $main.shuffle('shuffle', groupName );
    });
});


$(function() {

    // window.scroll menu ite activated    
    $(window).scroll(function() {
        activeMenuItem();
    });


    // function for active menuitem
    var sections = $('section'),
        nav = $('#nav'),
        nav_height = nav.outerHeight();

    function activeMenuItem() {
        var cur_pos = $(window).scrollTop() + 2;
        sections.each(function() {
            var top = $(this).offset().top - nav_height,
                bottom = top + $(this).outerHeight();

            if (cur_pos >= top && cur_pos <= bottom) {
                nav.find('ul > li > a').parent().removeClass('active');
                nav.find('a[href="#' + $(this).attr('id') + '"]').parent().addClass('active');
            } else if (cur_pos == 2) {
                nav.find('ul > li > a').parent().removeClass('active');
            }
        });
    }
    

    // smooth-scrolling click menu item
    $(function() {
        $('#navbar > ul > li > a').click(function() {
            if (location.pathname.replace(/^\//,'') == this.pathname.replace(/^\//,'') && location.hostname == this.hostname) {
                var target = $(this.hash);
                target = target.length ? target : $('[name=' + this.hash.slice(1) +']');
                if (target.length) {
                    $('html, body').animate({
                    scrollTop: target.offset().top
                }, 500);
                    return false;
                }
            }
        });
    });


    // home-slider
    $('.slider').owlCarousel({
        singleItem:true,
        autoPlay: true,
        navigation: true,
        navigationText: ['<i class="fa fa-angle-left">', '<i class="fa fa-angle-right">'],
        mouseDrag: false
    });


    // gift-slider
    $('.gift-slider').owlCarousel({
        items: 3,
        itemsDesktop: [1199,3],  
        itemsDesktopSmall: [991, 2],
        itemsTabletSmall: [600, 1],
        mouseDrag: false
    });


    // comming count down clock
    $('#clock-1').countDown({  
        targetDate: {
            'day'   : 1,
            'month' : 6,
            'year'  : 2016,
            'hour'  : 0,
            'min'   : 0,
            'sec'   : 0
        },
        omitWeeks: true
    });


    //parallax-scrolling
    $('.parallax-window').parallax();


	// about Lightbox pluging
	$('.gallery .col a').nivoLightbox();


});