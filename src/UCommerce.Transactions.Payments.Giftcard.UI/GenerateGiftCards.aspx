<%@ Page Language="C#" MasterPageFile="../../MasterPages/Dialog.Master" AutoEventWireup="true" CodeBehind="GenerateGiftCards.aspx.cs" Inherits="UCommerce.Transactions.Payments.GiftCard.UI.GenerateGiftCards" %>
<%@ Register tagPrefix="commerce" tagName="ValidationSummary" src="ValidationSummaryDisplay.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentArea" runat="server">
	<div class="propertyPane dialog-header">
	    <h3>
			<span>
				<asp:Localize runat="server" meta:resourcekey="CreateGiftcards"></asp:Localize>
			</span>
		</h3>
        <div>
			<p class="guiDialogTiny">
				<asp:Localize ID="Localize2" runat="server" meta:resourcekey="CreateGiftCardsManchet"></asp:Localize>
			</p>
		</div>
	</div>
	<div class="propertyPane contentPane">
        <commerce:ValidationSummary ID="ValidationSummary1" runat="server" />	    
		<div class="propertyItem">
		    <div class="propertyItemHeader">
		        <asp:Label><asp:Localize ID="Localize3" runat="server" meta:resourcekey="NumberOfGiftCards" /></asp:Label>
		    </div>
            <div class="propertyItemContent">
                <asp:TextBox runat="server" ID="tbNumberOfGiftCards"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RevAmount" ControlToValidate="tbNumberOfGiftCards" ValidationExpression="\d*" ErrorMessage="number of gift cards must be a natural number" Text="*"></asp:RegularExpressionValidator>
                <asp:RangeValidator runat="server" id="rvAmount" ControlToValidate="tbNumberOfGiftCards" MinimumValue="1" MaximumValue="9999999999999999999" ErrorMessage="number of gift cards must be more than zero" Text="*"></asp:RangeValidator>
                <asp:RequiredFieldValidator runat="server" id="rfvAmount" ControlToValidate="tbNumberOfGiftCards" ErrorMessage="number of gift are required to have a value" Text="*"></asp:RequiredFieldValidator>
            </div>
		</div>
		<div class="propertyItem">
		    <div class="propertyItemHeader">
		        <asp:Label><asp:Localize ID="Localize4" runat="server" meta:resourcekey="AvaliableBalance" /></asp:Label>
		    </div>
            <div class="propertyItemContent">
                <asp:TextBox runat="server" ID="tbAmount"></asp:TextBox>
                <asp:RangeValidator runat="server" id="rvBalance" ControlToValidate="tbAmount" MinimumValue="1" MaximumValue="9999999999999999999" ErrorMessage="Balance must be larger than zero" Text="*"></asp:RangeValidator>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="tbAmount" ValidationExpression="^-?[0-9]+((\.|,)[0-9]{1,20})?$" ErrorMessage="balance of gift cards must be a decimal number" Text="*"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator runat="server" id="rfvBakance" ControlToValidate="tbAmount" ErrorMessage="balance of gift card are required to have a value" Text="*"></asp:RequiredFieldValidator>
            </div>
		</div>        
       	<div class="propertyItem">
		    <div class="propertyItemHeader">
		        <asp:Label><asp:Localize ID="Localize5" runat="server" meta:resourcekey="ExpiresOn" /></asp:Label>
		    </div>
            <div class="propertyItemContent">
				<asp:PlaceHolder id="date" runat="server"></asp:PlaceHolder>			                
            </div>
		</div> 
        <div class="propertyItem">
		    <div class="propertyItemHeader">
		        <asp:Label><asp:Localize ID="Localize6" runat="server" meta:resourcekey="SelectCurrency" /></asp:Label>
		    </div>
            <div class="propertyItemContent">
				<asp:DropDownList runat="server" ID="CurrenciesDropDown"/>                
            </div>
		</div>
        <div class="propertyItem">
		    <div class="propertyItemHeader">
		        <Label style="display: block;"><asp:Localize ID="Localize7" runat="server" meta:resourcekey="Note" /></Label>	
		    </div>
            <div class="propertyItemContent">
    			<asp:TextBox CssClass="gcTextArea" runat="server" Width="350px" Height="100px" TextMode="MultiLine" ID="NoteTextBox"></asp:TextBox>            
            </div>
		</div>
	</div>
    <div class="propertyPane dialog-actions">
		<div class="footerOkCancel">
		    <p>
			    <asp:Button id="SaveButton" runat="server" meta:resourcekey="SaveButton" Text="Save" onclick="SaveButton_Click" />
			    <em><asp:Localize id="Localize8" runat="server" meta:resourcekey="Or" Text="Cancel" /></em>
                <a href="#" style="color: blue" onclick="UCommerceClientMgr.closeModalWindow()"><asp:Localize id="Localize1" runat="server" meta:resourcekey="CancelButton" Text="Cancel" /></a>
		    </p>
		</div>
	</div>
</asp:Content>