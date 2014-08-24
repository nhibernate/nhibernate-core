<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Schema.aspx.cs" Inherits="Schema" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Schema Operations</title>
</head>
<body>
    <asp:HyperLink runat="server" NavigateUrl="~/Default.aspx">Back to the main page</asp:HyperLink>
    <form id="SchemaOperationsForm" runat="server">
    <div>
        <asp:Button ID="CreateSchema" runat="server" Text="Create Schema" OnClick="CreateSchema_Click" />
        <asp:Button ID="DropSchema" runat="server" Text="Drop Schema" OnClick="DropSchema_Click" />
    </div>
    </form>
    <asp:Label ID="Status" runat="server" Text="" />
</body>
</html>
