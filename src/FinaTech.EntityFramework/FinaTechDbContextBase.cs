﻿using Microsoft.EntityFrameworkCore;
using FinaTech.Core;
using FinaTech.Core.Account;
using FinaTech.Core.Payment;

namespace FinaTech.EntityFramework;

/// <summary>
/// Represents the base DbContext for payment-related operations, providing access
/// to the database sets for Payments, Accounts, and Addresses, as well as handling
/// the entity configuration through the OnModelCreating method.
/// </summary>
/// <typeparam name="T">The type of the DbContext inheriting from FinaTechDbContextBase.</typeparam>
public abstract class FinaTechDbContextBase<T>(DbContextOptions<T> options) : DbContext(options)
  where T : DbContext
{

  /// <summary>
  /// Gets or sets the database set for payment transactions, allowing CRUD operations
  /// on the <see cref="Payment"/> entities stored in the database.
  /// </summary>
  public DbSet<Payment> Payments { get; set; }

  /// <summary>
  /// Gets or sets the database set for accounts, enabling CRUD operations
  /// on the <see cref="Account"/> entities stored in the database.
  /// </summary>
  public DbSet<Account> Accounts { get; set; }

  /// <summary>
  /// Gets or sets the database set for address entities, enabling CRUD operations
  /// on the <see cref="Address"/> entities stored in the database.
  /// </summary>
  public DbSet<Address> Addresses { get; set; }

  /// <summary>
  /// Configures the model and relationships for the database context during the creation of the model.
  /// </summary>
  /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    CreateAddressEntity(modelBuilder);
    CreateAccountEntity(modelBuilder);
    CreatePaymentEntity(modelBuilder);
  }

  /// <summary>
  /// Configures the entity mappings and constraints for the Account entity within the database model.
  /// </summary>
  /// <param name="modelBuilder">The builder used to define the model and its relationships.</param>
  private void CreateAccountEntity(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Account>(account =>
    {
      account.ToTable("Accounts");
      account.HasKey(a => a.Id);
      account.HasIndex(a => a.Name);
      account.HasIndex(a => a.AccountNumber);
      account.HasIndex(a => a.Iban);
      account.HasIndex(a => a.Bic);

      account.HasOne(a => a.Address)
        .WithMany()
        .HasForeignKey(a=>a.AddressId);
    });
  }

  /// <summary>
  /// Configures the Address entity and its relationships during the model creation process.
  /// </summary>
  /// <param name="modelBuilder">The builder used to construct the model and configure the Address entity.</param>
  private void CreateAddressEntity(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Address>(account =>
    {
      account.ToTable("Address");
      account.HasKey(a => a.Id);
      account.HasIndex(a => a.AddressLine1);
      account.HasIndex(a => a.CountryCode);
      account.HasIndex(a => a.PostCode);
      account.HasIndex(a => a.City);
    });
  }

  /// <summary>
  /// Configures the entity settings for the Payment table, including relationships,
  /// key definitions, indices, and owned properties.
  /// </summary>
  /// <param name="modelBuilder">The builder used to define the model for the Payment entity.</param>
  private void CreatePaymentEntity(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Payment>(payment =>
    {
      payment.ToTable("Payments");
      payment.HasKey(p => p.Id);
      payment.HasIndex(p => p.ReferenceNumber);
      payment.Property(p => p.Details);
      payment.OwnsOne(p => p.Amount, amount =>
      {
        amount.Property(a => a.Value)
          .IsRequired();
        amount.Property(a => a.Currency)
          .HasMaxLength(PaymentConstants.MaxLengthOfCurrency)
          .IsRequired();
      });

      payment.HasOne(p => p.OriginatorAccount)
        .WithMany()
        .HasForeignKey(p=>p.OriginatorAccountId);

      payment.HasOne(p => p.BeneficiaryAccount)
        .WithMany()
        .HasForeignKey(p=>p.BeneficiaryAccountId);

    });
  }
}
