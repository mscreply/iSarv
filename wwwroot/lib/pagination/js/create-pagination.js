document.write(`
<link type="text/css" href="/lib/pagination/css/pagination.css" rel="stylesheet"/>
<script src="/lib/pagination/js/pagination.min.js"></script>
<script>
jQuery(document).ready(function ($) {
    let src = $("#pagination-src");
    $('#pagination-div').pagination({
        dataSource: function(done){
            let result = [];
            for(let i = 1; i < src.find(".pagination-item").length + 1; i++){
                result.push(i);
            }
            done(result);
        },
        pageSize: 10,
        showSizeChanger: true,
        showNavigator: true,
        formatNavigator: '<%= rangeStart %>-<%= rangeEnd %> of <%= totalNumber %> items',
        callback: function (data, pagination) {
            let html = src.clone().find(".pagination-item").slice((pagination.pageNumber - 1) * pagination.pageSize, pagination.pageNumber * pagination.pageSize);
            $("#data-container").html(html);
        }
    })
})
</script>
`)