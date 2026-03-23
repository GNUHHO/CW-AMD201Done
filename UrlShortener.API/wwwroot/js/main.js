var qrModal = new bootstrap.Modal(document.getElementById('qrModal'));
var qrcodeInstance = null;

function showQR(url) {
    var box = document.getElementById("qrcodeBox");
    box.innerHTML = "";

    qrcodeInstance = new QRCode(box, {
        text: url,
        width: 200, height: 200,
        colorDark: "#000000", colorLight: "#ffffff",
        correctLevel: QRCode.CorrectLevel.H
    });

    qrModal.show();
}