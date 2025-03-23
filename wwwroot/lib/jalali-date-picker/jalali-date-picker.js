// To use jalali-date-picker
// define input type date with id
// define btn with class btn-j-calendar and add attr calendar-for=input.date.id

document.write(`
    <link rel="stylesheet" href="/lib/jalali-date-picker/jalali-date-picker.css" />
    <script src="/lib/jalali-date-picker/moment.min.js"></script>
    <script src="/lib/jalali-date-picker/jalali-moment.browser.js"></script>
`);

$(".btn-j-calendar").each(function () {
    $(this).on("click", function () {
        $("#" + this.id + "-modal").toggle();
        setModalDate($("#" + this.id + "-modal"));
    });
    let m = $(`<div class='modal-j-calendar' id='${this.id}-modal'  style='top: ${this.clientHeight}px; left: 0; direction:rtl'></div>`);
    m.html(`
        <div class="input-group">
            <select class="form-select form-select-sm">
                <option value="1">فروردین</option>
                <option value="2">اردیبهشت</option>
                <option value="3">خرداد</option>
                <option value="4">تیر</option>
                <option value="5">مرداد</option>
                <option value="6">شهریور</option>
                <option value="7">مهر</option>
                <option value="8">آبان</option>
                <option value="9">آذر</option>
                <option value="10">دی</option>
                <option value="11">بهمن</option>
                <option value="12">اسفند</option>
            </select>
            <input class="form-control form-control-sm text-center" type="number" placeholder="0000"/>
        </div>
        <table class="calendar-table mt-2">
            <thead class="bg-primary text-light">
                <tr><td>ش</td><td>ی</td><td>د</td><td>س</td><td>چ</td><td>پ</td><td>ج</td></tr>
            </thead>
            <tbody>
                <tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td></td>
                <tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td></td>
                <tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td></td>
                <tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td></td>
                <tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td></td>
                <tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td></td>
            </tbody>
        </table>
        <div class="row text-center px-2">
            <span class="btn btn-sm text-success col">امروز</span>
            <span class="btn btn-sm btn-primary col">انتخاب</span>
            <span class="btn btn-sm text-secondary col">انصراف</span>
        </div>
    `);
    $(m).insertAfter(this);
});

$(".modal-j-calendar").each(function () {
    $(this).find("span").eq(0).on("click", function () { setModalToday($(this).closest(".modal-j-calendar")) });
    $(this).find("span").eq(1).on("click", function () { retModalDate($(this).closest(".modal-j-calendar")) });
    $(this).find("span").eq(2).on("click", function () { $(this).closest(".modal-j-calendar").hide() });
});

$(".modal-j-calendar select").on("change", function () {
    let ySel = $(this).next("input");
    if (!RegExp("^1[34][0-9]{2}$").test(ySel.val())) {
        //        ySel[0].setCustomValidity("لطفا تاریخ معتبر انتخاب نمایید");
        //        ySel[0].reportValidity();
        $(ySel).addClass("year-error");
        ySel.focus();
        return false;
    }
    $(ySel).removeClass("year-error");
    let d = moment(ySel.val() + "/" + $(this).val() + "/1", "jYYYY/jM/jD").jDay() - 1;
    let id = "#" + $(this).closest(".modal-j-calendar").attr("id") + " ";
    $(id + ".calendar-table tbody td").text("");
    for(let i = 1; i <= 29;i++)
        $(id + ".calendar-table tbody td").eq(i + d).text(i);
    if ($(this).val() <= 11 || moment.jIsLeapYear(ySel.val()))
        $(id + ".calendar-table tbody td").eq(30 + d).text(30);
    if ($(this).val() <= 6)
        $(id + ".calendar-table tbody td").eq(31 + d).text(31);
    if($(id + ".calendar-table tbody tr:last-child").html().includes("3"))
        $(id + ".calendar-table tbody tr:last-child").css("visibility", "visible");
    else
        $(id + ".calendar-table tbody tr:last-child").css("visibility", "hidden");
});

$(".modal-j-calendar input").on("change", function () {
    if (!RegExp("^1[34][0-9]{2}$").test($(this).val())) {
        //        ySel[0].setCustomValidity("لطفا تاریخ معتبر انتخاب نمایید");
        //        ySel[0].reportValidity();
        $(this).addClass("year-error");
        $(this).focus();
        return false;
    }
    $(this).removeClass("year-error");
    $(this).prev("select").change();
});

$(".calendar-table tbody td").on("click", function() {
    if($(this).text() === "") return;
    let id = "#" + $(this).closest(".modal-j-calendar").attr("id") + " ";
    $(id + ".calendar-table tbody td.selected").removeClass("selected");
    $(this).addClass("selected");
});

function setModalToday(dateModal) {
    let gDate = Date.now();
    let jDate = moment(gDate).format("jYYYY/jM/jD");
    let y = jDate.split("/")[0];
    let m = jDate.split("/")[1];
    let d = jDate.split("/")[2];
    let mSel = $(dateModal).find("select").eq(0);
    let ySel = $(dateModal).find("input").eq(0);
    ySel.val(y);
    mSel.val(m).change();
    $("#" + dateModal.attr("id") + " .calendar-table tbody td.selected").removeClass("selected");
    $("#" + dateModal.attr("id") + " .calendar-table tbody td").filter(function(){return $(this).text() === d }).addClass("selected");
}

function setModalDate(dateModal) {
    let gDate = $("input[type=date]#" + $(dateModal).prev(".btn-j-calendar").attr("calendar-for")).val();
    if (gDate === "") gDate = Date.now();
    let jDate = moment(gDate).format("jYYYY/jM/jD");
    let y = jDate.split("/")[0];
    let m = jDate.split("/")[1];
    let d = jDate.split("/")[2];
    let mSel = $(dateModal).find("select").eq(0);
    let ySel = $(dateModal).find("input").eq(0);
    ySel.val(y);
    mSel.val(m).change();
    $("#" + dateModal.attr("id") + " .calendar-table tbody td.selected").removeClass("selected");
    $("#" + dateModal.attr("id") + " .calendar-table tbody td").filter(function(){return $(this).text() === d }).addClass("selected");
}

function retModalDate(dateModal) {
    let d = $("#" + dateModal.attr("id") + " .calendar-table tbody td.selected").text();
    let mSel = $(dateModal).find("select").eq(0);
    let ySel = $(dateModal).find("input").eq(0);
    if (!RegExp("^1[34][0-9]{2}$").test(ySel.val())) {
        //        ySel[0].setCustomValidity("لطفا تاریخ معتبر انتخاب نمایید");
        //        ySel[0].reportValidity();
        $(ySel).addClass("year-error");
        ySel.focus();
        return false;
    }
    $(ySel).removeClass("year-error");
    let gDate = $("input[type=date]#" + $(dateModal).prev(".btn-j-calendar").attr("calendar-for"));
    let jDate = moment.from(`${ySel.val()}/${mSel.val()}/${d}`, "fa", "jYYYY/jM/jD").format("YYYY-MM-DD");
    if (jDate.includes("-")) {
        gDate.val(jDate).change();
        $(dateModal).hide();
        return true;
    } else {
        alert("لطفا تاریخ معتبر انتخاب نمایید");
        return false;
    }
}

$("input[type=date]").on("change", function () {
    let d = new Date($(this).val());
    let el = $(this).closest(".form-group").find("label").eq(0);
    let txt = d.toLocaleDateString('fa-IR', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
    });
    if (txt.includes("Invalid"))
        txt = "تاریخ مورد نظر را انتخاب کنید";
    $(el).text($(el).text().split("-")[0] + " - " + txt);
}).change();