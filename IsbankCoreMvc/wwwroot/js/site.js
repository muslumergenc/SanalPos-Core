// Kart Numarası
document.getElementById('cardNumber').addEventListener('input', function (e) {
    let input = e.target.value.replace(/\D/g, ''); // Rakam olmayanları kaldır
    let formattedInput = input.match(/.{1,4}/g); // 4 rakamda bir gruplandır
    let displayValue = formattedInput ? formattedInput.join(' ') : '#### #### #### ####';
    e.target.value = formattedInput ? formattedInput.join('-') : ''; // '-' ile form alanı formatı
    document.getElementById('displayCardNumber').textContent = displayValue; // ' ' ile görsel format

    // Kart türünü kontrol et
    let cardLogo = document.getElementById('cardLogo');

    if (/^4/.test(input)) {
        cardLogo.style.backgroundImage = "url('img/visa-logo.png')";
    } else if (/^5[1-5]/.test(input) || /^2[2-7]/.test(input)) {
        cardLogo.style.backgroundImage = "url('img/mastercard-logo.png')";
    } else if (/^9792/.test(input)) {
        cardLogo.style.backgroundImage = "url('img/troy-logo.png')";
    } else {
        cardLogo.style.backgroundImage = "none";
    }
});

// CVV Maskesi
document.getElementById('cvv').addEventListener('focus', function () {
    document.getElementById('creditCard').querySelector('.credit-card-inner').style.transform = 'rotateY(180deg)';
});
document.getElementById('cvv').addEventListener('blur', function () {
    document.getElementById('creditCard').querySelector('.credit-card-inner').style.transform = 'rotateY(0deg)';
});
// CVV alanına her input olduğunda çalışacak fonksiyon
document.getElementById('cvv').addEventListener('input', function (e) {
    let input = e.target.value.replace(/\D/g, ''); // Yalnızca rakamları al
    e.target.value = input.slice(0, 3); // En fazla 3 hane al

    // Eğer CVV 1 haneli ise, sadece ilk haneyi göster
    // Eğer 2 veya 3 haneli ise, sadece ilk haneyi açık bırak, geri kalanını maskele
    let displayCvv = input.length > 0 ? input[0] + '**' : '###'; // 1. hane görünsün, diğerleri maskelensin
    document.getElementById('cvvDisplay').textContent = displayCvv; // CVV alanında maskelenmiş gösterim
});

// Kart Sahibi
document.getElementById('cardName').addEventListener('input', function (e) {
    let input = e.target.value.trim(); // Boşlukları temizle
    document.getElementById('displayCardName').textContent = input || 'Kart Sahibi';
});

// Son Kullanma Tarihi
function updateExpiryDate() {
    let month = document.getElementById('expiryMonth').value.padStart(2, '0'); // Eksik haneleri tamamla
    let year = document.getElementById('expiryYear').value.slice(-2); // Son iki haneyi al
    let displayValue = month && year ? `${month}/${year}` : 'MM/YY';
    document.getElementById('displayExpiryDate').textContent = displayValue;
}
document.getElementById('expiryMonth').addEventListener('input', updateExpiryDate);
document.getElementById('expiryYear').addEventListener('input', updateExpiryDate);