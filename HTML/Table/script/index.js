$("#rowmenu").hide();
$("#colmenu").hide();
var curCol = -1;
var curRow = -1;
var lastFocus;
var colresizeElement;
var rowresizeElement;
var press = false;
var offsetLeft = 0;
var offsetTop = 0;
jQuery(function () {
    $("#rowmenu li").on("click", function (e) {
        if (curRow == -1) {
            return;
        }
        var name = e.target.className;
        if (name == "delete") {
            table.deleteRow(curRow);
        }
        else if (name == "addbefore") {
            table.insertRow(curRow);
        }
        else if (name == "addafter") {
            table.insertRow(curRow + 1);
        }
        table.generateDataToView();
        $("#rowmenu").hide();
    });
    $("#colmenu li").on("click", function (e) {
        if (curCol == -1) {
            return;
        }
        var name = e.target.className;
        if (name == "delete") {
            table.deleteColumn(curCol);
        }
        else if (name == "addbefore") {
            table.insertColumn(curCol);
        }
        else if (name == "addafter") {
            table.insertColumn(curCol + 1);
        }
        table.generateDataToView();
        $("#colmenu").hide();
    });
});
$(document).on("contextmenu", function (event) {
    $("#rowmenu").hide();
    $("#colmenu").hide();
    event.preventDefault();
});
$(document).on("click", function (e) {
    $("#rowmenu").hide();
    $("#colmenu").hide();
});
$(document).on("mouseup", function () {
    press = false;
    colresizeElement = undefined;
    rowresizeElement = undefined;
    console.log("press", press);
});
$(document).on("mousemove", function (e) {
    var _a, _b;
    console.log("press", press);
    if (!press ||
        (colresizeElement == undefined && rowresizeElement == undefined)) {
        return;
    }
    if (colresizeElement != undefined) {
        var width = e.pageX - Number((_a = $(colresizeElement).parent().offset()) === null || _a === void 0 ? void 0 : _a.left);
        width = width < 100 ? 100 : width;
        $(colresizeElement)
            .parent()
            .width(width + "px");
    }
    else if (rowresizeElement != undefined) {
        var height = e.pageY - Number((_b = $(rowresizeElement).parent().offset()) === null || _b === void 0 ? void 0 : _b.top);
        height = height < 30 ? 30 : height;
        $(rowresizeElement)
            .parent()
            .height(height + "px");
    }
});
var Column = /** @class */ (function () {
    function Column(name, selector) {
        this.name = name;
        this.selector = selector;
    }
    Column.prototype.createColResize = function () {
        var colresize = jQuery("<div>", {
            "class": "colresize"
        });
        colresize.on("mousedown", function (e) {
            press = true;
            console.log("press", press);
            offsetLeft = e.clientX;
            colresizeElement = $(this);
            $(this).css("cursor", "col-resize");
        });
        return colresize;
    };
    Column.prototype.createColumn = function (i) {
        var col = document.createElement("div");
        col.innerHTML = this.numberToName(i + 1);
        col.className = "head";
        $(col).on("contextmenu", function (e) {
            curCol = $(this).index() - 1;
            console.log("是表格的第 " + curCol + " 列");
            $("#rowmenu").hide();
            $("#colmenu").show(100);
            $("#colmenu").css({
                top: e.pageY - 10 + "px",
                left: e.pageX + "px"
            });
            return false;
        });
        var colresize = this.createColResize();
        colresize.appendTo(col);
        return col;
    };
    Column.prototype.numberToName = function (num) {
        var name = "";
        while (num >= 1) {
            name = String.fromCharCode(64 + (num % 26 == 0 ? 26 : num % 26)) + name;
            num = num % 26 == 0 ? num / 26 - 1 : num / 26;
        }
        return name;
    };
    return Column;
}());
var Cell = /** @class */ (function () {
    function Cell(name, value) {
        this.name = name;
        this.value = value;
    }
    Cell.prototype.createCell = function (value) {
        var cell = document.createElement("div");
        $(cell).on("click", function () {
            if (lastFocus !== undefined) {
                console.log("test lastFocus ", lastFocus);
                lastFocus.removeClass("focus");
            }
            lastFocus = $(this);
            $(this).addClass("focus");
        });
        $(cell).on("dblclick", function () {
            if (lastFocus !== undefined) {
                lastFocus.attr("contentEditable", "false");
            }
            lastFocus = $(this);
            $(this).attr("contentEditable", "true");
        });
        cell.innerHTML = value;
        cell.className = "cell";
        return cell;
    };
    return Cell;
}());
var Row = /** @class */ (function () {
    function Row(count) {
        var list = new Array();
        for (var i = 0; i < count; i++) {
            var cell = new Cell("", "");
            list.push(cell);
        }
        this.cellList = list;
    }
    Row.prototype.createIdCell = function (i) {
        var idCell = document.createElement("div");
        idCell.innerHTML = (i + 1).toString();
        idCell.className = "id";
        $(idCell).on("contextmenu", function (e) {
            curRow = Number(e.target.innerText) - 1;
            console.log("是表格的第 " + curRow + " 行");
            $("#colmenu").hide();
            $("#rowmenu").show(100);
            $("#rowmenu").css({
                top: e.pageY - 10 + "px",
                left: e.pageX + "px"
            });
            return false;
        });
        var rowresize = this.createRowResize();
        rowresize.appendTo(idCell);
        return idCell;
    };
    Row.prototype.createRowResize = function () {
        var rowresize = jQuery("<div>", {
            "class": "rowresize"
        });
        rowresize.on("mousedown", function (e) {
            press = true;
            offsetLeft = e.clientX;
            rowresizeElement = $(this);
            $(this).css("cursor", "row-resize");
        });
        return rowresize;
    };
    Row.prototype.createRow = function (i) {
        var tr = document.createElement("div");
        tr.className = "row";
        var idCell = this.createIdCell(i);
        tr.append(idCell);
        for (var j = 0; j < this.cellList.length; j++) {
            var cell = this.cellList[j].createCell(this.cellList[j].value);
            tr.appendChild(cell);
        }
        return tr;
    };
    Row.prototype.insertColumn = function (index) {
        var cell = new Cell("", "");
        this.cellList.splice(index, 0, cell);
    };
    Row.prototype.deleteColumn = function (index) {
        this.cellList.splice(index, 1);
    };
    return Row;
}());
var Table = /** @class */ (function () {
    function Table() {
        this.columns = [];
        this.rows = [];
    }
    Table.prototype.createEmptyTable = function (col, row) {
        this.columns = new Array();
        for (var i = 0; i < col; i++) {
            var col_1 = new Column("", "");
            this.columns.push(col_1);
        }
        this.rows = new Array();
        for (var i = 0; i < row; i++) {
            var row_1 = this.generateRow();
            this.rows.push(row_1);
        }
    };
    Table.prototype.generateDataToView = function () {
        var table = document.createElement("div");
        table.id = "showTable";
        var head = this.generateHead();
        head.appendTo(table);
        for (var i = 0; i < this.rows.length; i++) {
            var row = this.rows[i].createRow(i);
            table.appendChild(row);
        }
        var container = document.getElementById("container");
        if (container != null) {
            container.innerHTML = "";
            container.appendChild(table);
        }
    };
    Table.prototype.generateHead = function () {
        var tr = jQuery("<div>", {
            "class": "row"
        });
        var idCell = document.createElement("div");
        idCell.innerHTML = "";
        idCell.className = "id";
        tr.append(idCell);
        for (var i = 0; i < this.columns.length; i++) {
            var col = this.columns[i].createColumn(i);
            tr.append(col);
        }
        return tr;
    };
    Table.prototype.insertRow = function (index) {
        var row = this.generateRow();
        this.rows.splice(index, 0, row);
    };
    Table.prototype.deleteRow = function (index) {
        this.rows.splice(index, 1);
    };
    Table.prototype.generateRow = function () {
        var row = new Row(this.columns.length);
        return row;
    };
    Table.prototype.insertColumn = function (index) {
        var col = new Column("", "");
        this.columns.splice(index, 0, col);
        for (var _i = 0, _a = this.rows; _i < _a.length; _i++) {
            var row = _a[_i];
            row.insertColumn(index);
        }
    };
    Table.prototype.deleteColumn = function (index) {
        this.columns.splice(index, 1);
        for (var _i = 0, _a = this.rows; _i < _a.length; _i++) {
            var row = _a[_i];
            row.deleteColumn(index);
        }
    };
    return Table;
}());
var table = new Table();
function init() {
    table.createEmptyTable(25, 25);
    table.generateDataToView();
}
