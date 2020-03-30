function MenuButtonAction() {
    var elem = document.querySelector('.mainMenu');

    if (elem.classList.contains('On') == true) {
        elem.classList.remove('On');
        elem.style.display = 'none';
    }
    else {
        elem.classList.add('On');
        elem.style.display = 'block';
    }
}