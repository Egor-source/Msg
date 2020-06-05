

function MenuButtonAction() {
    $('.hedder__content__authUser__menu').toggleClass('hedder__content__authUser__menu__active');
}

function MenuButtonAction2() {

    $('.hedder__content__menu__icon').removeClass('hedder__content__menu__icon__active');
    $('.hedder__content__menu').removeClass('hedder__content__menu__active');
    $('.hedder__content__authUser__menu').removeClass('hedder__content__authUser__menu__active');
}


function MenuClick () {

    $('.hedder__content__menu__icon').toggleClass('hedder__content__menu__icon__active');
    $('.hedder__content__menu').toggleClass('hedder__content__menu__active');
    $('.hedder__content__authUser__menu').toggleClass('hedder__content__authUser__menu__active');
}
