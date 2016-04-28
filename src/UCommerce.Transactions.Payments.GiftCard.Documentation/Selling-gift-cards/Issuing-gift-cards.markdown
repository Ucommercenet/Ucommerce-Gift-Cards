# How are Gift Cards Issued

Gift cards are generated when the order is finalized - which means that when the order is ready to ship, the order will change status from 'New order' to 'Completed'. In this process, the 'ToCompleted' pipeline will execute. We have hooked two tasks in to this pipeline, to issue the gift cards bought and send it using the shipping address of the order.

## Change how gift cards are issued

If you want to change how gift cards are issued you can easily do so by modifying the 'ToCompleted' pipeline and hook the tasks up another place for example the 'Checkout' pipeline. If you're interested in how to achieve that, [please read how to use partial components][1] and [how to register custom components][2]

[1]: http://docs.ucommerce.net/ucommerce/v7.0/extending-ucommerce/create-pipeline-task.html
[2]: http://docs.ucommerce.net/ucommerce/v7.0/extending-ucommerce/register-a-component.html