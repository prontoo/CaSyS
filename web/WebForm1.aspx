<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="web.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="invoiceContainer" class="tableInvoices" style="width: 100%; text-align: center;
        padding: 20px 0px 35px 0px">
        <div style="margin: 10px 0px; text-align: left; font-weight: bold; color: #E20074; margin: 0px 10px; font-size: 13px;">
            <asp:Literal ID="ltrSuccessMsg"
                runat="server"></asp:Literal>
        </div>
        <table cellspacing="2" cellpadding="4" border="0" width="620px" style="width: 620px"
            id="InvoicesListData">
            <tr class="dnaslovmagenta_center">
                <td width="10">
                    #
                </td>
                <td width="120" align="center">
                    <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                </td>
                <td width="120" align="center">
                    <asp:Literal ID="Literal2" runat="server"></asp:Literal>
                </td>
                <td width="120" align="center">
                    <asp:Literal ID="Literal3" runat="server"></asp:Literal>
                </td>
            </tr>
            <asp:Repeater ID="RepeaterInvoices" runat="server">
                <ItemTemplate>
                    <tr class="dorange40_center">
                        <td>
                            <%# (Container.ItemIndex + 1) + "." %>
                        </td>
                        <td>
                            <%# Eval("InvoiceNumber")%>
                        </td>
                        <td class="priceCol">
                            <%# Eval("InvoicePayedAmount")%>
                        </td>
                        <td class="deleteCol">
                            <asp:Literal ID="Literal4" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr class="dorange30_center">
                <td colspan="2">
                </td>
                <td class="middle">
                    <asp:Literal ID="ltrVkupno" runat="server"></asp:Literal>
                </td>
                <td>
                </td>
            </tr>
        </table>
        <asp:HyperLink ID="invoiceсWithPay" runat="server" NavigateUrl="~/invoices.nspx"></asp:HyperLink>
    </div>
</asp:Content>
