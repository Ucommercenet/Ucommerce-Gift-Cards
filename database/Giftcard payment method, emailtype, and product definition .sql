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

INSERT [dbo].[uCommerce_EmailType] ([Name], [Description], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [Deleted]) 
VALUES (N'GiftCard', N'E-mail which will be sent to the customer with the gift card code after order is completed.', CAST(0x00009C7500DD1FB9 AS DateTime), N'uCommerce', CAST(0x00009C7500DD1FBA AS DateTime), N'uCommerce', 0)

INSERT [dbo].[uCommerce_ProductDefinition] ([Name], [Description], [Deleted], [SortOrder])
VALUES (N'GiftCard', N'Definition for handling uCommerce Gift cards',0,0)

INSERT [dbo].[uCommerce_PaymentMethod] ([Name], [FeePercent], [ImageMediaId], [PaymentMethodServiceName], [Enabled], [Deleted], [ModifiedOn], [ModifiedBy], [Pipeline])
VALUES (N'Gift card', 0, null, N'Gift Card', 1, 0, CAST(0x00009C7500DD1FBA AS DateTime), N'uCommerce', N'Checkout')


