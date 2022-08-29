const testColumns = [
  {
    name: " ",
    selector: "row",
  },
  {
    name: "Name",
    selector: "name",
  },
  {
    name: "Gender",
    selector: "gender",
  },
  {
    name: "Birthday",
    selector: "birthday",
  },
  {
    name: "Job",
    selector: "job",
  },
];

const testData = [
  {
    name: "Gillian",
    gender: "female",
    birthday: "1996.3",
    job: "writer",
  },
  {
    name: "Florrie",
    gender: "female",
    birthday: "1988.5",
    job: "receptionist",
  },
  {
    name: "Grace",
    gender: "male",
    birthday: "1978.2",
    job: "manager",
  },
  {
    name: "Janet",
    gender: "female",
    birthday: "1990.1",
    job: "boss",
  },
];

function init() {
  let table = new Table();
  table.createTableFromJSON(testColumns, testData);
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
  constructor(row: number, col: number, name: string, value: string) {
    this.row = row;
    this.col = col;
    this.name = name;
    this.value = value;
  }
  public row: number;
  public col: number;
  public name: string;
  public value: string;
}

class Row {
  public id: number;
  constructor(id: number) {
    this.id = id;
  }
  public cellList: Cell[];
  getValue(name: string) {
    for (var cell of this.cellList) {
      if (cell.name == name) {
        return cell.value;
      }
    }
    return "";
  }
}

class Table {
  constructor() {}
  public columns: Array<Column>;
  createTableFromJSON(columns: any[], data: any) {
    let cols = new Array<Column>();
    for (let column of columns) {
      let col = new Column(column.name, column.selector);
      cols.push(col);
    }

    let rowList = new Array<Row>();
    for (var i = 0; i < data.length; i++) {
      let row = new Row(i);
      let list = new Array<Cell>();
      for (var j = 0; j < cols.length; j++) {
        let cell = new Cell(i, j, cols[j].selector, data[i][cols[j].selector]);
        list.push(cell);
      }
      row.cellList = list;
      rowList.push(row);
    }

    let table = document.createElement("div");
    let tr = document.createElement("div");

    let idCell = document.createElement("div");
    idCell.innerHTML = "";
    idCell.className = "id";
    tr.appendChild(idCell);

    for (var i = 1; i < cols.length; i++) {
      let th = document.createElement("div");
      th.innerHTML = cols[i].name;
      th.className = "head";
      tr.appendChild(th);
    }
    table.appendChild(tr);

    for (var i = 0; i < rowList.length; i++) {
      let tr = document.createElement("div");

      let idCell = document.createElement("div");
      idCell.innerHTML = rowList[i].id.toString();
      idCell.className = "id";
      tr.append(idCell);

      for (var j = 1; j < cols.length; j++) {
        let cell = document.createElement("div");
        cell.innerHTML = rowList[i].getValue(cols[j].selector);
        cell.className = "cell";
        tr.appendChild(cell);
      }
      table.appendChild(tr);
    }
    var container = document.getElementById("showTable");
    if (container != null) {
      container.innerHTML = "";
      container.appendChild(table);
    }
  }
}
