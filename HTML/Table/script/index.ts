$("#rowmenu").hide();
$("#colmenu").hide();

let curCol;
let curRow;
let lastFocus;
let colresizeElement;
let rowresizeElement;
let pressResize = false;
let pressCell = false;

jQuery(function () {
  $("#rowmenu li").on("click", function (e) {
    if (curRow == undefined) {
      return;
    }
    let name = e.target.className;
    if (name == "delete") {
      table.deleteRow(curRow);
    } else if (name == "addbefore") {
      table.insertRow(curRow, "before");
    } else if (name == "addafter") {
      table.insertRow(curRow, "after");
    }
    $("#rowmenu").hide();
  });
  $("#colmenu li").on("click", function (e) {
    if (curCol == undefined) {
      return;
    }
    let name = e.target.className;
    if (name == "delete") {
      table.deleteColumn(curCol);
    } else if (name == "addbefore") {
      table.insertColumn(curCol, "before");
    } else if (name == "addafter") {
      table.insertColumn(curCol, "after");
    }
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
  pressResize = false;
  pressCell = false;
  colresizeElement = undefined;
  rowresizeElement = undefined;
});

$(document).on("mousemove", function (e) {
  if (pressResize && colresizeElement != undefined) {
    let width = e.pageX - Number($(colresizeElement).parent().offset()?.left);
    width = width < 100 ? 100 : width;
    $(colresizeElement)
      .parent()
      .width(width + "px");
  } else if (pressResize && rowresizeElement != undefined) {
    let height = e.pageY - Number($(rowresizeElement).parent().offset()?.top);
    height = height < 30 ? 30 : height;
    $(rowresizeElement)
      .parent()
      .height(height + "px");
  } else if (pressCell && lastFocus != undefined) {
    clearState();

    let cur = $(e.target);
    let lastCol = lastFocus.index();
    let lastRow = lastFocus.parent().index();
    let curCol = cur.index();
    let curRow = cur.parent().index();

    let availableRows = $("#showTable").find(".row");
    let hoverRows = availableRows.slice(
      Math.min(lastRow, curRow),
      Math.max(lastRow, curRow) + 1
    );

    hoverRows.each(function (row) {
      let availableCells = $(this).find(".cell");
      availableCells = availableCells.slice(
        Math.min(lastCol, curCol) - 1,
        Math.max(lastCol, curCol)
      );
      availableCells.each(function (index) {
        setHeadFocus(index + Math.min(lastCol, curCol));
      });
      if (row == 0) {
        availableCells.addClass("top");
      } else if (row == hoverRows.length - 1) {
        availableCells.addClass("bottom");
      }
      $(availableCells[0]).addClass("left");
      $(availableCells[availableCells.length - 1]).addClass("right");
      availableCells.addClass("selected");
      availableCells.parent().children(".id").addClass("focus");
    });
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
      pressResize = true;
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
      curCol = $(this);
      $("#rowmenu").hide();
      $("#colmenu").show(100);
      $("#colmenu").css({
        top: e.pageY - 10 + "px",
        left: e.pageX + "px",
      });
      return false;
    });
    $(col).on("mousedown", function () {
      if (pressResize) {
        return;
      }
      clearState();
      let rows = $("#showTable").children();
      rows.each(function (index, element) {
        let item = $(element).children().eq($(col).index());
        item.addClass("selected");
        item.addClass("left");
        item.addClass("right");
        if (index == 1) {
          item.addClass("top");
          clearLastFocus();
          lastFocus = item;
          setLastFocus();
        } else if (index == rows.length - 1) {
          item.addClass("bottom");
        }
      });
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
    $(cell).on("mousedown", function () {
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
      curRow = $(this);
      $("#colmenu").hide();
      $("#rowmenu").show(100);
      $("#rowmenu").css({
        top: e.pageY - 10 + "px",
        left: e.pageX + "px",
      });
      return false;
    });
    $(idCell).on("mousedown", function () {
      if (pressResize) {
        return;
      }
      clearState();
      let children = $(this).parent().children();
      $(children[1]).addClass("left");
      $(children[children.length - 1]).addClass("right");
      children.addClass("selected");
      children.addClass("top");
      children.addClass("bottom");
      clearLastFocus();
      lastFocus = $(children[1]);
      setLastFocus();
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
      pressResize = true;
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

  insertColumn(row: number, col: number) {
    let cell = new Cell("", "");
    $(cell.createCell("")).insertBefore(
      $("#showTable").children().eq(row).children().eq(col)
    );
    this.cellList.splice(col - 1, 0, cell);
  }

  deleteColumn(row: number, col: number) {
    this.cellList.splice(col - 1, 1);
    $("#showTable").children().eq(row).children().eq(col).remove();
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

  insertRow(item: any, type: string) {
    let row = this.generateRow();
    let newrow = row.createRow(Number(item.text()));
    if (type == "before") {
      $(newrow).insertBefore(item.parent());
    } else if (type == "after") {
      $(newrow).insertAfter(item.parent());
    }
    this.rows.splice(item.index() - 1, 0, row);
    this.sortRow();
  }
  deleteRow(item: any) {
    this.rows.splice(item.parent().index - 1, 1);
    item.parent().remove();
    this.sortRow();
  }
  generateRow() {
    let row = new Row(this.columns.length);
    return row;
  }
  sortRow() {
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
  }

  insertColumn(item: any, type: string) {
    let col = new Column("", "");
    let newcol = col.createColumn(item.index());
    if (type == "before") {
      $(newcol).insertBefore(item);
      for (let i = 0; i < this.rows.length; i++) {
        this.rows[i].insertColumn(i + 1, item.index() - 1);
      }
    } else if (type == "after") {
      $(newcol).insertAfter(item);
      for (let i = 0; i < this.rows.length; i++) {
        this.rows[i].insertColumn(i + 1, item.index() + 1);
      }
    }
    this.columns.splice(item.index() - 1, 0, col);
    this.sortColumn();
  }
  deleteColumn(item: any) {
    this.columns.splice(item.index(), 1);
    for (let i = 0; i < this.rows.length; i++) {
      this.rows[i].deleteColumn(i + 1, item.index());
    }
    item.remove();
    this.sortColumn();
  }
  sortColumn() {
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
            let name = "";
            while (index >= 1) {
              name =
                String.fromCharCode(64 + (index % 26 == 0 ? 26 : index % 26)) +
                name;
              index = index % 26 == 0 ? index / 26 - 1 : index / 26;
            }
            this.textContent = name;
          });
      });
  }
}

let table = new Table();

function init() {
  table.createEmptyTable(25, 25);
  table.generateDataToView();
}

function clearState() {
  let availableChildren = $("#showTable").find(".cell");
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
  removeHeadFocus(lastFocus.index());
}

function setLastFocus() {
  lastFocus.parent().children(".id").addClass("focus");
  setHeadFocus(lastFocus.index());
  lastFocus.addClass("focus");
}

function setHeadFocus(index: number) {
  $("#showTable").children(":first").children().eq(index).addClass("focus");
}

function removeHeadFocus(index: number) {
  $("#showTable").children(":first").children().eq(index).removeClass("focus");
}
