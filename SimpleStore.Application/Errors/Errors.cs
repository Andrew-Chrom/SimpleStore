using Microsoft.AspNetCore.Identity;
using Spectre.Console;

namespace SimpleStore.Application.Errors
{
    public enum ErrorType { BadRequest=400, Unauthorized=401, NotFound = 404, Conflict =409, Validation = 422 }

    public record DomainError(string Id, ErrorType Type, string Description);

    public static class DomainErrors
    {
        public static class Category
        {
            public static readonly DomainError NotFound = new("Category.NotFound", ErrorType.NotFound, "Category not found");
            public static readonly DomainError Validation = new("Category.Validation", ErrorType.Validation, "Category validation failed");

        }

        public static class Product
        {
            public static readonly DomainError NotFound = new("Product.NotFound", ErrorType.NotFound, "Product not found");
            public static readonly DomainError Validation = new("Product.Validation", ErrorType.Validation, "Product validation failed");
        }

        public static class Cart
        {
            public static readonly DomainError NotFound = new("CartItem.NotFound", ErrorType.NotFound, "CartItem item not found");
            public static readonly DomainError Conflict = new("CartItem.Conflict", ErrorType.Conflict, "CartItem conflict ");
        }
        public static class Order
        {
            public static readonly DomainError NotFound = new("Order.NotFound", ErrorType.NotFound, "Order item not found");
            public static readonly DomainError Conflict = new("Order.Conflict", ErrorType.Conflict, "Order conflict ");
        }
        public static class Wishlist
        {
            public static readonly DomainError NotFound = new("Wishlist.NotFound", ErrorType.NotFound, "Wishlist item not found");
            public static readonly DomainError Conflict = new("Wishlist.Conflict", ErrorType.Conflict, "Wishlist conflict ");
        }

        public static class Payment
        {
            public static readonly DomainError InvalidWebhook = new("Payment.InvalidWebhook", ErrorType.BadRequest, "Invalid Stripe webhook");
            public static readonly DomainError PaymentFailed = new("Payment.InvalidWebhook", ErrorType.BadRequest, "Invalid Stripe webhook");

        }
        public static class Authentication
        {
            public static readonly DomainError EmailExists = new("Authentication.EmailExists", ErrorType.Conflict, "Email already exists");
            public static readonly DomainError Unauthorized = new("Authentication.Unauthorized", ErrorType.Unauthorized, "Unauthorized access");
            public static readonly DomainError NotFound = new("Authentication.NotFound", ErrorType.NotFound, "Authentication item not found");

            public static DomainError IdentityError(IEnumerable<IdentityError> errors)
            {
                var message = string.Join(", ", errors.Select(e => e.Description));
                return new DomainError("Auth.IdentityError", ErrorType.BadRequest, message);
            }

        }
    }
}
