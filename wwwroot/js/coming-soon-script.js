$(function() {
    // comming count down clock
    $('#clock').countDown({  
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
});
