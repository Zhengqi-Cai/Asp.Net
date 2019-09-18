function showDateAndTime() {
    var d = new Date();
    document.getElementById('dateAndTime').innerHTML = d.toLocaleString();
    var dateE = document.getElementById("date");
    if (dateE) {
        dateE.innerHTML = d.toLocaleDateString();
    }
}

function updateDateAndTime() {
    setInterval("showDateAndTime()", "1000");
    // 我们可以在使用innerHTML时先把原内容读出来，在合并进去
}

function initTemplate() {
    //updateDateAndTime();
    initTheme();
}

function initTheme() {
    var theme = null;
    var checkedTheme = window.localStorage.getItem("lab2CheckedTheme");
    if (checkedTheme == null) {

        checkedTheme = "pageTheme";
    }
    shotCheckedTheme(checkedTheme);


    theme = transToTheme(checkedTheme);
    setBodyTheme(theme);
    setLinkTheme(theme);

}

function transToTheme(radioTheme) {
    var theme = null;
    if (radioTheme == "pageTheme") {
        theme = document.getElementById("originTheme").classList[0];
    } else {
        theme = radioTheme;
    }
    return theme;
}


function shotCheckedTheme(theme) {
    var themes = document.getElementsByName("Theme");
    for (var i = 0; i < themes.length; i++) {
        themes[i].checked = false;
        if (themes[i].value == theme) {
            themes[i].checked = true;
        }
    }
}

function setBodyTheme(theme) {
    var body = document.getElementsByTagName("body")[0];
    body.classList.add(theme);
}

function setLinkTheme(theme) {
    for (let i = 0, l = document.links.length; i < l; ++i) {
        let lnk = document.links[i];
        lnk.classList.add(theme);
    }
}

function toggleTheme(obj) {
    var newTheme = transToTheme(obj.value);
    var oldTheme = transToTheme(window.localStorage.getItem("lab2CheckedTheme"));

    changeStoredTheme(newTheme);

    swapTheme(oldTheme, newTheme);
}

function swapTheme(oldT, newT) {
    var body = document.getElementsByTagName("body")[0];
    body.classList.remove(oldT);
    body.classList.add(newT);
}

function changeStoredTheme(theme) {
    window.localStorage.setItem("lab2CheckedTheme", theme);
}

function checkType() {
    var input = document.getElementById("fileInput");
    var thefile = input.files[0];
    var fileName = (String)(thefile.name);
    var ext = fileName.substr(fileName.lastIndexOf("."));

    if (ext != ".pdf" && ext != ".mobi") {
        document.getElementById("labelTypeWarn").style.display = "block";
        document.getElementById("btnUpload").style.display = "none";
    } else {
        document.getElementById("labelTypeWarn").style.display = "none";
        document.getElementById("btnUpload").style.display = "block";

    }


}

