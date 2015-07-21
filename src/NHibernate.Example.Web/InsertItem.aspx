<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InsertItem.aspx.cs" Inherits="InsertItem" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Insert New Item</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ObjectDataSource ID="ItemList" runat="server"
            SelectMethod="GetAllItems" TypeName="NHibernate.Example.Web.Persistence.ItemList" DataObjectTypeName="NHibernate.Example.Web.Domain.Item" DeleteMethod="DeleteItem" InsertMethod="InsertItem" UpdateMethod="UpdateItem">
        </asp:ObjectDataSource>
        <asp:FormView ID="InsertForm" runat="server" DefaultMode="Insert" DataKeyNames="Id" DataSourceID="ItemList">
            <InsertItemTemplate>
                <table>
                    <tr>
                        <td>Price:</td>
                        <td><asp:TextBox ID="Price" runat="server" Text='<%# Bind("Price") %>' /></td>
                    </tr>
                    <tr>
                        <td>Description:</td>
                        <td><asp:TextBox ID="Description" runat="server" Text='<%# Bind("Description") %>' /></td>
                    </tr>
                    <tr>
                        <td colspan="2" align="right">
                            <asp:Button ID="OKButton" runat="server" Text="OK" Width="6em" OnClick="OKButton_Click" />
                            <asp:Button ID="CancelButton" runat="server" Text="Cancel" Width="6em" CausesValidation="False" OnClick="CancelButton_Click" /></td>
                    </tr>
                </table>
            </InsertItemTemplate>
        </asp:FormView>
    </div>
    </form>
</body>
</html>
