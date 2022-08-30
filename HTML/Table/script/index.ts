$("#rowmenu").hide();
$("#colmenu").hide();

$(".head").on("contextmenu", function (e) {
  $("#colmenu").show(100);
  $("#colmenu").css({
    top: e.pageY + "px",
    left: e.pageX + "px",
  });
  return false;
});

$(".id").on("contextmenu", function (e) {
  $("#rowmenu").show(100);
  $("#rowmenu").css({
    top: e.pageY + "px",
    left: e.pageX + "px",
  });
  return false;
});

$(document).on("click", function (e) {
  $("#rowmenu").hide();
  $("#colmenu").hide();
});

function init() {
  let table = new Table();
  // table.createTableFromJSON(testColumns, testData);
  table.createEmptyTable(25, 25);
  table.insertColumn(1);
  table.insertColumn(4);
  table.deleteColumn(2);
  table.insertRow(1);
  table.generateDataToView();
}

class Column {
  constructor(name: string, selector: string) {
    this.name = name;
    this.selector = selector;
  }
  public name: string;
  public selector: string;
}

class Cell {
  constructor(name: string, value: string) {
    this.name = name;
    this.value = value;
  }
  public name: string;
  public value: string;
}

class Row {
  constructor(count: number) {
    let list = new Array<Cell>();
    for (var j = 0; j < count; j++) {
      let cell = new Cell("", "");
      list.push(cell);
    }
    this.cellList = list;
  }
  public cellList: Cell[];
  insertColumn(index: number) {
    let cell = new Cell("", "");
    this.cellList.splice(index, 0, cell);
  }
  deleteColumn(index: number) {
    this.cellList.splice(index, 1);
  }
}

class Table {
  constructor() {}
  public columns: Array<Column> = [];
  public rows: Array<Row> = [];

  createEmptyTable(col: number, row: number) {
    this.columns = new Array<Column>();
    for (var i = 0; i < col; i++) {
      let col = new Column("", "");
      this.columns.push(col);
    }
    this.rows = new Array<Row>();
    for (var i = 0; i < row; i++) {
      let row = this.generateRow();
      this.rows.push(row);
    }
  }

  generateDataToView() {
    let table = document.createElement("div");
    table.id = "showTable";
    let tr = document.createElement("div");
    tr.className = "row";

    let idCell = document.createElement("div");
    idCell.innerHTML = "";
    idCell.className = "id";
    tr.appendChild(idCell);

    for (var i = 0; i < this.columns.length; i++) {
      let th = document.createElement("div");
      th.innerHTML = this.numberToName(i + 1);
      th.className = "head";
      tr.appendChild(th);
    }
    table.appendChild(tr);

    for (var i = 0; i < this.rows.length; i++) {
      let tr = document.createElement("div");
      tr.className = "row";

      let idCell = document.createElement("div");
      idCell.innerHTML = (i + 1).toString();
      idCell.className = "id";
      tr.append(idCell);

      for (var j = 0; j < this.columns.length; j++) {
        let cell = document.createElement("div");
        cell.innerHTML = this.rows[i].cellList[j].value;
        cell.className = "cell";
        tr.appendChild(cell);
      }
      table.appendChild(tr);
    }
    var container = document.getElementById("container");
    if (container != null) {
      container.innerHTML = "";
      container.appendChild(table);
    }
  }

  numberToName(num: number) {
    let name = "";
    while (num >= 1) {
      name = String.fromCharCode(64 + (num % 26 == 0 ? 26 : num % 26)) + name;
      console.log(num);
      num = num % 26 == 0 ? num / 26 - 1 : num / 26;
    }
    return name;
  }

  insertRow(index: number) {
    let row = this.generateRow();
    this.rows.splice(index, 0, row);
  }
  deleteRow(index: number) {
    this.rows.splice(index, 1);
  }
  generateRow() {
    let row = new Row(this.columns.length);
    return row;
  }

  insertColumn(index: number) {
    let col = new Column("", "");
    this.columns.splice(index, 0, col);
    for (let row of this.rows) {
      row.insertColumn(index);
    }
  }
  deleteColumn(index: number) {
    this.columns.splice(index, 1);
    for (let row of this.rows) {
      row.deleteColumn(index);
    }
  }
}
