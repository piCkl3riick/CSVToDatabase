<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ImportCSVPersons._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

      <div>
          <asp:FileUpload ID="FileUpload1" runat="server"/>
          <asp:Button ID="btnImport" runat="server" Text="Import" OnClick="btnImport_Click"/>
          <br/>
          <asp:Label ID="Label1" runat="server" ForeColor="Blue"/>
          <br />
          <asp:Label ID="Label2" runat="server" ForeColor="Green"/>
          <br />
          <asp:Label ID="Label3" runat="server" ForeColor="Blue"/>
          <br />
          <asp:Label ID="lblError" runat="server" ForeColor="Red"/>
        </div>

    

</asp:Content>
