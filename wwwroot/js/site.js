jQuery(document).ready(function ($) {

    let htmlEl = $("html");

	if($("#culture-picker-en").prop('checked')){
        htmlEl.attr("lang", "en");
		htmlEl.attr("dir", "ltr");
	}else{
		htmlEl.attr("lang", "fa");
		htmlEl.attr("dir", "rtl");
	}

    $('a').each(function () {
        if ($(this).attr("href") === undefined)
            $(this).attr("href", "#");
    });

    $(".btn-delete").on("click", function () {
        if (!confirm($("html").attr("lang") === "en" ? "Are you sure?" : "آیا مطمئن هستید؟"))
            return false;
    });

    $("div .modal-header")
        .append('<button type="button" class="btn-close m-0" data-bs-dismiss="modal"></button>')

    $("dialog .dialogTitle .close").click(function () {
        $(this).closest('dialog')[0].close();
    });

    $(".dropdown-menu").each(function () {
        if ($(this).html().trim() === "")
            $(this).prev("a").addClass("disabled");
    });

    $("input").focus(function (){
        $(this).attr("autocomplete", "on");
    });

    $(".text-truncate").each(function (){
        $(this).attr("title", $(this).text());
        $(this).attr("data-bs-toggle", "tooltip");
        $(this).attr("data-bs-placement", "bottom");
    });

    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });

    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    const popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl)
    });

    $('[name=culture]:checked').next('label').removeClass('text-secondary').addClass('text-warning');

    const arabicNumbers = ['۰', '١', '٢', '٣', '٤', '٥', '٦', '٧', '٨', '٩'];
    $(".content-num-fa").text(function(idx, v) {
        if($("#culture-picker-fa").prop('checked')) {
            const chars = v.split('');
            for (let i = 0; i < chars.length; i++) {
                if (/\d/.test(chars[i])) {
                    chars[i] = arabicNumbers[chars[i]];
                }
            }
            return chars.join('');
        }
    });

    $("#menu-btn").click(function (){
        if ($(this).next('span').hasClass('d-none')){
            $(this).closest(".bg-dark").css("width", "");
            $('span').removeClass('d-none');
        }
        else{
            $(this).closest(".bg-dark").css("width", "80px");
            $('span').addClass('d-none');
        }
    });

    // File form validation
    $(".btn-upload").click(function () {
        let form = $(this).parents("form");
        let fileElement = form.find("input:file")[0];
        let fileValidation = form.find(".file-validation");
        if (fileElement.files.length) {
            if ($("#FileTbl tr").length > 10) {
                fileValidation.html("You are not allowed to upload more than 10 files!").show('blind');
                return false;
            }
            const file = fileElement.files[0];
            if (file.size > form.find("[name=filesize]").val() * 1024) {
                fileValidation.html("Big File Size! (Max = " + form.find("[name=filesize]").val() + "KB)").show('blind');
                return false;
            }
        } else {
            fileValidation.html("Please select file!").show('blind');
            return false;
        }

        fileValidation.html("");
    });
    
    $(".hidden-file-btn-upload").click(hiddenFileBtnUploadClick);

    $(".hidden-file-input-upload").change(hiddenFileInputUploadChange);

    let toast = $(".toast");
    toast.each(function (){
        new bootstrap.Toast(this).show();
    });

    $(".main-category-select option").each(function() {
        $(this).siblings("[value='"+ this.value +"']").remove();
    });
    $('.main-category-select').on('change', function(e) {
        let selector = $(this).val();
        $(this).next(".sub-category-select").find("option").hide();
        $(this).next(".sub-category-select").find("option").each(function() {
            if($(this).attr('tag') === selector)
                $(this).show();
        });
        $(this).next(".sub-category-select").find("option").each(function (){
            if($(this).css("display") !== "none"){
                $(this).prop("selected", true);
                return false;
            }
        });
    });
    $(".main-category-select").change();
    
    if ($(".no-scrolling-iframe")[0]) {
        iframeResize();
        setInterval(iframeResize, 2000);
    };

});

function goSearch(searchElement) {
    $("#" + $(searchElement).attr("searchTable")).find("tr").not(":first").each(function () {
        $(this).hide();
        $(this).find("td").slice(parseInt($(searchElement).attr("colStart")), parseInt($(searchElement).attr("colEnd"))).each(function () {
            $(this).find(".highlight").each(function (){
                $(this).replaceWith($(this).text());
            });
            let i = $(this).text().toLowerCase().indexOf($(searchElement).val().toLowerCase());
            if (i >= 0) {
                $(this).closest("tr").show();
                if ($(searchElement).attr("highlight") === "on") {
                    let foundText = $(this).text().substr(i, $(searchElement).val().length);
                    $(this).html($(this).html().replace(foundText, "<span class='highlight'>" + foundText + "</span>"));
                }
            }
        });
    });
}

function hiddenFileBtnUploadClick(){
    let target = this;
    if(target.toString().indexOf("Window") > -1)
        target = event.target;
    $(target).next("input:file").click();
    return false;
}

