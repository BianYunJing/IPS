var requestUrl = 'https://localhost:44305/';
var requstFullUrl = requestUrl + 'api/values/';
var requestTokenUrl = requestUrl + 'Authentication/RequestToken'
var requestTokenUrlPart = 'Authentication/RequestToken'
var token = '';



window.onload = function () {
    var lochref = location.href;
    var storage = window.sessionStorage;
    var token = sessionStorage.getItem("token");
    var usertype = storage.getItem("usertype");
    var username = storage.getItem("username");
    var isnotvalid = (token == null || token == "" || usertype == "" || username == "") ? true : false;
    var isbeforeLogin = (lochref.indexOf(".html") == -1 || lochref.indexOf("login.html") > 0) ? true : false;
    if (!isbeforeLogin && isnotvalid) {
        location.href = "/login.html";
        storage.setItem("isforceQuit", true);
    }
    else if (isbeforeLogin && !isnotvalid) {
        location.href = "/index.html";
    }
    if (isbeforeLogin && storage.getItem("isforceQuit") == "true" && token == "") {
        alert("登录已失效，请重新登录");
        storage.setItem("isforceQuit", false);
    }
}

function login() {
    var user = $("#account").val().toString();
    var pwd = $("#password").val().toString();
    if (user == "" || pwd == "") {
        alert("账号/密码不能为空！");
        return;
    }
    var jdata = JSON.stringify(JSON.parse("{ \"username\": \"" + user + "\", \"password\": \""+pwd+"\" }"));
    //var jdata = '{ \"username\": \"StevieLi\", \"password\": \"Yzj123!\" }';

    var res = true;
    $.ajax({
        async: false,
        type: "post",
        contentType: "application/json;charset=utf-8",
        url: requestTokenUrl,
        data: jdata,
        dataType: "json",
        //beforeSend: function (xhr) {   //Include the bearer token in header
        //    xhr.setRequestHeader("Authorization", 'Bearer ' + window.sessionStorage.getItem("token"));
        //},
        complete: function (xhr) {
            var storage = window.sessionStorage;
            //var resObj = JSON.parse(xhr.responseText); //bug ,can't parse json to jsonobject with key value mode  
            var resArr = xhr.responseText.replaceAll('{', '').replaceAll('}', '').replaceAll('\"', '').replaceAll('\\', '').replaceAll('"', '').split(',');
            var resPartArr = [resArr[0].split(':')[1], resArr[1].split(':')[1]];
            if (resPartArr[0] == "0" || resPartArr[0]>3) {
                alert(resPartArr[1]);
                return;
            }
            else {
                storage.setItem("token", resPartArr[1]);
                storage.setItem("usertype", resPartArr[0]);
                storage.setItem("username",user);
                location.href = "/index.html";
            }
        },
        error: function (message) {
            if (message > 0) {
                alert(message);
            }
        }
    });
    return res;
}

function logout() {
    var storage = window.sessionStorage;
    storage.setItem("token", "");
    storage.setItem("username", "");
    storage.setItem("usertype", "");
    location.href = "/login.html";
}

function register() {
    var user = $("#username").val().toString();
    var pwd = $("#passwordsignin").val().toString();
    var fullname = $("#fullname").val().toString();
    var pwdii = $("#confirmpassword").val().toString();
    var email = $("#email").val().toString();
    if (pwd != pwdii) {
        alert("两次密码输入不一致，请重新确认！");
        return;
    }
    else if (pwd.length < 8) {
        alert("密码长度过短，请重新设定！");
        return;
    }
    var jdata = JSON.stringify(JSON.parse("{  \"roleID\": \"" + user + "\",  \"roleName\": \"" + fullname + "\",  \"email\": \"" + email + "\",  \"password\": \"" + pwd + "\"}"));
    //var jdata = '{ \"username\": \"StevieLi\", \"password\": \"Yzj123!\" }';

    var res = true;
    $.ajax({
        async: false,
        type: "post",
        contentType: "application/json;",//charset=utf-8",
        url: requestUrl + "RoleManagement/InsertRole",
        data: jdata,
        dataType: "json",
        complete: function (xhr) {
            //var storage = window.sessionStorage;
            var resArr = xhr.responseText.replaceAll('{', '').replaceAll('}', '').replaceAll('\"', '').split(',');//.replaceAll('\\', '').replaceAll('"', '').split(',');
            var resPartArr = [resArr[0].split(':')[1], resArr[1].split(':')[1]];
            alert(resPartArr[1]);
            $("#show-signin").click(); //back to signin
        }
    });
    return res;
}

function getRoleInfo(_roleId) {  
    var res;
    var tempstr;
    $.ajaxSettings.async = false; //必须加的，若不加返回的是""
    $.getJSON(requstUrl + "/RoleManagement/GetRoleByInfo", { "roleId": _roleId.toString() }, function (data) {
        res = data;
    });
    $.ajaxSettings.async = true; //必须加的，若不加返回的是""
    if (res != null) {
        tempstr = JSON.stringify(res).replace(/"level":"1"/g, '"level":"系统管理员"').replace(/"level":"2"/g, '"level":"管理员"').replace(/"level":"3"/g, '"level":"一般用户"');
        res = JSON.parse(tempstr);
    }
    return res;
}
