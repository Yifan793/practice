$("#rowmenu").hide();
$("#colmenu").hide();
var curCol;
var curRow;
var lastFocus;
var colresizeElement;
var rowresizeElement;
var press = false;
var pressCell = false;
var offsetLeft = 0;
var offsetTop = 0;
jQuery(function () {
    $("#rowmenu li").on("click", function (e) {
        if (curRow == undefined) {
            return;
        }
        var name = e.target.className;
        if (name == "delete") {
            table.deleteRow(curRow);
        }
        else if (name == "addbefore") {
            table.insertRow(curRow, "before");
        }
        else if (name == "addafter") {
            table.insertRow(curRow, "after");
        }
        // table.generateDataToView();
        $("#rowmenu").hide();
    });
    $("#colmenu li").on("click", function (e) {
        if (curCol == undefined) {
            return;
        }
        var name = e.target.className;
        if (name == "delete") {
            table.deleteColumn(curCol);
        }
        else if (name == "addbefore") {
            table.insertColumn(curCol, "before");
        }
        else if (name == "addafter") {
            table.insertColumn(curCol, "after");
        }
        // table.generateDataToView();
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
    pressCell = false;
    colresizeElement = undefined;
    rowresizeElement = undefined;
});
$(document).on("mousemove", function (e) {
    var _a, _b;
    if (press && colresizeElement != undefined) {
        var width = e.pageX - Number((_a = $(colresizeElement).parent().offset()) === null || _a === void 0 ? void 0 : _a.left);
        width = width < 100 ? 100 : width;
        $(colresizeElement)
            .parent()
            .width(width + "px");
    }
    else if (press && rowresizeElement != undefined) {
        var height = e.pageY - Number((_b = $(rowresizeElement).parent().offset()) === null || _b === void 0 ? void 0 : _b.top);
        height = height < 30 ? 30 : height;
        $(rowresizeElement)
            .parent()
            .height(height + "px");
    }
    else if (pressCell && lastFocus != undefined) {
        var cur = $(e.target);
        var availableRows = $("#showTable").find(".row");
        var lastCol_1 = lastFocus.index();
        var lastRow = lastFocus.parent().index();
        var curCol_1 = cur.index();
        var curRow_1 = cur.parent().index();
        var hoverRows_1 = availableRows.slice(Math.min(lastRow, curRow_1), Math.max(lastRow, curRow_1) + 1);
        clearState();
        hoverRows_1.each(function (row) {
            var availableCells = $(this).find(".cell");
            availableCells.each(function (index) {
                if (index < Math.min(lastCol_1, curCol_1)) {
                    return;
                }
                if (index > Math.max(lastCol_1, curCol_1)) {
                    return;
                }
                $("#showTable")
                    .children(":first")
                    .children()
                    .eq(index)
                    .addClass("focus");
            });
            availableCells = availableCells.slice(Math.min(lastCol_1, curCol_1) - 1, Math.max(lastCol_1, curCol_1));
            if (row == 0) {
                availableCells.addClass("top");
            }
            else if (row == hoverRows_1.length - 1) {
                availableCells.addClass("bottom");
            }
            $(availableCells[0]).addClass("left");
            $(availableCells[availableCells.length - 1]).addClass("right");
            availableCells.addClass("selected");
            availableCells.parent().children(".id").addClass("focus");
        });
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
            curCol = $(this);
            $("#rowmenu").hide();
            $("#colmenu").show(100);
            $("#colmenu").css({
                top: e.pageY - 10 + "px",
                left: e.pageX + "px"
            });
            return false;
        });
        $(col).on("click", function () {
            clearState();
            var rows = $("#showTable").children();
            console.log("test rows ", rows.length, $(this).index());
            rows.each(function (index, element) {
                var item = $(element).children().eq($(col).index());
                item.addClass("selected");
                item.addClass("left");
                item.addClass("right");
                if (index == 1) {
                    item.addClass("top");
                    clearLastFocus();
                    lastFocus = item;
                    setLastFocus();
                }
                else if (index == rows.length - 1) {
                    item.addClass("bottom");
                }
            });
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
                clearLastFocus();
            }
            clearState();
            lastFocus = $(this);
            setLastFocus();
        });
        $(cell).on("dblclick", function () {
            if (lastFocus !== undefined) {
                lastFocus.attr("contentEditable", "false");
            }
            lastFocus = $(this);
            $(this).attr("contentEditable", "true");
        });
        $(cell).on("mousedown", function () {
            pressCell = true;
        });
        $(cell).on("mouseup", function () {
            pressCell = false;
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
            curRow = $(this);
            $("#colmenu").hide();
            $("#rowmenu").show(100);
            $("#rowmenu").css({
                top: e.pageY - 10 + "px",
                left: e.pageX + "px"
            });
            return false;
        });
        $(idCell).on("click", function () {
            clearState();
            var children = $(this).parent().children();
            $(children[1]).addClass("left");
            $(children[children.length - 1]).addClass("right");
            children.addClass("selected");
            children.addClass("top");
            children.addClass("bottom");
            clearLastFocus();
            lastFocus = $(children[1]);
            setLastFocus();
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
    Row.prototype.insertColumn = function (row, col) {
        var cell = new Cell("", "");
        $(cell.createCell("")).insertBefore($("#showTable").children().eq(row).children().eq(col));
        this.cellList.splice(col - 1, 0, cell);
    };
    Row.prototype.deleteColumn = function (row, col) {
        this.cellList.splice(col - 1, 1);
        $("#showTable").children().eq(row).children().eq(col).remove();
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
    Table.prototype.insertRow = function (item, type) {
        var row = this.generateRow();
        var newrow = row.createRow(Number(item.text()));
        if (type == "before") {
            $(newrow).insertBefore(item.parent());
        }
        else if (type == "after") {
            $(newrow).insertAfter(item.parent());
        }
        this.rows.splice(item.index() - 1, 0, row);
        this.sortRow();
    };
    Table.prototype.deleteRow = function (item) {
        this.rows.splice(item.parent().index - 1, 1);
        item.parent().remove();
        this.sortRow();
    };
    Table.prototype.generateRow = function () {
        var row = new Row(this.columns.length);
        return row;
    };
    Table.prototype.sortRow = function () {
        $("#showTable")
            .children()
            .each(function (index, element) {
            if (index == 0) {
                return;
            }
            $(element)
                .children(":first")
                .contents()
                .filter(function () {
                return this.nodeType == 3;
            })
                .each(function () {
                this.textContent = index.toString();
            });
        });
    };
    Table.prototype.insertColumn = function (item, type) {
        var col = new Column("", "");
        var newcol = col.createColumn(item.index());
        if (type == "before") {
            $(newcol).insertBefore(item);
            for (var i = 0; i < this.rows.length; i++) {
                this.rows[i].insertColumn(i + 1, item.index() - 1);
            }
        }
        else if (type == "after") {
            $(newcol).insertAfter(item);
            for (var i = 0; i < this.rows.length; i++) {
                this.rows[i].insertColumn(i + 1, item.index() + 1);
            }
        }
        this.columns.splice(item.index() - 1, 0, col);
        this.sortColumn();
    };
    Table.prototype.deleteColumn = function (item) {
        this.columns.splice(item.index(), 1);
        for (var i = 0; i < this.rows.length; i++) {
            this.rows[i].deleteColumn(i + 1, item.index());
        }
        item.remove();
        this.sortColumn();
    };
    Table.prototype.sortColumn = function () {
        $("#showTable")
            .children(":first")
            .children()
            .each(function (index, element) {
            if (index == 0) {
                return;
            }
            $(element)
                .contents()
                .filter(function () {
                return this.nodeType == 3;
            })
                .each(function () {
                var name = "";
                while (index >= 1) {
                    name =
                        String.fromCharCode(64 + (index % 26 == 0 ? 26 : index % 26)) +
                            name;
                    index = index % 26 == 0 ? index / 26 - 1 : index / 26;
                }
                this.textContent = name;
            });
        });
    };
    return Table;
}());
var table = new Table();
function init() {
    table.createEmptyTable(25, 25);
    table.generateDataToView();
}
function clearState() {
    var availableChildren = $("#showTable").find(".cell");
    availableChildren.removeClass("selected");
    availableChildren.removeClass("top");
    availableChildren.removeClass("bottom");
    availableChildren.removeClass("left");
    availableChildren.removeClass("right");
    availableChildren.parent().children(".id").removeClass("focus");
    $("#showTable").find(".head").removeClass("focus");
}
function clearLastFocus() {
    if (lastFocus == undefined) {
        return;
    }
    lastFocus.removeClass("focus");
    lastFocus.parent().children(".id").removeClass("focus");
    $("#showTable")
        .children(":first")
        .children()
        .eq(lastFocus.index())
        .removeClass("focus");
}
function setLastFocus() {
    lastFocus.parent().children(".id").addClass("focus");
    $("#showTable")
        .children(":first")
        .children()
        .eq(lastFocus.index())
        .addClass("focus");
    lastFocus.addClass("focus");
}
