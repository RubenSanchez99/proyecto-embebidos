using FluentValidation;
using Basket.API.Model;

namespace Basket.API.Validators
{
    public class CustomerBasketValidator : AbstractValidator<CustomerBasket>
    {
        public CustomerBasketValidator()
        {
            RuleForEach(x => x.Items)
                .Must(x => x.Quantity > 0)
                .WithMessage("Invalid number of units");
        }
    }
}