using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Presentation.Views.Catalog;
using UCommerce.Presentation.Web.Controls;
using UCommerce.Presentation.Web.Pages;
namespace UCommerce.Transactions.Payments.Giftcard.UI
{
	public partial class EditGiftCardPrices : ViewEnabledControl<IEditProductView>, ISection
	{
		private readonly IRepository<PriceGroup> _priceGroupRepository;

		public EditGiftCardPrices()
		{
			_priceGroupRepository = ObjectFactory.Instance.Resolve<IRepository<PriceGroup>>();
		}

		private IList<Product> _variants;
		protected IList<Product> Variants
		{
			get
			{
				if (_variants == null)
				{
					_variants = new List<Product>(View.Product.Variants);

					// Add placeholder variants to allow bulk creation of variants.
					AddEmptyVariants(_variants);
				}
				return _variants;
			}
			set
			{
				_variants = value;
			}
		}

		private void InitializeGridView()
		{
			var pricegroups = _priceGroupRepository.Select().ToList();

			foreach (var priceGroup in pricegroups)
			{
				var field = new TemplateField
				{
					HeaderText = priceGroup.Name
				};

				GiftCardPrices.Columns.Add(field);
			}
		}

		/// <summary>
		/// Creates 10 empty variant to allow bulk creation of products variants.
		/// </summary>
		/// <param name="existingVariants"></param>
		private void AddEmptyVariants(IList<Product> existingVariants)
		{
			for (int i = 0; i < 10; i++)
			{
				var product = GetEmptyVariant(i);
				existingVariants.Add(product);
			}
		}

		private Product GetEmptyVariant(int variantNumber)
		{
			return new Product
			{
				ParentProduct = View.Product,
				ProductDefinition = View.Product.ProductDefinition,
				VariantSku = variantNumber.ToString()
			};
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			View.Saving += View_Saving;
			View.Saved += View_Saved;
			GiftCardPrices.RowDataBound += GiftCardPrices_RowDataBound;
			GiftCardPrices.RowUpdating += GiftCardPrices_RowUpdating;
			GiftCardPrices.RowDeleting += GiftCardPrices_RowDeleting;

			InitGridView();

		}

		private void View_Saved(object sender, EntityCommandEventArgs<Product> e)
		{
			RefreshVariants();

			DataBind();
		}

		private void GiftCardPrices_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			var row = GiftCardPrices.Rows[e.RowIndex];
			var productVariantHiddenField = row.FindControl("ProductIdHiddenVariantField") as HiddenField;

			var productVariantId = Convert.ToInt32(productVariantHiddenField.Value);
			var currentProductVariant = View.Product.Variants.First(x => x.ProductId == productVariantId);
			if (currentProductVariant != null)
			{
				View.Product.Variants.Remove(currentProductVariant);
				currentProductVariant.Delete();
				RefreshVariants();
			}
		}

		private void RefreshVariants()
		{
			Variants = null; // forces reload of the variants lists also adding placeholder variants for bulk creation of variant prices.
			DataBind();
		}

		private void View_Saving(object sender, EntityCommandEventArgs<Product> e)
		{
			if (!Page.IsValid) return;

			GiftCardPrices.BulkUpdate();
		}

		private void InitGridView()
		{
			InitializeGridView();
			DataBind();
		}

		protected void GiftCardPrices_RowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			//we have to differentiate this row as it is a header row and contains no product or new product row
			if (e.RowIndex == -1) return;

			var row = GiftCardPrices.Rows[e.RowIndex];
			if (!ShouldSaveVariantRow(row)) return;

			var productVariantHiddenField = (row.FindControl("ProductIdHiddenVariantField") as HiddenField);

