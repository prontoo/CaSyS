<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="PayFailed.aspx.cs" Inherits="web.PayFailed" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
    <asp:Literal ID="LiteralInfo" runat="server"></asp:Literal>
</div>
<div>
    <asp:Literal ID="LiteralReasonOfDecline" runat="server"></asp:Literal>
</div>
<a href="invoices.nspx" runat="server" id="invoiceсWithPay">Враќање на Фактури</a>

</asp:Content>
