console.log(1);
$("#rowmenu").hide();
$("#colmenu").hide();
$(".head").on("contextmenu", function (e) {
    $("#colmenu").show(100);
    $("#colmenu").css({
        top: e.pageY + "px",
        left: e.pageX + "px"
    });
    return false;
});
$(".id").on("contextmenu", function (e) {
    $("#rowmenu").show(100);
    $("#rowmenu").css({
        top: e.pageY + "px",
        left: e.pageX + "px"
    });
    return false;
});
$(document).on("click", function (e) {
    $("#rowmenu").hide();
    $("#colmenu").hide();
});
function init() {
    var table = new Table();
    // table.createTableFromJSON(testColumns, testData);
    table.createEmptyTable(25, 25);
    table.insertColumn(1);
    table.insertColumn(4);
    table.deleteColumn(2);
    table.insertRow(1);
    table.generateDataToView();
}
var Column = /** @class */ (function () {
    function Column(name, selector) {
        this.name = name;
        this.selector = selector;
    }
    return Column;
}());
var Cell = /** @class */ (function () {
    function Cell(name, value) {
        this.name = name;
        this.value = value;
    }
    return Cell;
}());
var Row = /** @class */ (function () {
    function Row(count) {
        var list = new Array();
        for (var j = 0; j < count; j++) {
            var cell = new Cell("", "");
            list.push(cell);
        }
        this.cellList = list;
    }
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
        var tr = document.createElement("div");
        tr.className = "row";
        var idCell = document.createElement("div");
        idCell.innerHTML = "";
        idCell.className = "id";
        tr.appendChild(idCell);
        for (var i = 0; i < this.columns.length; i++) {
            var th = document.createElement("div");
            th.innerHTML = this.numberToName(i + 1);
            th.className = "head";
            tr.appendChild(th);
        }
        table.appendChild(tr);
        for (var i = 0; i < this.rows.length; i++) {
            var tr_1 = document.createElement("div");
            tr_1.className = "row";
            var idCell_1 = document.createElement("div");
            idCell_1.innerHTML = (i + 1).toString();
            idCell_1.className = "id";
            tr_1.append(idCell_1);
            for (var j = 0; j < this.columns.length; j++) {
                var cell = document.createElement("div");
                cell.innerHTML = this.rows[i].cellList[j].value;
                cell.className = "cell";
                tr_1.appendChild(cell);
            }
            table.appendChild(tr_1);
        }
        var container = document.getElementById("container");
        if (container != null) {
            container.innerHTML = "";
            container.appendChild(table);
        }
    };
    Table.prototype.numberToName = function (num) {
        var name = "";
        while (num >= 1) {
            name = String.fromCharCode(64 + (num % 26 == 0 ? 26 : num % 26)) + name;
            console.log(num);
            num = num % 26 == 0 ? num / 26 - 1 : num / 26;
        }
        return name;
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
