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
        }
        .farsiLink:link
        {
            font-family: Tahoma;
            font-size: 12px;
            color: #485B85;
            text-decoration: none;
        }
        .farsiLink:visited
        {
            font-family: Tahoma;
            font-size: 12px;
            color: #0066FF;
            text-decoration: none;
        }
        .farsiLink:hover
        {
            font-family: Tahoma;
            font-size: 13px;
            color: #1D3057;
            text-decoration: none;
            font-weight: bold;
        }
        .farsiLink:active
        {
            font-family: Tahoma;
            font-size: 13px;
            color: #0066FF;
            text-decoration: none;
        }
    </style>
</head>
<body style="height: 100%">
    <form id="form1" runat="server" style="height: 100%">
    <table border="0" cellpadding="0" cellspacing="0" style="height: 100%">
        <tr>
            <td style="height: 42px; padding-left: 30px; font-size; 18px; background-color: #4E4F52;
                color: White" valign="middle" align="left" colspan="3" class="cssLoginTitle">
                <p>
                    Terminal Zero Web
                </p>
            </td>
        </tr>
        <tr>
            <td style="width: 200px" valign="top" align="center">
                <div style="width: 200px">
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
                                <asp:HyperLink ID="HyperLink1" CssClass="farsiLink" NavigateUrl="./TerminalZeroWebClientTestPage.aspx"
                                    runat="server">Terminal Zero</asp:HyperLink>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="width: 200px">
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
                                <asp:HyperLink ID="HyperLink2" CssClass="farsiLink" NavigateUrl="~/Downloads/Basics/Silverlight_4.exe"
                                    runat="server">Silverlight 4</asp:HyperLink>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
            <td style="height: 100%; width: 100%;" valign="top">
                <iframe src="./TerminalZeroWebClientTestPage.aspx" frameborder="0" style="height: 100%;
                    width: 100%"></iframe>
            </td>
        </tr>
        <tr>
            <td style="height: 22px; padding-left: 30px; font-size; 18px; background-color: #4E4F52;
                color: White" valign="middle" align="center" colspan="3" class="cssLoginTitle">
                <p>
                    Sebastian Etchevest
                </p>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
