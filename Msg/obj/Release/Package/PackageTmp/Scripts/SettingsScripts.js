function Settings(ind) {
    var elem = document.querySelectorAll('.Button');
    var changes = document.querySelectorAll('.Changes');

    if (elem[ind].classList.contains('On') == true) {
        elem[ind].textContent = 'Изменить';
        elem[ind].classList.remove('On');
        changes[ind].style.display = 'none';
    }
    else {
        elem[ind].textContent = 'Отмена';
        elem[ind].classList.add('On');
        changes[ind].style.display = 'block';
    }
}