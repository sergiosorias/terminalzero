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
    </style>
</head>
<body>
    <form id="form1" runat="server" style="height: 100%">
    <table border="0" cellpadding="0" cellspacing="0" style="height: 100%">
        <tr>
            <td colspan="3" class="cssLoginTitle" style="background-color: #4E4F52; height: 42px;">
                <table>
                    <tr>
                        <td style="padding-left: 15px; width: 200px; font-size: 18px; color: White" valign="middle"
                            align="left">
                             <img src="images/logo.png" height="40" width=" 200px" alt="Terminal Zero" />
                        </td>
                        <td style="width: 100%;" align="right">
                            <div style="font-size: 0.5em;" class="cssAppMain">
                                Version 1.0.0.1</div>
                            <asp:LoginName ID="LoginName" runat="server" FormatString="Bienvenido, {0}" CssClass="cssAppMain" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="width: 130px" valign="top" align="center">
                <div style="width: 130px">
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
                                <asp:HyperLink ID="HyperLink1" CssClass="menuLink" NavigateUrl="./TerminalZeroWebClientTestPage.aspx"
                                    runat="server">Terminal Zero</asp:HyperLink>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="width: 130px">
                    <table class="cssGrid" style="width: 100%">
                        <tr>
                            <td class="cssGridHeader">
                                <p>
                                    Downloads
                                </p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:HyperLink ID="HyperLink2" CssClass="menuLink" NavigateUrl="~/Upload/Silverlight_4.exe"
                                    runat="server">Silverlight 4</asp:HyperLink>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
            <td style="height: 100%; width: 100%;" valign="top">
                <iframe src="./TerminalZero.aspx" frameborder="0" style="height: 100%;
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
