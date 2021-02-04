function update_label(message, color) {
    document.getElementById(labelElem).innerHTML = message;
    document.getElementById(labelElem).style.color = color;
}

function validate() {
    var str = document.getElementById(passwordFieldElem).value;

    if (str.length < 8) {
        update_label("Password Length must be at Least 8 Characters", "Red");
        return ("too_short");
    }
    else if (str.search(/[A-Z]/) == -1) {
        update_label("Password must have at least 1 upper case character", "Red");
        return ("no_lowercasechar")
    }
    else if (str.search(/[a-z]/) == -1) {
        update_label("Password must have at least 1 lower case character", "Red");
        return ("no_uppercasechar")
    }
    else if (str.search(/[^A-Za-z0-9]/) == -1) {
        update_label("Password must have at least 1 special character", "Red");
        return ("no_specialchar")
    }
    update_label("Excellent", "Blue");
}