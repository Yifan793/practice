var isSuccess = false;
const size = 400;

const game = {
  start: function () {
    if (isSuccess) {
      document.querySelector("#success").setAttribute("hidden", true);
      document.querySelector("#oriImg").removeAttribute("hidden");
      isSuccess = false;
    }
    let img = document.querySelector("#oriImg");
    let w = document.querySelector("#splitW").value;
    let h = document.querySelector("#splitH").value;
    document.querySelector("#table").innerHTML = "";
    puzzle.initImage(img, w, h);
  },
  selectFile: function () {
    var selectFiles = document.querySelector("#select").files;
    for (var file of selectFiles) {
      if (!["image/jpeg", "image/png", "image/gif"].includes(file.type)) {
        alert("不是有效的图片文件!");
        return;
      }
      var reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onloadend = function () {
        let img = document.querySelector("#oriImg");
        img.src = this.result;
      };
      document.querySelector("#table").innerHTML = "";
      document.querySelector("#success").setAttribute("hidden", true);
      document.querySelector("#oriImg").removeAttribute("hidden");
    }
  },
  success: function () {
    return puzzle.success();
  },
};

const puzzle = {
  initImage: function (image, splitW, splitH) {
    elem.imageSrc = "url(" + image.src + ")";
    elem.ratioW = 100 / (splitW - 1);
    elem.ratioH = 100 / (splitH - 1);
    elem.scale = image.width / image.height;
    document
      .querySelector("#table")
      .setAttribute("style", "width: " + (size + Number(splitW) * 2) + "px");
    for (var i = 0; i < splitW * splitH; i++) {
      let e = elem.init(splitW, splitH, i);
      document.getElementById("table").appendChild(e);
    }
    this.shuffle("table");
  },
  shuffle: () => {
    var table = document.getElementById("table");
    for (var i = table.children.length; i >= 0; i--) {
      table.appendChild(table.children[(Math.random() * i) | 0]);
    }
  },
  success: () => {
    let current = Array.from(document.querySelector("#table").children).map(
      (x) => x.id
    );
    for (var i in current) {
      if (i !== current[i]) {
        return false;
      }
    }
    return true;
  },
};

const elem = {
  imageSrc: "",
  ratioW: 0,
  ratioH: 0,
  scale: 1,
  init: function (splitW, splitH, i) {
    let x = this.ratioW * (i % splitW) + "%";
    let y = this.ratioH * Math.floor(i / splitW) + "%";
    let e = document.createElement("div");
    e.id = i;
    e.style.backgroundImage = this.imageSrc;
    e.style.backgroundSize = splitW * 100 + "% " + splitH * 100 + "%";
    e.style.backgroundPosition = x + " " + y;
    e.style.width = size / splitW + "px";
    e.style.height = size / this.scale / splitH + "px";
    e.style.float = "left";

    e.setAttribute("draggable", "true");
    e.addEventListener("dragstart", this.dragstart, false);
    e.addEventListener("drag", this.drag, false);
    e.addEventListener("dragover", this.dragover, false);
    e.addEventListener("drop", this.drop, false);

    return e;
  },
  dragstart: function (event) {
    event.dataTransfer.setData("data", event.target.id);
  },
  dragover: function (event) {
    event.preventDefault();
  },
  drop: function (event) {
    let ori = document.getElementById(event.dataTransfer.getData("data"));
    let dest = document.getElementById(event.target.id);

    let prev1 = ori.previousSibling;
    let prev2 = dest.previousSibling;
    if (prev1 == dest) {
      ori.after(dest);
    } else if (prev2 == ori) {
      dest.after(ori);
    } else if (prev1 == null) {
      let next = ori.nextSibling;
      next.before(dest);
      prev2.after(ori);
    } else if (prev2 == null) {
      let next = dest.nextSibling;
      next.before(ori);
      prev1.after(dest);
    } else {
      prev1.after(dest);
      prev2.after(ori);
    }

    if (game.success()) {
      isSuccess = true;
      document.querySelector("#oriImg").setAttribute("hidden", true);
      document.querySelector("#success").removeAttribute("hidden");
    }
  },
};
