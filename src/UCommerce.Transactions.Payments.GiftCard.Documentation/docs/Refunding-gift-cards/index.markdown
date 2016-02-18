# Refunding gift cards

Like every other order, an order placed with a gift card payment might need to be returned for various reasons. Gift cards payments can be refunded like a credit card payment and are supported out of the box.

## Automatic refund when order is cancelled 

Out of the box with uCommerce comes the ability to refund orders for payment gateways that supports that funcionallity. This is per default configured in the ToCanceled pipeline. Gift card payments are configured to be refunded in this piepline as well. This means that when you cancel an order the gift card payment will automatically be refunded, and the user can spent the balance on another order.

## Change how gift cards are refunded

If you want to change how gift cards are refunded you can easily do so by modifying the 'ToCanceled' pipeline and hook the tasks up in another pipeline. If you're interested in how to achieve that, [please read how to use partial components][1] and [how to register custom componets][2]

[1]: http://docs.ucommerce.net/ucommerce/v7.0/extending-ucommerce/create-pipeline-task.html
[2]: http://docs.ucommerce.net/ucommerce/v7.0/extending-ucommerce/register-a-component.html