<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="TZeroHost.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="~/Styles/CSS-Styles.css" />
    <style type="text/css">
        html, body, form
        {
            overflow: hidden;
            height: 100%;
            width: 100%;
        }
    </style>
    <title>Login Geo Codification</title>
</head>
<body>
    <form id="form1" runat="server">
    <table style="background-color: Gray Green; width: 100%; height: 100%">
        <tr>
            <td align="center" style="height: 25%;" valign="bottom">
                <img src="Images/logoLogIn.jpg" width="393px" alt="Terminal Zero" />
            </td>
        </tr>
        <tr style="height: 180px;">
            <td align="center" style="height: 180px;" valign="middle" class="cssPage">
                <asp:Login ID="Login1" runat="server" OnLoggingIn="TryLogIn"
                    LoginButtonText="" HyperLinkStyle-HorizontalAlign="Left"
                    DestinationPageUrl="~/Default.aspx"
                    BorderColor="Transparent" ForeColor="#f0f1f2">
                    <TitleTextStyle CssClass="cssLoginTitle" />
                    <InstructionTextStyle Font-Italic="True" ForeColor="#f0f1f2" />
                    <TextBoxStyle CssClass="cssLoginTextBox"/>
                    <LoginButtonStyle CssClass="cssButtonYes" />
                    <FailureTextStyle ForeColor="#77bfb8" />
                    <HyperLinkStyle ForeColor="#f0f1f2" />
                </asp:Login>
        </tr>
        <tr>
            <td align="center" style="height: 100%;" valign="top">
                <asp:ValidationSummary ID="ValidationSummary1" ValidationGroup="Login1" runat="server"
                    CssClass="cssAppMain" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
