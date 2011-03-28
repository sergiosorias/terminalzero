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
            text-align:center;
            top: 1px; 
            left: 300px; 
            width:120;
            height:35;
            position: absolute; 
            background-repeat:no-repeat;
            background-image: url('./images/btnNormal.png');
        }
    </style>
    <script type="text/javascript" src="Scripts/jquery-1.4.1.min.js" />
    <script type="text/javascript" src="Scripts/jquery-1.4.1-vsdoc.js" />
    <script type="text/javascript" src="Scripts/jquery-1.4.1.js" />
    <script type="text/javascript">
        

    </script>
</head>
<body>
    <form id="form1" runat="server" style="height: 100%; width: 100%">
    <div id="menuHeader" class="headerButtonParent">
        <a class="headerButton">Menu</a>
    </div>
    <div id="minimizedHeader" style="display:none" class="headerButtonParent">
        <a class="headerButton">Show Header</a>
    </div>
    <div id="linksDiv" style="background-color:transparent; width:140; display:none" class="headerButtonParent">
        <table class="cssGrid" style="width: 100%">
            <tr>
                <td align="center" class="cssGridHeader">
                    <p style="">
                        Links
                    </p>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:HyperLink ID="HyperLink1" CssClass="menuLink" NavigateUrl="#" runat="server">Web Control</asp:HyperLink>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:HyperLink ID="HyperLink2" CssClass="menuLink" NavigateUrl="#" runat="server">TS - Server</asp:HyperLink>
                </td>
            </tr>
            <tr>
                <td class="cssGridHeader">
                    <p>
                        Downloads
                    </p>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:HyperLink ID="HyperLink3" CssClass="menuLink" Target="_blank" NavigateUrl="~/Upload/Silverlight_4.exe"
                        runat="server">Silverlight 4</asp:HyperLink>
                </td>
            </tr>
        </table>
    </div>
    
    <script>
        $(document).ready(function () {
            $(linksDiv).hover(open, close);
            $(menuHeader).click(function (event) { event.preventDefault(); open(event) });
            $(headerContent).click(function (event) { $(headerContent).hide("slow"); close(event); $(minimizedHeader).show("slow"); $(menuHeader).hide("slow") });
            $(minimizedHeader).click(function () { $(headerContent).show("slow"); $(minimizedHeader).hide("slow"); $(menuHeader).show("slow") });
            $("#HyperLink1").click(function () {
                setFrameSource('./TerminalZero.aspx');
            });
            $("#HyperLink2").click(function () {
                setFrameSource('http://tzhost.dyndns-server.com/tsweb');
            });
        });

        function close(event) {
            $(linksDiv).slideUp("fast");
        }

        function open(event) {
            $(linksDiv).slideDown("normal");
            $(linksDiv).offset.left = event.pageX;
            $(linksDiv).offset.Top = event.pageY;
        }

        function setFrameSource(source) {
            $('#linkContent').attr("src", source);
        }
    </script>
    <table border="0" cellpadding="0" cellspacing="0" style="height: 100%; width: 100%">
        <tr>
            <td id="headerContent" colspan="3" class="cssLoginTitle" style="background-color: #4E4F52; height: 42px;">
                <table cellpadding="0" cellspacing="0" style="width: 100%" >
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
