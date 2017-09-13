# How do I Use a Gift Card

When the gift card is issued, your customer is ready to use it in the webshop. As described in [Whats in the box][1] gift cards has individual balances and will automatically be spent based on the order total of the order.	

[1]: intro.markdown

## Gift Cards are used as a Payment Method

As described in [Whats in the box][1] a payment method "Gift Card" is created. This Payment method is used to add payments to the order once you start using gift cards. Using a gift card on the order will result in a payment created using that payment method.
However the "Gift Card" payment method should not be used like a regular payment method. The user should not see it as a valid option, nor select it manually while checking out, so you need to hide it from the user. Instead, use the API provided to add a giftcard. See the section "Spending Your Gift Card" for code samples.
## Gift card payments are automatically adjusted

As part of the strategy to make it easy to use gift cards, the payment associated with a gift card will automatically be adjusted in the basket pipeline. This ensures that the right balance are spent on the gift card based on the order total of the order. If multiple gift cards are used, the gift card with the smallest balance will be used first.

## Spending Your Gift Card

In order to spend a gift card, just use the provided API to do so. When gift cards are issued, they will have a generated code that needs to be used.

{CODE-START:csharp /}
		
	UCommerce.Transactions.Payments.GiftCard.Api.GiftCardLibrary.UseGiftCard(giftCardCode);

{CODE-END /} 

Once the gift card has been used, the API will automatically use the given code to look for a valid gift card and add it to the order. The Api will return the created payment, which will be null if the gift card for some reason isn't valid.

## Spending multiple Gift Cards on a Single Order

Got 1 or 2 or even 3 gift cards? No problem! With the app you can safely add multiple gift cards to the same order. You don't have to accommodate for this in a special way. Just add the gift card to the order with the code above, and everything else is taken care of. The default strategy is designed so we're trying to close as many gift cards as possible, by spending the gift cards with the least balance first.

Consider the following scenarios:

*Scenario 1:*

One basket with a total of 1000 is added.

One gift card with a value of 500 is used on the basket. This means that you will get a payment on the order with value 500. When the user checks out, we still need to collect a payment of 500 on the basket, so the user needs to be redirected to a payment gateway to use a credit card for the remaining 500. The remaining value on the gift card are now 0 and considered closed.

*Scenario 2:*

One basket with a total of 1000 is added.

Two gift cards of value 500 each are used on the basket. This means that you will get two payments on the order with value 500 on each. When the user checks out, no further payments needs to be collected and the order can be placed. The remaining value on both gift cards are now 0 and considered closed.   

*Scenario 3:*

One basket with a total of 1000 is added.

Two gift cards are used on the order. Gift card number 1 has a balance on 800 and the other gift card on 400. This means that when the user checks out the gift card with a balance of 400 is closed first, which spends 600 on the next gift card leaving the balance at 200 after the order has checked out.

The payments will automatically be adjusted every time the basket pipeline runs to accommodate for the changes to the order total. 

## How is the Gift Card Redeemed

When a gift card is used on the order, it will initially be placed as an authorized payment on the order. This payment will stay there as a full or partial payment of the order. When the user checks out and all payments is authorized, the checkout pipeline will be executed. The first step in the checkout pipeline is redeeming the gift cards used by adjusting the balance available on the gift card. This will also ensure that the balance on the gift card is still valid making it impossible to fraud with the gift cards.