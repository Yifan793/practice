$("#rowmenu").hide();
$("#colmenu").hide();

let curCol = -1;
let curRow = -1;
let lastFocus;
let colresizeElement;
let rowresizeElement;
let press = false;
let offsetLeft = 0;
let offsetTop = 0;

jQuery(function () {
  $("#rowmenu li").on("click", function (e) {
    if (curRow == -1) {
      return;
    }
    let name = e.target.className;
    if (name == "delete") {
      table.deleteRow(curRow);
    } else if (name == "addbefore") {
      table.insertRow(curRow);
    } else if (name == "addafter") {
      table.insertRow(curRow + 1);
    }
    table.generateDataToView();
    $("#rowmenu").hide();
  });
  $("#colmenu li").on("click", function (e) {
    if (curCol == -1) {
      return;
    }
    let name = e.target.className;
    if (name == "delete") {
      table.deleteColumn(curCol);
    } else if (name == "addbefore") {
      table.insertColumn(curCol);
    } else if (name == "addafter") {
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
  console.log("press", press);
  if (
    !press ||
    (colresizeElement == undefined && rowresizeElement == undefined)
  ) {
    return;
  }
  if (colresizeElement != undefined) {
    let width = e.pageX - Number($(colresizeElement).parent().offset()?.left);
    width = width < 100 ? 100 : width;
    $(colresizeElement)
      .parent()
      .width(width + "px");
  } else if (rowresizeElement != undefined) {
    let height = e.pageY - Number($(rowresizeElement).parent().offset()?.top);
    height = height < 30 ? 30 : height;
    $(rowresizeElement)
      .parent()
      .height(height + "px");
  }
});

class Column {
  constructor(name: string, selector: string) {
    this.name = name;
    this.selector = selector;
  }
  public name: string;
  public selector: string;

  createColResize() {
    let colresize = jQuery("<div>", {
      class: "colresize",
    });
    colresize.on("mousedown", function (e) {
      press = true;
      console.log("press", press);
      offsetLeft = e.clientX;
      colresizeElement = $(this);
      $(this).css("cursor", "col-resize");
    });
    return colresize;
  }

  createColumn(i: number) {
    let col = document.createElement("div");
    col.innerHTML = this.numberToName(i + 1);
    col.className = "head";
    $(col).on("contextmenu", function (e) {
      curCol = $(this).index() - 1;
      console.log("是表格的第 " + curCol + " 列");
      $("#rowmenu").hide();
      $("#colmenu").show(100);
      $("#colmenu").css({
        top: e.pageY - 10 + "px",
        left: e.pageX + "px",
      });
      return false;
    });
    let colresize = this.createColResize();
    colresize.appendTo(col);
    return col;
  }

  numberToName(num: number) {
    let name = "";
    while (num >= 1) {
      name = String.fromCharCode(64 + (num % 26 == 0 ? 26 : num % 26)) + name;
      num = num % 26 == 0 ? num / 26 - 1 : num / 26;
    }
    return name;
  }
}

class Cell {
  constructor(name: string, value: string) {
    this.name = name;
    this.value = value;
  }
  public name: string;
  public value: string;

  createCell(value: string) {
    let cell = document.createElement("div");
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
  }
}

class Row {
  constructor(count: number) {
    let list = new Array<Cell>();
    for (var i = 0; i < count; i++) {
      let cell = new Cell("", "");
      list.push(cell);
    }
    this.cellList = list;
  }
  public cellList: Cell[];

  createIdCell(i: number) {
    let idCell = document.createElement("div");
    idCell.innerHTML = (i + 1).toString();
    idCell.className = "id";
    $(idCell).on("contextmenu", function (e) {
      curRow = Number(e.target.innerText) - 1;
      console.log("是表格的第 " + curRow + " 行");
      $("#colmenu").hide();
      $("#rowmenu").show(100);
      $("#rowmenu").css({
        top: e.pageY - 10 + "px",
        left: e.pageX + "px",
      });
      return false;
    });
    let rowresize = this.createRowResize();
    rowresize.appendTo(idCell);
    return idCell;
  }

  createRowResize() {
    let rowresize = jQuery("<div>", {
      class: "rowresize",
    });
    rowresize.on("mousedown", function (e) {
      press = true;
      offsetLeft = e.clientX;
      rowresizeElement = $(this);
      $(this).css("cursor", "row-resize");
    });
    return rowresize;
  }

  createRow(i: number) {
    let tr = document.createElement("div");
    tr.className = "row";

    let idCell = this.createIdCell(i);
    tr.append(idCell);
    for (var j = 0; j < this.cellList.length; j++) {
      let cell = this.cellList[j].createCell(this.cellList[j].value);
      tr.appendChild(cell);
    }
    return tr;
  }

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
    let head = this.generateHead();
    head.appendTo(table);

    for (let i = 0; i < this.rows.length; i++) {
      let row = this.rows[i].createRow(i);
      table.appendChild(row);
    }

    let container = document.getElementById("container");
    if (container != null) {
      container.innerHTML = "";
      container.appendChild(table);
    }
  }

  generateHead() {
    let tr = jQuery("<div>", {
      class: "row",
    });
    let idCell = document.createElement("div");
    idCell.innerHTML = "";
    idCell.className = "id";
    tr.append(idCell);

    for (var i = 0; i < this.columns.length; i++) {
      let col = this.columns[i].createColumn(i);
      tr.append(col);
    }
    return tr;
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

let table = new Table();

function init() {
  table.createEmptyTable(25, 25);
  table.generateDataToView();
}
