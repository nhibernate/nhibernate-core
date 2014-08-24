<%@ Page Language="C#" EnableViewState="false" EnableViewStateMac="false" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>NHibernate Demo</title>
</head>
<body>
    <h1>NHibernate Demo</h1>
    <asp:HyperLink ID="Schema" runat="server" NavigateUrl="~/Schema.aspx">Schema Operations</asp:HyperLink>
    <asp:HyperLink ID="ViewData" runat="server" NavigateUrl="~/ViewData.aspx">View Data</asp:HyperLink>
</body>
</html>
