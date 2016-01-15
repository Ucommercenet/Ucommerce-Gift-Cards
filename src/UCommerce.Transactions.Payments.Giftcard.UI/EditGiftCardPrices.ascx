<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditGiftCardPrices.ascx.cs" Inherits="UCommerce.Transactions.Payments.GiftCard.UI.EditGiftCardPrices" %>
<%@ Import Namespace="UCommerce.EntitiesV2" %>
<%@ Register tagPrefix="commerce" tagName="ValidationSummary" src="ValidationSummaryDisplay.ascx" %>
<%@ Register TagPrefix="commerce" Namespace="UCommerce.Presentation.Web.Controls" Assembly="UCommerce.Presentation" %>
<commerce:ValidationSummary ID="ValidationSummary1" runat="server" />

<script type="text/javascript">
	function confirm_delete_variant() {
		return (confirm('Are you sure you want to delete?') == true);
	}
	$('document').ready(function () {

		$('.newVariantRow input[type=image]').click(removeNewVariantClick);
		$('.addButton').click(addNewVariantClick);
	});
	
	
	function removeNewVariantClick() {
		var button = $(this);
		var row = button.parent().parent();
		
		row.find('.priceInput').attr('disabled', 'disabled');
		row.find(".hiddenContainer input").first().val('false');
		row.find('.priceInput').addClass('aspNetDisabled');
		row.find('.priceInput').val('0');
		row.find('.priceInput').removeClass('priceInput');
		row.css('display', 'none');

		toggleAddButton();

		return false;
	}
	
	function addNewVariantClick() {

		var row = $(".newVariantRow:hidden").first();

		row.css('display', 'table-row');
		row.find('.aspNetDisabled').removeAttr('disabled');
		row.find('.aspNetDisabled').addClass('priceInput');
		row.find(".hiddenContainer input").first().val('true');
		row.find('.aspNetDisabled').removeClass('aspNetDisabled');

		row.removeClass('hide');
	}
	
</script>
<div class="propertyPane" style="display:table;width:95%;">
    <table>
        <tr>
            <th></th>
            <td>
                <commerce:BulkEditGridView BulkEdit="true" runat="server" ID="GiftCardPrices" DataSource="<%# Variants %>" ShowHeader="true" DataKeyNames="ProductId" AutoGenerateColumns="false" GridLines="None" CssClass="variantContainer" UseAccessibleHeader="True">
                    <Columns>
                        <asp:TemplateField meta:resourceKey="CommandHeader" HeaderText="">
                            <EditItemTemplate>
                                <asp:HiddenField runat="server" ID="ProductIdHiddenVariantField" Value='<%# ((Product)Container.DataItem).ProductId %>' />
                                <div class="hiddenContainer">
									<asp:HiddenField runat="server" ID="AddVariantHiddenField" Value="false"/>									
								</div>
								<asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="Media/delete.png" Width="16px" Height="16px" CommandArgument='<%# ((Product)Container.DataItem).ProductId %>' CommandName="Delete" AlternateText="Delete" OnClientClick="return confirm_delete_variant();" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </commerce:BulkEditGridView>
            </td>
        </tr>
    </table>
</div>