			var productVariantId = Convert.ToInt32(productVariantHiddenField.Value);
			SaveGiftCardPrices(row, productVariantId);
		}

		/// <summary>
		/// We can only differentiate new variants in the gridView by classname (which will be empty if not a new product).
		/// AddVariantHiddenField are used to find out weather to save or not.
		/// </summary>
		private bool ShouldSaveVariantRow(GridViewRow row)
		{
			if (row.Attributes["class"] == null)
				return true;

			var hidden = row.FindControl("AddVariantHiddenField") as HiddenField;

			return hidden.Value != "false";
		}

		private Product CreateVariantFor(Product parentProduct)
		{
			string variantSku = GenerateVariantSku(parentProduct);

			Product variant = new Product();
			variant.Sku = View.Product.Sku;
			variant.DisplayOnSite = true;
			variant.ParentProduct = View.Product;
			variant.Name = "giftCard" + variantSku;
			variant.VariantSku = variantSku;
			variant.ProductDefinition = ProductDefinition.SingleOrDefault(x => x.Name == Constants.GiftCardProductDefinition);
			parentProduct.Variants.Add(variant);

			return variant;
		}

		private void SaveGiftCardPrices(GridViewRow row, int productVariantId)
		{
			if (!ShouldSaveVariantRow(row)) return;

			Product currentProductVariant;

			// Empty variants will all have the same id (0) which will 
			// cause collapse in saving as it will only find the first 
			// product in the list.
			if (productVariantId == 0)
				currentProductVariant = CreateVariantFor(View.Product);
			else
				currentProductVariant = View.Product.Variants.SingleOrDefault(x => x.ProductId == productVariantId);

			var pricegroups = _priceGroupRepository.Select().ToList();

			foreach (var priceGroup in pricegroups)
			{
				var field = row.FindControl("NewPrice_" + priceGroup.Id) as TextBox;
				var price = Convert.ToDecimal(field.Text);

				if (price == 0) continue;

				var priceGroupPrice = currentProductVariant.PriceGroupPrices.FirstOrDefault(x => x.PriceGroup == priceGroup)
					?? new PriceGroupPrice { PriceGroup = priceGroup };

				priceGroupPrice.Price = price;

				// PriceGroupPrices set elimnates duplicates if price exists already
				currentProductVariant.AddPriceGroupPrice(priceGroupPrice);
			}
		}

		/// <summary>
		/// Generate a Sku based on the highest number in the VariantSku row
		/// </summary>
		private string GenerateVariantSku(Product parentProduct)
		{
			if (!parentProduct.Variants.Any()) return "1";

			int maxSku = parentProduct.Variants.Max(x =>
			{
				int value;
				int.TryParse(x.VariantSku, out value);
				return value;
			});

			int newSku = maxSku + 1;

			return newSku.ToString();
		}

		protected virtual Control GetInitializedControlForPriceGroupPrice(Product product, PriceGroup priceGroup, bool enabled)
		{
			TextBox textBox = new TextBox();
			textBox.Enabled = !enabled;
			textBox.Attributes.Add("class", "priceInput amountInput");
			textBox.ID = "NewPrice_" + priceGroup.Id;
			textBox.Text = 0.ToString("0.00");

			var price = product.PriceGroupPrices.FirstOrDefault(x => x.PriceGroup == priceGroup);
			if (price != null)
			{
				textBox.Text = price.Price.GetValueOrDefault().ToString("0.00");
			}

			return textBox;
		}

		void GiftCardPrices_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			// We have to differentiate this row as it is a header row and contains no product or new product row
			if (e.Row.RowIndex == -1)
				return;
			int productId = 0;
			var productIdHiddenField = e.Row.Cells[0].FindControl("ProductIdHiddenVariantField") as HiddenField;
			if (productIdHiddenField != null)
				productId = int.Parse(productIdHiddenField.Value);

			bool newVariant = (productId <= 0);

			int cellIndex = 1;
			Product currentVariant = Variants.FirstOrDefault(x => x.ProductId == productId);


			var pricegroups = _priceGroupRepository.Select().ToList();

			foreach (var priceGroup in pricegroups)
			{
				Control control = GetInitializedControlForPriceGroupPrice(currentVariant, priceGroup, newVariant);
				e.Row.Cells[cellIndex].Controls.Add(control);
				e.Row.Cells[cellIndex].Controls.Add(GetRequiredFieldValidator(control, priceGroup));
				e.Row.Cells[cellIndex].Controls.Add(GetRegularExpressionValidator(control, priceGroup));

				cellIndex++;
			}

			if (newVariant)
				e.Row.Attributes.Add("Class", "newVariantRow");
		}

		private Control GetRegularExpressionValidator(Control controlToValidateAgainst, PriceGroup priceGroup)
		{
			var regularExpressionValidator = new RegularExpressionValidator();
			regularExpressionValidator.ControlToValidate = controlToValidateAgainst.ID;
			regularExpressionValidator.Display = ValidatorDisplay.Static;
			regularExpressionValidator.ErrorMessage = string.Format("{0} {1}", priceGroup.Name, GetLocalResourceObject("requiredFieldValidator").ToString());
			regularExpressionValidator.ValidationExpression = @"^-?[0-9]+((\.|,)[0-9]{1,20})?$";
			regularExpressionValidator.ForeColor = Color.Red;
			regularExpressionValidator.Text = "*";

			return regularExpressionValidator;
		}

		private Control GetRequiredFieldValidator(Control controlToValidateAgainst, PriceGroup priceGroup)
		{
			var requiredFieldValidator = new RequiredFieldValidator();
			requiredFieldValidator.ControlToValidate = controlToValidateAgainst.ID;
			requiredFieldValidator.Display = ValidatorDisplay.Static;
			requiredFieldValidator.ErrorMessage = string.Format("{0} {1}", priceGroup.Name, GetLocalResourceObject("requiredFieldValidator").ToString());
			requiredFieldValidator.Text = "*";
			requiredFieldValidator.ForeColor = Color.Red;

			return requiredFieldValidator;
		}

		public IList<ICommand> GetCommands()
		{
			return new List<ICommand>()
				{
					new ClientImageCommand
						{
							Icon = Presentation.Resources.Images.Menu.Create,
							Text = GetLocalResourceObject("CreateGiftCard").ToString(),
							ClientCommand = "addNewVariantClick(); return false;"
						}
				};
		}

		public bool Show
		{
			get { return View.Product.ProductDefinition.Name == Constants.GiftCardProductDefinition; }
		}
	}
}