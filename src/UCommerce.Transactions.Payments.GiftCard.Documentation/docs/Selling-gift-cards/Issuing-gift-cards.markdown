# How are gift cards issued

Gift cards are generated when the order is finalized - which means that when the order is ready to ship, the order will change status from 'New order' to 'Completed'. In that process the 'ToCompleted' pipeline will be executed. In this pipeline we've hooked in two tasks to issue the gift cards bought and send it using the shipping address of the order.

## Change how gift cards are issued

If you want to change how gift cards are issued you can easily do so by modifying the 'ToCompleted' pipeline and hook the tasks up another place for example the 'Checkout' pipeline. If you're interested in how to achieve that: please read how to use partial components and how to register custom componets
