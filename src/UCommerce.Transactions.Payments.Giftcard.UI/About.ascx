<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="About.ascx.cs" Inherits="SampleApp.Web.AboutUserControl" %>
<link href="../Apps/SampleApp/Css/About.css" rel="stylesheet" />
<h1>About</h1>

<h3 id="uCommerceVersionHeader" runat="server">uCommerce Version:
	<asp:label id="uCommerceVersion" runat="server"></asp:label>
</h3>

<h3 id="SchemaVersionHeader" runat="server">Schema Version:
	<asp:label id="SchemaVersion" runat="server"></asp:label>
</h3>
