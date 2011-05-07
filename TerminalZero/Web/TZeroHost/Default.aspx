<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TZeroHost._Default" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Main Page</title>
    <link rel="stylesheet" type="text/css" href="~/Styles/CSS-Styles.css" />
    <style type="text/css">
        html, body
        {
            height: 100%;
            width: 100%;
            margin: 0px;
            overflow: auto;
        }
        .menuLink:link
        {
            font-family: Tahoma;
            font-size: 12px;
            color: #a4cde5;
            text-decoration: none;
        }
        .menuLink:visited
        {
            font-family: Tahoma;
            font-size: 12px;
            color: #a4cde5;
            text-decoration: none;
        }
        .menuLink:hover
        {
            font-family: Tahoma;
            font-size: 13px;
            color: #1D3057;
            text-decoration: none;
            font-weight: bold;
        }
        .menuLink:active
        {
            font-family: Tahoma;
            font-size: 13px;
            color: #0066FF;
            text-decoration: none;
        }
        
        .headerButton
        {
            font-weight: bold;
            font-family: Tahoma;
            font-size: 14px;
            color: White;
            text-decoration: none;
        }
        
        .headerButtonParent
        {
            cursor: hand;
            text-align: center;
            top: 1px;
            left: 300px;
            width: 120;
            height: 35;
            position: absolute;
            background-repeat: no-repeat;
            background-image: url('./images/btnNormal.png');
        }
        
        ul.topnav
        {
            list-style: none;
            padding: 0 20px;
            margin: 0;
            float: left;
            width: 400;
            background: #222;
            font-size: 15px;
            text-align:center;
            vertical-align:middle;
            background: url(topnav_bg.gif) repeat-x;
        }
        ul.topnav li
        {
            float: left;
            margin: 0;
            padding: 0 15px 0 0;
            position: relative; /*--Declare X and Y axis base for sub navigation--*/
        }
        ul.topnav li a
        {
            padding: 10px 5px;
            color: #fff;
            display: block;
            text-decoration: none;
            float: left;
        }
        
        ul.topnav li span
        {
            /*--Drop down trigger styles--*/
            width: 40px;
            height: 35px;
            float: left;
            background: url(Images/semaforo_blanco.png) no-repeat center center;
        }
        ul.topnav li span.subhover
        {
            background: url(Images/semaforo_verde.png) no-repeat center center;
            cursor: pointer;
        }
        /*--Hover effect for trigger--*/
        ul.topnav li ul.subnav
        {
            list-style: none;
            position: absolute; /*--Important - Keeps subnav from affecting main navigation flow--*/
            left: 0;
            top: 35px;
            background: #333;
            margin: 0;
            padding: 0;
            display: none;
            float: left;
            width: 200px;
            border: 1px solid #111;
        }
        ul.topnav li ul.subnav li
        {
            margin: 0;
            padding: 0;
            border-top: 1px solid #252525; /*--Create bevel effect--*/
            border-bottom: 1px solid #444; /*--Create bevel effect--*/
            clear: both;
            width: 200px;
        }
        html ul.topnav li ul.subnav li a
        {
            font-size:10px;
            float: left;
            width: 180px;
            background: #333 url(Images/icon_template_inac.png) no-repeat 10px center;
            padding-left: 30px;
        }
        html ul.topnav li ul.subnav li a:hover
        {
            /*--Hover effect for subnav links--*/
            background: #222 url(Images/icon_template_active.png) no-repeat 10px center;
        }
    </style>
    <script type="text/javascript" language="javascript" src="Scripts/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" language="javascript" src="Scripts/jquery-1.4.1-vsdoc.js"></script>
    <script type="text/javascript" language="javascript" src="Scripts/jquery-1.4.1.js"></script>
</head>
<body>
    <form id="form1" runat="server" style="height: 100%; width: 100%">
    <input type="hidden" runat="server" value="" id="filesToShow" />
    <div id="menuHeader" class="headerButtonParent" style="width: 140; background-image: url('');">
        <ul class="topnav">
            <li><a href="#">Links</a>
                <ul class="subnav">
                    <li><a id="HyperLink1" href="#">Web Control</a></li>
                    <li><a id="HyperLink2" href="#">TS - Server</a></li>
                </ul>
            </li>
            <li><a href="#">Download</a>
                <ul class="subnav" id="filesToDownload">
                </ul>
            </li>
        </ul>
    </div>
    <div id="minimizedHeader" style="display: none" class="headerButtonParent">
        <a class="headerButton">Show Header</a>
    </div>
    <script>
        $(document).ready(function () {
            $("ul.subnav").parent().append("<span/></span>"); //Only shows drop down trigger when js is enabled (Adds empty span tag after ul.subnav*)
            var array = $("#filesToShow").attr("value").toString().split('|');
            for (file in array) {
                if (array[file]) {
                    $("#filesToDownload").append("<li><a href=\"upload/" + array[file] + "\" target=\"_blank\">" + array[file] + "</a></li>");
                }
            }
            
            $("ul.topnav li span").click(function () { //When trigger is clicked...
                //Following events are applied to the subnav itself (moving subnav up and down)

                $(this).parent().find("ul.subnav").slideDown('fast').show(); //Drop down the subnav on click

                $(this).parent().hover(function () {
                }, function () {
                    $(this).parent().find("ul.subnav").slideUp('normal'); //When the mouse hovers out of the subnav, move it back up
                });

                //Following events are applied to the trigger (Hover events for the trigger)
            }).hover(function () {
                $(this).addClass("subhover"); //On hover over, add class "subhover"
            }, function () {	//On Hover Out
                $(this).removeClass("subhover"); //On hover out, remove class "subhover"
            });

        });

        $(document).ready(function () {
            $(headerContent).click(function (event) { $(headerContent).hide("slow"); $(minimizedHeader).show("slow"); $(menuHeader).hide("slow") });
            $(minimizedHeader).click(function () { $(headerContent).show("slow"); $(minimizedHeader).hide("slow"); $(menuHeader).show("slow") });

            $("#HyperLink1").click(function () {
                setFrameSource('./TerminalZero.aspx');
            });
            
            $("#HyperLink2").click(function () {
                setFrameSource("./tsweb");
            });
        });

        function setFrameSource(source) {
            $('#linkContent').attr("src", source);
        }
    </script>
    <table border="0" cellpadding="0" cellspacing="0" style="height: 100%; width: 100%">
        <tr>
            <td id="headerContent" colspan="3" class="cssLoginTitle" style="background-color: #4E4F52;
                height: 42px;">
                <table cellpadding="0" cellspacing="0" style="width: 100%">
                    <tr>
                        <td style="padding-left: 15px; width: 180px; font-size: 18px; color: White" valign="middle"
                            align="left">
                            <img src="images/logo.png" height="40" width="180px" alt="Terminal Zero" />
                        </td>
                        <td align="right" style="width: 100%; padding-right: 5px">
                            <div style="font-size: 0.5em;" class="cssAppMain">
                                Version 1.0.0.2</div>
                            <asp:LoginName ID="LoginName" runat="server" FormatString="Bienvenido, {0}" CssClass="cssAppMain" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="height: 100%; width: 100%;" valign="top">
                <iframe id="linkContent" frameborder="0" src="TerminalZero.aspx" style="height: 100%;
                    width: 100%"></iframe>
            </td>
        </tr>
        <tr>
            <td style="height: 22px; padding-left: 30px; font-size; 18px; background-color: #4E4F52;
                color: White" valign="middle" align="center" colspan="3" class="cssLoginTitle">
                <p>
                    N&S Systems
                </p>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
