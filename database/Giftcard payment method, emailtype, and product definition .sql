-- script for creating the EditGiftCardPrices tab on EditProduct page in uCommerce
INSERT INTO uCommerce_AdminTab
(
	VirtualPath, 
	AdminPageId, 
	SortOrder,
	MultiLingual,
	ResouceKey,
	HasSaveButton,
	HasDeleteButton,
	[Enabled]
)
VALUES
(
	'EditGiftCardPrices.ascx',
	1,
	2,
	0,
	'GiftCardPrices',
	1,
	0,
	1
)

INSERT INTO uCommerce_AdminTab
(
	VirtualPath, 
	AdminPageId, 
	SortOrder,
	MultiLingual,
	ResouceKey,
	HasSaveButton,
	HasDeleteButton,
	[Enabled]
)
VALUES
(
	'EditPaymentMethodGiftCards.ascx',
	9,
	4,
	0,
	'GiftCards',
	1,
	0,
	1
)

-- Enable save button on Edit Shipping tab.
UPDATE uCommerce_adminTab SET HasSaveButton = 1 WHERE VirtualPath = 'EditOrderShipments.ascx'