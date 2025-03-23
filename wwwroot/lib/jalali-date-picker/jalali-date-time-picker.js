// To use jalali-date-picker
// define input type date with id
// define btn with class btn-j-calendar-time and add attr calendar-for=input.date.id

document.write(`
    <link rel="stylesheet" href="/lib/jalali-date-picker/jalali-date-picker.css" />
    <script src="/lib/jalali-date-picker/moment.min.js"></script>
    <script src="/lib/jalali-date-picker/jalali-moment.browser.js"></script>
`);

$(".btn-j-calendar-time").each(function () {
    $(this).on("click", function () {
        $("#" + this.id + "-modal").toggle();
        setModalDateTime($("#" + this.id + "-modal"));
    });
    let m = $(`<div class='modal-j-calendar modal-with-time' id='${this.id}-modal'  style='top: ${this.clientHeight}px; left: 0; direction:rtl'></div>`);
    m.html(`
        <div class="d-flex">
            <div class="calendar-div">
                <div class="input-group">
                    <select class="form-select form-select-sm" id="month">
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
                    <input class="form-control form-control-sm text-center" type="number" placeholder="0000" id="year"/>
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
            </div>
            <div class="time-div input-group">
                <select class="form-select form-select-sm" id="minute" size="10"></select>
                <select class="form-select form-select-sm" id="hour" size="10"></select>
            </div>
        </div>
    `);
    $(m).insertAfter(this);
});

$(".modal-j-calendar.modal-with-time").each(function () {
    $(this).find("span").eq(0).on("click", function () { setModalTodayWithTime($(this).closest(".modal-j-calendar")) });
    $(this).find("span").eq(1).on("click", function () { retModalDateTime($(this).closest(".modal-j-calendar")) });
    $(this).find("span").eq(2).on("click", function () { $(this).closest(".modal-j-calendar").hide() });
    $(this).find("#hour").each(function () {
        for(let i = 0; i < 24; i++)
            $(this).append(`<option value=${i}>${i<10?'0'+i:i}</option>`)
    })
    $(this).find("#minute").each(function () {
        for(let i = 0; i < 60; i++)
            $(this).append(`<option value=${i}>${i<10?'0'+i:i}</option>`)
    })
});

$(".modal-j-calendar.modal-with-time select").on("change", function () {
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

$(".modal-j-calendar.modal-with-time input").on("change", function () {
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

function setModalTodayWithTime(dateModal) {
    let gDate = Date.now();
    let jDate = moment(gDate).format("jYYYY/jM/jD H:m");
    jDate = moment(jDate, "jYYYY/jM/jD H:m");
    let y = jDate.jYear();
    let m = jDate.jMonths() + 1;
    let d = jDate.jDate();
    let h = jDate.hour();
    let t = jDate.minute();
    let mSel = $(dateModal).find("#month");
    let ySel = $(dateModal).find("#year");
    let hSel = $(dateModal).find("#hour");
    let tSel = $(dateModal).find("#minute");
    ySel.val(y);
    mSel.val(m).change();
    $(dateModal).find(".calendar-table tbody td.selected").removeClass("selected");
    $(dateModal).find(".calendar-table tbody td").filter(function(){return $(this).text() === d.toString() }).addClass("selected");
    hSel.val(h);
    tSel.val(t);
}

function setModalDateTime(dateModal) {
    let gDate = $("input[type=datetime-local]#" + $(dateModal).prev(".btn-j-calendar-time").attr("calendar-for")).val();
    if (gDate === "") gDate = Date.now();
    let jDate = moment(gDate).format("jYYYY/jM/jD H:m");
    jDate = moment(jDate, "jYYYY/jM/jD H:m");
    let y = jDate.jYear();
    let m = jDate.jMonths() + 1;
    let d = jDate.jDate();
    let h = jDate.hour();
    let t = jDate.minute();
    let mSel = $(dateModal).find("#month");
    let ySel = $(dateModal).find("#year");
    let hSel = $(dateModal).find("#hour");
    let tSel = $(dateModal).find("#minute");
    ySel.val(y);
    mSel.val(m).change();
    $(dateModal).find(".calendar-table tbody td.selected").removeClass("selected");
    $(dateModal).find(".calendar-table tbody td").filter(function(){return $(this).text() === d.toString() }).addClass("selected");
    hSel.val(h);
    tSel.val(t);
}

function retModalDateTime(dateModal) {
    let d = $("#" + dateModal.attr("id") + " .calendar-table tbody td.selected").text();
    let mSel = $(dateModal).find("#month");
    let ySel = $(dateModal).find("#year");
    if (!RegExp("^1[34][0-9]{2}$").test(ySel.val())) {
        //        ySel[0].setCustomValidity("لطفا تاریخ معتبر انتخاب نمایید");
        //        ySel[0].reportValidity();
        $(ySel).addClass("year-error");
        ySel.focus();
        return false;
    }
    $(ySel).removeClass("year-error");
    let hSel = $(dateModal).find("#hour");
    let tSel = $(dateModal).find("#minute");
    let gDate = $("input[type=datetime-local]#" + $(dateModal).prev(".btn-j-calendar-time").attr("calendar-for"));
    let jDate = moment.from(`${ySel.val()}/${mSel.val()}/${d} ${hSel.val()}:${tSel.val()}`, "fa", "jYYYY/jM/jD H:m").format("YYYY-MM-DDTHH:mm");
    if (jDate.includes("-")) {
        gDate.val(jDate).change();
        $(dateModal).hide();
        return true;
    } else {
        alert("لطفا تاریخ معتبر انتخاب نمایید");
        return false;
    }
}

$("input[type=datetime-local]").on("change", function () {
    let d = new Date($(this).val());
    let el = $(this).closest(".form-group").find("label").eq(0);
    let txt = d.toLocaleDateString('fa-IR', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: 'numeric',
        minute: 'numeric'
    });
    if (txt.includes("Invalid"))
        txt = "تاریخ مورد نظر را انتخاب کنید";
    $(el).text($(el).text().split("-")[0] + " - " + txt);
}).change();