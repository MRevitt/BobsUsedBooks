using Bookstore.Domain.Addresses;
using Bookstore.Domain.Books;
using Bookstore.Domain.Carts;
using Bookstore.Domain.Customers;
using Bookstore.Domain.Offers;
using Bookstore.Domain.Orders;
using Bookstore.Domain.ReferenceData;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System;

namespace Bookstore.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        static ApplicationDbContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Address> Address { get; set; }

        public DbSet<Book> Book { get; set; }

        public DbSet<Customer> Customer { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<ShoppingCart> ShoppingCart { get; set; }

        public DbSet<ShoppingCartItem> ShoppingCartItem { get; set; }

        public DbSet<OrderItem> OrderItem { get; set; }

        public DbSet<Offer> Offer { get; set; }

        public DbSet<ReferenceDataItem> ReferenceData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply table and column mappings for Address entity
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("address", "bobsusedbookstore_dbo");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AddressLine1).HasColumnName("addressline1");
                entity.Property(e => e.AddressLine2).HasColumnName("addressline2");
                entity.Property(e => e.City).HasColumnName("city");
                entity.Property(e => e.State).HasColumnName("state");
                entity.Property(e => e.Country).HasColumnName("country");
                entity.Property(e => e.ZipCode).HasColumnName("zipcode");
                entity.Property(e => e.CustomerId).HasColumnName("customerid");
                entity.Property(e => e.IsActive).HasColumnName("isactive").HasConversion<int>();
                entity.Property(e => e.CreatedBy).HasColumnName("createdby");
                entity.Property(e => e.CreatedOn).HasColumnName("createdon");
                entity.Property(e => e.UpdatedOn).HasColumnName("updatedon");
            });

            // Apply table and column mappings for Book entity
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("book", "bobsusedbookstore_dbo");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Author).HasColumnName("author");
                entity.Property(e => e.Year).HasColumnName("year");
                entity.Property(e => e.ISBN).HasColumnName("isbn");
                entity.Property(e => e.PublisherId).HasColumnName("publisherid");
                entity.Property(e => e.BookTypeId).HasColumnName("booktypeid");
                entity.Property(e => e.GenreId).HasColumnName("genreid");
                entity.Property(e => e.ConditionId).HasColumnName("conditionid");
                entity.Property(e => e.CoverImageUrl).HasColumnName("coverimageurl");
                entity.Property(e => e.Summary).HasColumnName("summary");
                entity.Property(e => e.Price).HasColumnName("price").HasPrecision(18, 2);
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.CreatedBy).HasColumnName("createdby");
                entity.Property(e => e.CreatedOn).HasColumnName("createdon");
                entity.Property(e => e.UpdatedOn).HasColumnName("updatedon");
                // Computed properties IsInStock and IsLowInStock are excluded from mappings
            });

            // Apply table and column mappings for Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer", "bobsusedbookstore_dbo");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Sub).HasColumnName("sub");
                entity.Property(e => e.Username).HasColumnName("username");
                entity.Property(e => e.FirstName).HasColumnName("firstname");
                entity.Property(e => e.LastName).HasColumnName("lastname");
                // Computed property FullName is excluded from mappings
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.DateOfBirth).HasColumnName("dateofbirth");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.CreatedBy).HasColumnName("createdby");
                entity.Property(e => e.CreatedOn).HasColumnName("createdon");
                entity.Property(e => e.UpdatedOn).HasColumnName("updatedon");
                entity.HasIndex(x => x.Sub).IsUnique();
            });

            // Apply table and column mappings for Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order", "bobsusedbookstore_dbo");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CustomerId).HasColumnName("customerid");
                entity.Property(e => e.AddressId).HasColumnName("addressid");
                entity.Property(e => e.DeliveryDate).HasColumnName("deliverydate");
                entity.Property(e => e.OrderStatus).HasColumnName("orderstatus");
                // Computed properties Tax, SubTotal, Total are excluded from mappings
                entity.Property(e => e.CreatedBy).HasColumnName("createdby");
                entity.Property(e => e.CreatedOn).HasColumnName("createdon");
                entity.Property(e => e.UpdatedOn).HasColumnName("updatedon");
                entity.HasOne(x => x.Customer).WithMany().OnDelete(DeleteBehavior.Restrict);
            });

            // Apply table and column mappings for ShoppingCart entity
            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.ToTable("shoppingcart", "bobsusedbookstore_dbo");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CorrelationId).HasColumnName("correlationid");
                entity.Property(e => e.CreatedBy).HasColumnName("createdby");
                entity.Property(e => e.CreatedOn).HasColumnName("createdon");
                entity.Property(e => e.UpdatedOn).HasColumnName("updatedon");
            });

            // Apply table and column mappings for ShoppingCartItem entity
            modelBuilder.Entity<ShoppingCartItem>(entity =>
            {
                entity.ToTable("shoppingcartitem", "bobsusedbookstore_dbo");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ShoppingCartId).HasColumnName("shoppingcartid");
                entity.Property(e => e.BookId).HasColumnName("bookid");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.WantToBuy).HasColumnName("wanttobuy").HasConversion<int>();
                entity.Property(e => e.CreatedBy).HasColumnName("createdby");
                entity.Property(e => e.CreatedOn).HasColumnName("createdon");
                entity.Property(e => e.UpdatedOn).HasColumnName("updatedon");
            });

            // Apply table and column mappings for OrderItem entity
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("orderitem", "bobsusedbookstore_dbo");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.OrderId).HasColumnName("orderid");
                entity.Property(e => e.BookId).HasColumnName("bookid");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.CreatedBy).HasColumnName("createdby");
                entity.Property(e => e.CreatedOn).HasColumnName("createdon");
                entity.Property(e => e.UpdatedOn).HasColumnName("updatedon");
            });

            // Apply table and column mappings for Offer entity
            modelBuilder.Entity<Offer>(entity =>
            {
                entity.ToTable("offer", "bobsusedbookstore_dbo");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Author).HasColumnName("author");
                entity.Property(e => e.ISBN).HasColumnName("isbn");
                entity.Property(e => e.BookName).HasColumnName("bookname");
                entity.Property(e => e.FrontUrl).HasColumnName("fronturl");
                entity.Property(e => e.GenreId).HasColumnName("genreid");
                entity.Property(e => e.ConditionId).HasColumnName("conditionid");
                entity.Property(e => e.PublisherId).HasColumnName("publisherid");
                entity.Property(e => e.BookTypeId).HasColumnName("booktypeid");
                entity.Property(e => e.Summary).HasColumnName("summary");
                entity.Property(e => e.OfferStatus).HasColumnName("offerstatus");
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.CustomerId).HasColumnName("customerid");
                entity.Property(e => e.BookPrice).HasColumnName("bookprice").HasPrecision(18, 2);
                entity.Property(e => e.CreatedBy).HasColumnName("createdby");
                entity.Property(e => e.CreatedOn).HasColumnName("createdon");
                entity.Property(e => e.UpdatedOn).HasColumnName("updatedon");
            });

            // Apply table and column mappings for ReferenceDataItem entity
            modelBuilder.Entity<ReferenceDataItem>(entity =>
            {
                entity.ToTable("referencedata", "bobsusedbookstore_dbo");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DataType).HasColumnName("datatype");
                entity.Property(e => e.Text).HasColumnName("text");
                entity.Property(e => e.CreatedBy).HasColumnName("createdby");
                entity.Property(e => e.CreatedOn).HasColumnName("createdon");
                entity.Property(e => e.UpdatedOn).HasColumnName("updatedon");
            });

            // Preserve existing relationship configurations
            modelBuilder.Entity<Book>().HasOne(x => x.Publisher).WithMany().HasForeignKey(x => x.PublisherId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Book>().HasOne(x => x.BookType).WithMany().HasForeignKey(x => x.BookTypeId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Book>().HasOne(x => x.Genre).WithMany().HasForeignKey(x => x.GenreId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Book>().HasOne(x => x.Condition).WithMany().HasForeignKey(x => x.ConditionId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Offer>().HasOne(x => x.Publisher).WithMany().HasForeignKey(x => x.PublisherId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Offer>().HasOne(x => x.BookType).WithMany().HasForeignKey(x => x.BookTypeId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Offer>().HasOne(x => x.Genre).WithMany().HasForeignKey(x => x.GenreId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Offer>().HasOne(x => x.Condition).WithMany().HasForeignKey(x => x.ConditionId).OnDelete(DeleteBehavior.Restrict);

            PopulateDatabase(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
    }
}
