$(".buttons-header > div > a:has(i)").addClass("buttons-header-main");
var urlParams = new URLSearchParams(window.location.search);
if (urlParams.get('view') == 'list')
    $(".buttons-header > .col-auto > a:nth-child(1)").addClass("buttons-header-main");
else if (urlParams.get('view') == 'title')
    $(".buttons-header > .col-auto > a:nth-child(2)").addClass("buttons-header-main");
else if (urlParams.get('view') == 'table')
    $(".buttons-header > .col-auto > a:nth-child(3)").addClass("buttons-header-main");
else $(".buttons-header > .col-auto > a:nth-child(1)").addClass("buttons-header-main");