<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditPaymentMethodGiftCards.ascx.cs" Inherits="UCommerce.Transactions.Payments.GiftCard.UI.EditPaymentMethodGiftCards" %>
<%@ Register TagPrefix="commerce" TagName="ValidationSummary" Src="ValidationSummaryDisplay.ascx" %>
<script type="text/javascript">
	
	$(function () {
		$("#productTable").dataTable(
			{
				"bPaginate": false,
				// disable initial sort
				"aaSorting": []
			}
		);
	});
</script>
<div class="propertyPane">
	<commerce:ValidationSummary ID="ValidationSummary" runat="server" /> 
	<asp:CustomValidator runat="server" id="GiftCardValidator" onservervalidate="GiftCard_ServerValidate" Text="*" Display="None" ErrorMessage="<%$ Resources:ValidationError %>" CssClass="validator"> </asp:CustomValidator>
	<h1><asp:Localize runat="server" meta:resourcekey="GiftCardSummary"></asp:Localize></h1>

	<asp:ListView runat="server" ID="CurrencySummaryTable" EnableViewState="false">
		<LayoutTemplate>
			<table class="giftCardSymmaryTable dataTable dataList">
				<thead>
					<tr>
						<th class="centerText"><asp:Localize ID="Localize1" runat="server" meta:resourceKey="Currency" /></th>
						<th class="centerText"><asp:Localize ID="Localize2" runat="server" meta:resourceKey="GiftCardTotal" /></th>
						<th class="centerText"><asp:Localize ID="Localize3" runat="server" meta:resourceKey="PartialUsed" /></th>
						<th class="centerText"><asp:Localize ID="Localize4" runat="server" meta:resourceKey="GiftCardsClosed" /></th>
						<th class="centerText"><asp:Localize ID="Localize5" runat="server" meta:resourceKey="ExpiredGiftCards" /></th>
						<th class="amountInput"><asp:Localize ID="Localize6" runat="server" meta:resourceKey="OutstandingBalance" /></th>
						<th class="amountInput"><asp:Localize ID="Localize7" runat="server" meta:resourceKey="SpentBalance" /></th>
						<th class="amountInput"><asp:Localize ID="Localize8" runat="server" meta:resourceKey="TotalBalance" /></th>
					</tr>
				</thead>
				<tbody>
						<tr runat="server" id="itemPlaceholder" ></tr>
				</tbody>
			</table>
		</LayoutTemplate>
		<ItemTemplate>
			<tr>
				<td class="centerAligned"><%#Eval("Currency.Name")%></td>
				<td class="centerAligned"><%#Eval("TotalNumberOfGiftCards")%></td>
				<td class="centerAligned"><%#Eval("GiftCardsInUse")%></td>
				<td class="centerAligned"><%#Eval("GiftCardsClosed")%></td>
				<td class="centerAligned"><%#Eval("ExpiredGiftCards")%></td>
				<td class="rightAligned"><%#Eval("OutstandingBalance")%></td>
				<td class="rightAligned"><%#Eval("SpentBalance")%></td>
				<td class="rightAligned"><%#Eval("TotalBalance")%></td>
			</tr>
		</ItemTemplate>
		<EmptyDataTemplate>
              <table class="emptyTable" cellpadding="5" cellspacing="5">
                <tr>
                  <td>
                    <asp:Localize ID="Localize4" runat="server" meta:resourceKey="NoGiftCards" />
                  </td>
                </tr>
              </table>
          </EmptyDataTemplate>
	</asp:ListView>

</div>
<div class="propertyPane">
	<h1><asp:Localize ID="Localize9" runat="server" meta:resourcekey="GiftCards" /></h1>
	<div>
		<asp:ListView runat="server" ID="GiftCardListView" EnableViewState="false" OnPagePropertiesChanging="GiftCardListView_PagePropertiesChanging" >
			<LayoutTemplate>
				<table id="productTable" class="dataTable dataList" >
					<thead>
						<tr>
							<th><asp:Localize ID="Localize10" runat="server" meta:resourceKey="Code" /></th>
							<th class="rightAligned"><asp:Localize ID="Localize11" runat="server" meta:resourceKey="Amount" /></th>
							<th class="rightAligned"><asp:Localize ID="Localize12" runat="server" meta:resourceKey="Amountused" /></th>
							<th class="centerText"><asp:Localize ID="Localize13" runat="server" meta:resourceKey="Currency" /></th>
							<th><asp:Localize ID="Localize14" runat="server" meta:resourceKey="ExpiresOn" /></th>
							<th><asp:Localize ID="Localize15" runat="server" meta:resourceKey="CreatedOn" /></th>
							<th><asp:Localize ID="Localize16" runat="server" meta:resourceKey="CreatedBy" /></th>
							<th><asp:Localize ID="Localize17" runat="server" meta:resourceKey="OrderNumber" /></th>
							<th><asp:Localize ID="Localize18" runat="server" meta:resourceKey="Note" /></th>
							<th class="centerText"><asp:Localize ID="Localize19" runat="server" meta:resourceKey="Enabled" /></th>	
						</tr>
					</thead>
					<tbody>
						<tr runat="server" id="itemPlaceholder" ></tr>
					</tbody>
				</table>
			</LayoutTemplate>
			<ItemTemplate>
				<tr id="Tr1" runat="server">
					
					<td><asp:HiddenField runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "GiftCardId" )%>' ID="GiftCardIdHiddenField"/><%# DataBinder.Eval(Container.DataItem, "Code") %></td>
					<td class="rightAligned"><%# DataBinder.Eval(Container.DataItem, "Amount", "{0:n}")%></td>
					<td class="rightAligned"><%# DataBinder.Eval(Container.DataItem, "AmountUsed", "{0:n}")%></td>
					<td class="centerAligned"><%# DataBinder.Eval(Container.DataItem, "Currency.Name")%></td>
					<td><%# DataBinder.Eval(Container.DataItem, "ExpiresOn")%></td>
					<td><%# DataBinder.Eval(Container.DataItem, "CreatedOn")%></td>
					<td><%# DataBinder.Eval(Container.DataItem, "CreatedBy")%></td>
					<td><%# DataBinder.Eval(Container.DataItem, "OrderNumber")%></td>
					<td>
						<span class="oneLineText"><%# DataBinder.Eval(Container.DataItem, "Note")%></span>
					</td>
					<td class="centerAligned">
						<asp:CheckBox Checked='<%# DataBinder.Eval(Container.DataItem, "Enabled" )%>' ID="GiftCardEnabledCheckBox" runat="server"></asp:CheckBox>
					</td>
				</tr>
			</ItemTemplate>
		</asp:ListView>
		<asp:DataPager runat="server" ID="GiftCardPager" PageSize="50" PagedControlID="GiftCardListView">
			<Fields>
				<asp:NumericPagerField 
				ButtonType="Link" ButtonCount="30" PreviousPageText="<--" NextPageText="-->"
				/>
			</Fields>
		</asp:DataPager>
	</div>
		<div class="propertyPaneFooter">
	</div>
</div>