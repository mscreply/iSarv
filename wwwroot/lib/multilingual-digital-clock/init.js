document.write(`
    <script src="/lib/jquery/dist/jquery.min.js"></script>

    <link rel="stylesheet" href="/lib/multilingual-digital-clock/jqClock.css" />
    <script src="/lib/multilingual-digital-clock/jqClock.js"></script>
    <div id="clk" class="jqclock ms-5"></div>
    <script>
        $("#clk").clock({
            locale: "fa",
            dateFormat: "D, d M Y",
            timeFormat: "H:i:s"
        });
    </script> 
`)