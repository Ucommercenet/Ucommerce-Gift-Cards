# Whats in the box

This article will briefly sum up what makes up the gift card app giving you an overview of whats in stalled and how to deal with them. Techincal details and further elaboration can be read in the individual areas. 

## what is a gift card?

Commonly mistaken a voucher code and a gift card is not the same and substituting a voucher with a gift card is not possible as the main difference between the two is:

* A gift card is a pre paid payment method
* A gift card has a balance that will be kept track off

* A voucher is not pre paid
* A voucher has a number of usages that can be handed in for a discount or a free product.

## Installed Components with the Gift Card App

* UI on a product editor to manage gift card prices
 * Will only appear when creating a product of product definition 'GiftCard'
* 'GiftCard' product definition
 * Used to manage gift cards in the backend and to identify a purchased gift card
* 'Gift Card' payment method
 * Used to accept payments with a gift card
* UI to keep track of issued gift cards and UI to manually issue a gift card
 * Will only be present on the payment method when of service type 'Gift Card'
* Ability to export 

## Pipelines modified

To make everything line up when customers are buying and using gift cards we need to hook into the various pipelines that runs associated with browse and checkout. Pipelines modified are elaborated below

### Basket Pipeline

In the basket pipeline we've added two steps.

* A step that removes VAT from the orderline that contains the gift card. 
 * Buying a gift card will not add VAT as these are tax exempt.

* A step that recalculates the amount that should be used on each of the gift cards added
 * The customer will continuously update the basket - maybe even after he has taken a gift card into use. All gift card payments will be recalculated properly.

### Checkout

* A step that redeems the gift cards used
 * When adding gift cards through the checkout flow they will be validated for funds but will not be charged untill the order has been placed. 
 * This step will also throw an exception if the funds authorized is no longer valid preventing fraud.

### ToCompleted

* A step that generates gift cards
 * When a customer has purchased a gift card it will be generated when the order is finalized. 
* A step that sends the gift cards to the shipping address
 * An email will be triggered as part of the checkout pipeline once it has been generated. 

### ToCanceled

This pipeline isn't actually modified. If an order is refunded that amount of the gift card used will be refunded as well.

