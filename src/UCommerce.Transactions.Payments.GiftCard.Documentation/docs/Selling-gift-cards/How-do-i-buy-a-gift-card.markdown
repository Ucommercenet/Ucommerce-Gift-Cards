# How do I Buy a Gift Card

When buying gift cards you have two options:
* Buy a gift card using the fixed prices entered in the backend.
* Let the customers choose their own price not present in the price tier.

This article will describe both scenarios.

Before we dive into the code you need to know that we're adding a variant to the basket, which will hold the price configured in the backend as described in [How to Create a Gift Card Product][1]. 

[1]: /Selling-gift-cards/create-a-gift-card-product.markdown 

## Presenting the gift cards

Say you want to present your gift card prices in a dropdown, the following code will allow you to show each of the prices. Current price group will automatically be taken into account.

Please note that you need to present the amount without Tax as gift cards are purchased without tax added. 
Please also note that the price showed is calculated so any discounts applied will be taken into account in "YourPrice", but will not affect the price issued on the gift card.
Alternatively, you can also show the list price, which is the price, configured in the backend and the amount issued on the gift card regardless of discounts.

{CODE-START:csharp /}
		
	<select name="variantSku">
	@foreach(var variant in product.Variants) {
		var variantPrice = CatalogLibrary.CalculatePrice(variant);
		<option value="@variant.VariantSku">@variantPrice.YourPrice.AmountExclTax</option> 

		//Alternatively show the list price
		//<option value="@variant.VariantSku">@variantPrice.ListPrice.AmountExclTax</option> 
	}
	</select>

{CODE-END /} 

## Buy a Gift Card Using Prices Entered in the Backend

In order to add a gift card to the basket, all you need to do is use TransactionLibrary exactly like adding another product to the basket:

{CODE-START:csharp /}

	UCommerce.Api.TransactionLibrary.AddToBasket(1, "sku", "variantSku");

{CODE-END /} 

## Let the Customer Choose the Price

If you want to let the customer choose a custom amount on the gift card issued, all you need to do is collection that information and use an overloaded parameter on AddToBasket. You do need to select a variant though, but the variant selected is not used for anything so it might as well just be the first variant on the gift card product.


{CODE-START:csharp /}
		
	TransactionLibrary.AddToBasket(1, product.Sku, product.Variants.First().VariantSku, unitPrice: 123, addToExistingLine: false);

{CODE-END /} 

That is all that is needed for now. The checkout flow will automatically take care of everything else. All you need to worry about now is letting the customer use the gift card once it has been issued.