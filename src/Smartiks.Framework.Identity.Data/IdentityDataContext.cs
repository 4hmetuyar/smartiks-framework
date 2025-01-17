﻿using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Smartiks.Framework.Identity.Data.Abstractions;
using System;
using System.Threading.Tasks;

namespace Smartiks.Framework.Identity.Data
{
    public class IdentityDataContext<TId, TUser, TRole> : IdentityDbContext<TUser, TRole, TId>, IConfigurationDbContext, IPersistedGrantDbContext
        where TId : IEquatable<TId>
        where TUser : User<TId>
        where TRole : Role<TId>
    {
        public IdentityDataContext(DbContextOptions options) : base(options)
        {
        }

        #region DbSets

        public DbSet<ApiResource> ApiResources { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

        public DbSet<IdentityResource> IdentityResources { get; set; }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Identity

            modelBuilder.Entity<TUser>(b => { b.ToTable("Users"); });

            modelBuilder.Entity<TRole>(b => { b.ToTable("Roles"); });

            modelBuilder.Entity<IdentityUserRole<TId>>(b => { b.ToTable("UserRoles"); });

            modelBuilder.Entity<IdentityUserClaim<TId>>(b => { b.ToTable("UserClaims"); });

            modelBuilder.Entity<IdentityUserLogin<TId>>(b => { b.ToTable("UserLogins"); });

            modelBuilder.Entity<IdentityUserToken<TId>>(b => { b.ToTable("UserTokens"); });

            modelBuilder.Entity<IdentityRoleClaim<TId>>(b => { b.ToTable("RoleClaims"); });

            #endregion

            #region IdentityServer4

            modelBuilder.Entity<Client>(client =>
            {
                client.ToTable("Clients");

                client.HasKey(x => x.Id);

                client.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
                client.Property(x => x.ProtocolType).HasMaxLength(200).IsRequired();
                client.Property(x => x.ClientName).HasMaxLength(200);
                client.Property(x => x.ClientUri).HasMaxLength(2000);
                client.Property(x => x.LogoUri).HasMaxLength(2000);
                client.Property(x => x.Description).HasMaxLength(1000);
                client.Property(x => x.FrontChannelLogoutUri).HasMaxLength(2000);
                client.Property(x => x.BackChannelLogoutUri).HasMaxLength(2000);
                client.Property(x => x.ClientClaimsPrefix).HasMaxLength(200);
                client.Property(x => x.PairWiseSubjectSalt).HasMaxLength(200);
                client.Property(x => x.UserCodeType).HasMaxLength(100);

                client.HasIndex(x => x.ClientId).IsUnique();

                client.HasMany(x => x.AllowedGrantTypes).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.RedirectUris).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.PostLogoutRedirectUris).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.AllowedScopes).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.ClientSecrets).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.Claims).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.IdentityProviderRestrictions).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.AllowedCorsOrigins).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.Properties).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ClientGrantType>(grantType =>
            {
                grantType.ToTable("ClientGrantTypes");

                grantType.Property(x => x.GrantType).HasMaxLength(250).IsRequired();
            });

            modelBuilder.Entity<ClientRedirectUri>(redirectUri =>
            {
                redirectUri.ToTable("ClientRedirectUris");

                redirectUri.Property(x => x.RedirectUri).HasMaxLength(2000).IsRequired();
            });

            modelBuilder.Entity<ClientPostLogoutRedirectUri>(postLogoutRedirectUri =>
            {
                postLogoutRedirectUri.ToTable("ClientPostLogoutRedirectUris");

                postLogoutRedirectUri.Property(x => x.PostLogoutRedirectUri).HasMaxLength(2000).IsRequired();
            });

            modelBuilder.Entity<ClientScope>(scope =>
            {
                scope.ToTable("ClientScopes");

                scope.Property(x => x.Scope).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<ClientSecret>(secret =>
            {
                secret.ToTable("ClientSecrets");

                secret.Property(x => x.Value).HasMaxLength(4000).IsRequired();
                secret.Property(x => x.Type).HasMaxLength(250).IsRequired();
                secret.Property(x => x.Description).HasMaxLength(2000);
            });

            modelBuilder.Entity<ClientClaim>(claim =>
            {
                claim.ToTable("ClientClaims");

                claim.Property(x => x.Type).HasMaxLength(250).IsRequired();
                claim.Property(x => x.Value).HasMaxLength(250).IsRequired();
            });

            modelBuilder.Entity<ClientIdPRestriction>(idPRestriction =>
            {
                idPRestriction.ToTable("ClientIdPRestrictions");

                idPRestriction.Property(x => x.Provider).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<ClientCorsOrigin>(corsOrigin =>
            {
                corsOrigin.ToTable("ClientCorsOrigins");

                corsOrigin.Property(x => x.Origin).HasMaxLength(150).IsRequired();
            });

            modelBuilder.Entity<ClientProperty>(property =>
            {
                property.ToTable("ClientProperties");

                property.Property(x => x.Key).HasMaxLength(250).IsRequired();
                property.Property(x => x.Value).HasMaxLength(2000).IsRequired();
            });

            modelBuilder.Entity<PersistedGrant>(grant =>
            {
                grant.ToTable("PersistedGrants");

                grant.Property(x => x.Key).HasMaxLength(200).ValueGeneratedNever();
                grant.Property(x => x.Type).HasMaxLength(50).IsRequired();
                grant.Property(x => x.SubjectId).HasMaxLength(200);
                grant.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
                grant.Property(x => x.CreationTime).IsRequired();
                // 50000 chosen to be explicit to allow enough size to avoid truncation, yet stay beneath the MySql row size limit of ~65K
                // apparently anything over 4K converts to nvarchar(max) on SqlServer
                grant.Property(x => x.Data).HasMaxLength(50000).IsRequired();

                grant.HasKey(x => x.Key);

                grant.HasIndex(x => new { x.SubjectId, x.ClientId, x.Type });
            });

            modelBuilder.Entity<DeviceFlowCodes>(codes =>
            {
                codes.ToTable("DeviceFlowCodes");

                codes.Property(x => x.DeviceCode).HasMaxLength(200).IsRequired();
                codes.Property(x => x.UserCode).HasMaxLength(200).IsRequired();
                codes.Property(x => x.SubjectId).HasMaxLength(200);
                codes.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
                codes.Property(x => x.CreationTime).IsRequired();
                codes.Property(x => x.Expiration).IsRequired();
                // 50000 chosen to be explicit to allow enough size to avoid truncation, yet stay beneath the MySql row size limit of ~65K
                // apparently anything over 4K converts to nvarchar(max) on SqlServer
                codes.Property(x => x.Data).HasMaxLength(50000).IsRequired();

                codes.HasKey(x => new { x.UserCode });

                codes.HasIndex(x => x.DeviceCode).IsUnique();
            });

            modelBuilder.Entity<IdentityResource>(identityResource =>
            {
                identityResource.ToTable("IdentityResources");

                identityResource.Property(x => x.Name).HasMaxLength(200).IsRequired();
                identityResource.Property(x => x.DisplayName).HasMaxLength(200);
                identityResource.Property(x => x.Description).HasMaxLength(1000);

                identityResource.HasIndex(x => x.Name).IsUnique();

                identityResource.HasMany(x => x.UserClaims).WithOne(x => x.IdentityResource).HasForeignKey(x => x.IdentityResourceId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                identityResource.HasMany(x => x.Properties).WithOne(x => x.IdentityResource).HasForeignKey(x => x.IdentityResourceId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<IdentityClaim>(claim =>
            {
                claim.ToTable("IdentityClaims");

                claim.Property(x => x.Type).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<IdentityResourceProperty>(property =>
            {
                property.ToTable("IdentityResourceProperties");

                property.Property(x => x.Key).HasMaxLength(250).IsRequired();
                property.Property(x => x.Value).HasMaxLength(2000).IsRequired();
            });

            modelBuilder.Entity<ApiResource>(apiResource =>
            {
                apiResource.ToTable("ApiResources");

                apiResource.Property(x => x.Name).HasMaxLength(200).IsRequired();
                apiResource.Property(x => x.DisplayName).HasMaxLength(200);
                apiResource.Property(x => x.Description).HasMaxLength(1000);

                apiResource.HasIndex(x => x.Name).IsUnique();

                apiResource.HasMany(x => x.Secrets).WithOne(x => x.ApiResource).HasForeignKey(x => x.ApiResourceId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                apiResource.HasMany(x => x.Scopes).WithOne(x => x.ApiResource).HasForeignKey(x => x.ApiResourceId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                apiResource.HasMany(x => x.UserClaims).WithOne(x => x.ApiResource).HasForeignKey(x => x.ApiResourceId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                apiResource.HasMany(x => x.Properties).WithOne(x => x.ApiResource).HasForeignKey(x => x.ApiResourceId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ApiSecret>(apiSecret =>
            {
                apiSecret.ToTable("ApiSecrets");

                apiSecret.Property(x => x.Description).HasMaxLength(1000);
                apiSecret.Property(x => x.Value).HasMaxLength(4000).IsRequired();
                apiSecret.Property(x => x.Type).HasMaxLength(250).IsRequired();
            });

            modelBuilder.Entity<ApiResourceClaim>(apiClaim =>
            {
                apiClaim.ToTable("ApiResourceClaims");

                apiClaim.Property(x => x.Type).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<ApiScope>(apiScope =>
            {
                apiScope.ToTable("ApiScopes");

                apiScope.Property(x => x.Name).HasMaxLength(200).IsRequired();
                apiScope.Property(x => x.DisplayName).HasMaxLength(200);
                apiScope.Property(x => x.Description).HasMaxLength(1000);

                apiScope.HasIndex(x => x.Name).IsUnique();

                apiScope.HasMany(x => x.UserClaims).WithOne(x => x.ApiScope).HasForeignKey(x => x.ApiScopeId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ApiScopeClaim>(apiScopeClaim =>
            {
                apiScopeClaim.ToTable("ApiScopeClaims");

                apiScopeClaim.Property(x => x.Type).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<ApiResourceProperty>(property =>
            {
                property.ToTable("ApiResourceProperties");

                property.Property(x => x.Key).HasMaxLength(250).IsRequired();
                property.Property(x => x.Value).HasMaxLength(2000).IsRequired();
            });

            #endregion
        }

        public virtual Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}