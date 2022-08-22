$(document).ready(function () {
  $(".face").mousemove(function (e) {
    let faceWidth = $(".face").width();
    let faceHeight = $(".face").height();
    let minX = (faceWidth / 0.8 - faceWidth) / 2;
    let x = Math.max(0, e.clientX - minX);
    x = Math.min(x, faceWidth);
    let y = Math.max(0, e.clientY - 15);
    y = Math.min(y, faceHeight);
    let ratioX = x / faceWidth;
    let ratioY = y / faceHeight;
    $(".pupil").css("top", (ratioY + 0.2) * 60);
    $(".pupil").css("left", ratioX * 60);
    // console.log("test minX ", minX);
    // console.log("test client ", e.clientX, e.clientY);
    // console.log("test face ", faceWidth, faceHeight);
    // console.log("test x&y ", x, y);
    // console.log("test percentage ", ratioX, ratioY);
    // console.log("==============================================");
  });
});