function hiddenFileInputUploadChange() {
    let target = this;
    if(target.toString().indexOf("Window") > -1)
        target = event.target;
    let form = $(target).parents("form");
    let fileValidation = form.find($(".file-validation"));
    if (target.files.length) {
        const file = target.files[0];
        if (file.size > form.find("[name=filesize]").val() * 1024) {
            fileValidation.html("Big File Size! (Max = " + form.find("[name=filesize]").val() + "KB)").show('blind');
            return false;
        }
    } else {
        fileValidation.html("Please select file!").show('blind');
        return false;
    }

    fileValidation.html("");
    form.submit();
}

function iframeResize() {
    $(".no-scrolling-iframe").each(function () {
        $(this).attr("scrolling", "no").css("height", this.contentWindow.document.body.scrollHeight + 'px');
    });
}

function divToPDF(){
    let file = prompt("لطفا نام فایل را وارد کنید");
    let opt = {
        margin: 0,
        filename: 'myfile.pdf',
        image: {type: 'jpeg', quality: 0.98},
        html2canvas: {scale: 2},
        jsPDF: {unit: 'in', format: 'A4', orientation: 'portrait'}
    };

// New Promise-based usage:
    html2pdf().set(opt).from(printDiv.innerHTML).save(file);
}

function frameToPDF(){
    let file = prompt("لطفا نام فایل را وارد کنید");
    let opt = {
        margin: 0,
        filename: 'myfile.pdf',
        image: {type: 'jpeg', quality: 0.98},
        html2canvas: {scale: 2},
        jsPDF: {unit: 'in', format: 'A4', orientation: 'portrait'}
    };

// New Promise-based usage:
    html2pdf().set(opt).from(document.querySelector("iframe").contentDocument.querySelector("#printDiv").innerHTML).save(file);
}

function divToPrintWin(printArea){
    let w = window.open("/print", "_blank");
    w.document.write(`<html lang="fa">
        <head>
            <meta charset="utf-8"/>
            <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
            <title>Print - mptravel</title>
            <link rel="stylesheet" href="/lib/bootstrap/dist/css/bootstrap.min.css"/>
            <link rel="stylesheet" href="/lib/fontawesome/css/all.min.css"/>
            <link rel="stylesheet" href="/lib/font/fonts.css"/>
            <link rel="stylesheet" href="/css/site.css"/>
            <script src="/lib/jquery/dist/jquery.min.js"></script>
            <script src="/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
            <script src="/js/site.js"></script>
        </head>
        <body>`);
    if(document.getElementById("mt") !== null)
        document.write(`
            <style>
                * {
                    -webkit-print-color-adjust: exact !important;   /* Chrome, Safari 6 – 15.3, Edge */
                    color-adjust: exact !important;                 /* Firefox 48 – 96 */
                    print-color-adjust: exact !important;           /* Firefox 97+, Safari 15.4+ */
                }
                .letter {
                    padding: ` + mt.value + "cm " + mr.value + "cm " + mb.value + "cm " + ml.value + "cm;" + `
                    font-size: ` + fs.value + "pt;" + `
                    font-family: ` + f.value + ";" + `
                }
                .letter table {
                    font-size: ` + fs.value + "pt!important;" + `
                    font-family: ` + f.value + ";" + `
                }
            </style>
        `);
    w.document.write(printArea.innerHTML);
    w.document.write("<script>setTimeout('print()', 1000)</script>");
    w.document.write("</body></html>");
    w.document.close();
}

function renderMultiPages(){
    let pageSize = Math.floor(29.5 / 0.026458);
    pageSize -= Math.ceil($(".letter").css("padding-top").replace("px",""));
    pageSize -= Math.ceil($(".letter").css("padding-bottom").replace("px",""));
    let titleDiv = document.querySelector(".letter div");
    let head = document.querySelector(".letter table thead");
    pageSize -= titleDiv.clientHeight;
    pageSize -= head.clientHeight + 50;
    let tr = document.querySelector(".letter table tbody tr");
    while(tr != null){
        let letter = document.createElement("div");
        letter.classList.add("letter");
        letter.appendChild(titleDiv.cloneNode(true));
        let tbl = document.createElement("table");
        tbl.classList.add(document.querySelector(".letter table").className);
        tbl.appendChild(head.cloneNode(true));
        let tbody = tbl.appendChild(document.createElement("tbody"));
        let size = 0;
        while(tr != null && size < pageSize - tr.cells[0].clientHeight){
            let r = tr.cells[0].rowSpan;
            for (var i = 0; tr != null && i < r; i++){
                size += tr.clientHeight;
                tbody.appendChild(tr);
                tr = document.querySelector(".letter:first-child table tbody tr");
            }
        }
        letter.appendChild(tbl);
        printDiv.appendChild(letter);
    }
    document.querySelector(".letter").remove();
}

function renderRow(st, end){
    let rows = $('.letter table tr');
    rows.addClass('d-none');
    rows.eq(0).removeClass('d-none');
    rows.slice(st, end+1).removeClass('d-none');
}

function setLetterBackground(on) {
    if(on) {
        $(".letter").css("background", "");
        $("iframe").contents().find(".letter").css("background", "");
    } else {
        $(".letter").css("background", "none");
        $("iframe").contents().find(".letter").css("background", "none");
    }
}

// Localized string &...; to utf-8
function decodeHtmlEntities(str) {
    let txt = document.createElement("textarea");
    txt.innerHTML = str;
    return txt.value;
}