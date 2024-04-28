
function checkPayment() {
    var orderId = document.getElementById("orderIdInput").value;

    var url = `/check-payment?order=${orderId}`;

    fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error('Ошибка проверки оплаты');
            }
            return response.json();
        })
        .then(data => {
            if (data.status == "SUCCESS")
                document.getElementById("paymentStatus").textContent = `Статус оплаты: Оплата произведена`;
            else
                document.getElementById("paymentStatus").textContent = `Статус оплаты: Оплата не произведена`;
            console.log(data);
        })
        .catch(error => {
            console.error('Ошибка:', error);
        });
}

document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("checkPaymentBtn").addEventListener("click", checkPayment);
});

function generateUUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
            v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function createQR() {
    var orderId = generateUUID();

    var requestData = {
        qrType: "QRDynamic",
        amount: 100,
        order: orderId,
        sbpMerchantId: "MA622976"
    };

    var url = "/create-qr";

    var headers = {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJNQTYyMjk3NiIsImp0aSI6ImI1OTNkODRkLTk1MWYtNGIyZi05ZGViLTcxOWExNDM4NWVmZCJ9.si-87k3Aw5GN67orgJpoyTXC0C2OpWwRCKzLogRWawU'
    };

    console.log("Request URL:", url);
    console.log("Request Headers:", headers);
    console.log("Request Data:", requestData);

    $.ajax({
        type: "POST",
        url: url,
        headers: headers,
        contentType: "application/json",
        data: JSON.stringify(requestData),
        success: function (response) {

            var qrResponseObj = JSON.parse(response.qrResponce);

            console.log("QR code URL:", qrResponseObj.qrUrl);
            console.log("QR code URL:", qrResponseObj);

            document.getElementById("qrCodeImage").src = qrResponseObj.qrUrl;
            document.getElementById("qrId").textContent = "QR ID: " + qrResponseObj.qrId;
            document.getElementById("orderUUID").textContent = "Заказ №: " + orderId;
        },
        error: function () {

            console.error("Ошибка создания QR");
        }
    });
}

document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("createQRBtn").addEventListener("click", createQR);
});

