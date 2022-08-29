var testColumns = [
    {
        name: " ",
        selector: "row"
    },
    {
        name: "Name",
        selector: "name"
    },
    {
        name: "Gender",
        selector: "gender"
    },
    {
        name: "Birthday",
        selector: "birthday"
    },
    {
        name: "Job",
        selector: "job"
    },
];
var testData = [
    {
        name: "Gillian",
        gender: "female",
        birthday: "1996.3",
        job: "writer"
    },
    {
        name: "Florrie",
        gender: "female",
        birthday: "1988.5",
        job: "receptionist"
    },
    {
        name: "Grace",
        gender: "male",
        birthday: "1978.2",
        job: "manager"
    },
    {
        name: "Janet",
        gender: "female",
        birthday: "1990.1",
        job: "boss"
    },
];
function init() {
    var table = new Table();
    table.createTableFromJSON(testColumns, testData);
}
var Column = /** @class */ (function () {
    function Column(name, selector) {
        this.name = name;
        this.selector = selector;
    }
    return Column;
}());
var Cell = /** @class */ (function () {
    function Cell(row, col, name, value) {
        this.row = row;
        this.col = col;
        this.name = name;
        this.value = value;
    }
    return Cell;
}());
var Row = /** @class */ (function () {
    function Row(id) {
        this.id = id;
    }
    Row.prototype.getValue = function (name) {
        for (var _i = 0, _a = this.cellList; _i < _a.length; _i++) {
            var cell = _a[_i];
            if (cell.name == name) {
                return cell.value;
            }
        }
        return "";
    };
    return Row;
}());
var Table = /** @class */ (function () {
    function Table() {
    }
    Table.prototype.createTableFromJSON = function (columns, data) {
        var cols = new Array();
        for (var _i = 0, columns_1 = columns; _i < columns_1.length; _i++) {
            var column = columns_1[_i];
            var col = new Column(column.name, column.selector);
            cols.push(col);
        }
        var rowList = new Array();
        for (var i = 0; i < data.length; i++) {
            var row = new Row(i);
            var list = new Array();
            for (var j = 0; j < cols.length; j++) {
                var cell = new Cell(i, j, cols[j].selector, data[i][cols[j].selector]);
                list.push(cell);
            }
            row.cellList = list;
            rowList.push(row);
        }
        var table = document.createElement("div");
        var tr = document.createElement("div");
        var idCell = document.createElement("div");
        idCell.innerHTML = "";
        idCell.className = "id";
        tr.appendChild(idCell);
        for (var i = 1; i < cols.length; i++) {
            var th = document.createElement("div");
            th.innerHTML = cols[i].name;
            th.className = "head";
            tr.appendChild(th);
        }
        table.appendChild(tr);
        for (var i = 0; i < rowList.length; i++) {
            var tr_1 = document.createElement("div");
            var idCell_1 = document.createElement("div");
            idCell_1.innerHTML = rowList[i].id.toString();
            idCell_1.className = "id";
            tr_1.append(idCell_1);
            for (var j = 1; j < cols.length; j++) {
                var cell = document.createElement("div");
                cell.innerHTML = rowList[i].getValue(cols[j].selector);
                cell.className = "cell";
                tr_1.appendChild(cell);
            }
            table.appendChild(tr_1);
        }
        var container = document.getElementById("showTable");
        if (container != null) {
            container.innerHTML = "";
            container.appendChild(table);
        }
    };
    return Table;
}());
