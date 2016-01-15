-- Table for handeling GiftCards in uCommerce
CREATE TABLE uCommerce_GiftCard 
(
	GiftCardId int identity primary key,
	PaymentMethodId int foreign key references uCommerce_PaymentMethod(PaymentMethodId) not null,
	CurrencyId int foreign key references uCommerce_Currency(CurrencyId) not null,
	Amount money not null,
	AmountUsed money not null default 0,
	Enabled bit not null default 1,
	CreatedOn DateTime not null,
	CreatedBy nvarchar(50) not null,
	ModifiedBy nvarchar(50),
	ModifiedOn DateTime not null default GETDATE(),
	ExpiresOn DateTime not null,
	Code nvarchar(512) not null,
	OrderNumber nvarchar(50) null,
	Note nvarchar(max) not null
)
-- Enable save button on Edit Shipping tab.
UPDATE uCommerce_adminTab SET HasSaveButton = 1 WHERE VirtualPath = 'EditOrderShipments.ascx'