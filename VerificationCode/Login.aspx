<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="VerificationCode.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="css/style.css" rel="stylesheet" />
    <link href="css/slide-unlock.css" rel="stylesheet" />
      <style>
        body {
            height: 100%;
            background: #16a085;
            overflow: hidden;
        }

        canvas {
            z-index: -1;
            position: absolute;
        }
    </style>
    <script src="js/jquery1.11.1.js"></script>
    <script src="js/Particleground.js"></script>
    <script src="js/slide.js"></script>

</head>
<body>
     <dl class="admin_login">
        <dt>
            <strong></strong>
            <em></em>
        </dt>
        <dd class="user_icon">
            <input type="text" id="userName" placeholder="账号" class="login_txtbx" />
        </dd>
        <dd class="pwd_icon">
            <input type="password" id="passWord" placeholder="密码" class="login_txtbx" />
        </dd>
        <dd class="val_icon">
            <div id="slider">
                <div id="slider_bg"></div>
                <span id="label">>></span>
                <span id="labelTip">拖动滑块验证</span>
            </div>

            <div id="huadongImage" style="width:300px; border:1px solid #ccc;
                height:250px; z-index:200; display:none; position: absolute;
                background-color: white;
                top:40px;">

            </div>
        </dd>
        <dd>
            <input type="button" value="立即登陆" class="submit_btn" />
        </dd>
        <dd>

        </dd>
    </dl>
</body>
</html>
<script>

    $(document).ready(function () {

        $(".submit_btn").click(function () {
            $.ajax({
                "url": "Login.ashx",
                "type": "post",
                "data": {
                    "userName": $("#userName").val(),
                    "passWord": $("#passWord").val()
                },
                "success": function (d) {
                    if (JSON.parse(d).status != "ok") {
                        Slider_init();
                    }
                    alert(JSON.parse(d).msg);
                }
            })


        })


        //粒子背景特效
        $('body').particleground({
            dotColor: '#5cbdaa',
            lineColor: '#5cbdaa'
        });

        Slider_init();
    });


    function Slider_init() {
        var slider = new SliderUnlock("#slider", {
            successLabelTip: "验证成功"
        }, function () {

            huadongCode();
        });
        slider.init();
    }



    function huadongCode() {

        num = 1;
        checkCode = [];
        $.ajax({
            "url": "VerificationCodeImage.ashx",
            "type": "get",
            "success": function (data) {
                console.log(data);
                var html = "<div id=\"imagediv\" style='position: absolute;left:10px; top:30px;background: #fff;z-index:300'><img src=" + JSON.parse(data).result + " alt=\"看不清？点击更换\" id=\"image\"/></div>";
                html += "<div id='divrefresh' style='width:20px;height:20px;position:absolute;cursor: pointer;margin-left: 90%;'> <img src=\"/images/shaxin.jpg\" /> </div>";
                $("#huadongImage").css("display", "block").html(html);
                $("#labelTip").html(JSON.parse(data).msg);
                imageClick();
                divrefreshClick();
            },
            "complete": function (XMLHttpRequest, status) {
                if (status == 'timeout') {

                }
            }
        })
    }


    function divrefreshClick() {
        $("#divrefresh").click(function () {
            huadongCode();
            num = 1;
            checkCode = [];
        })
    }


    var num = 1;
    var checkCode = [];
    function createPoint(pos) {
        if (num == 2) {
            PostcheckCode();
        }

        $("#imagediv").append('<div class="point-area" onclick="pointClick(this)" style="background-color:#539ffe;color:#fff;z-index:9999;width:25px;height:25px;text-align:center;line-height:25px;border-radius: 20%;position:absolute;border:2px solid white;top:' + parseInt(pos.y - 10) + 'px;left:' + parseInt(pos.x - 10) + 'px;">' + num + '</div>');
        ++num;
    }

    function PostcheckCode() {

        $.ajax({
            "url": "CheckCode.ashx",
            "type": "post",
            "data": {
                "code": JSON.stringify(checkCode)
            },
            "success": function (d) {
                console.log(d);
                if (JSON.parse(d).status == "ok") {
                    $("#labelTip").html(JSON.parse(d).msg);
                    $("#huadongImage").hide();
                } else {
                    huadongCode();
                }
            },
            "error": function (error) {

            }
        })

    }

    function pointClick(obj) {
        num = 1;
        checkCode = [];
        $(obj).parent().find('.point-area').remove();
    }
    
    function getMousePos(obj, event) {
        var e = event || window.event;
        var x = e.clientX - ($(obj).offset().left - $(window).scrollLeft());
        var y = e.clientY - ($(obj).offset().top - $(window).scrollTop());
        checkCode.push({ "_X": parseInt(x), "_Y": parseInt(y) });
        return { 'x': parseInt(x), 'y': parseInt(y) };
    }

    function imageClick() {
        $("#imagediv").click(function () {
            var _this = $(this);
            var pos = getMousePos(_this);
            createPoint(pos);
        })

    }
    </script>

