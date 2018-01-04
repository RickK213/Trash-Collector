namespace TrashCollector.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        StreetOne = c.String(),
                        StreetTwo = c.String(),
                        CityId = c.Int(nullable: false),
                        StateId = c.Int(nullable: false),
                        ZipCodeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.Cities", t => t.CityId, cascadeDelete: true)
                .ForeignKey("dbo.States", t => t.StateId, cascadeDelete: true)
                .ForeignKey("dbo.ZipCodes", t => t.ZipCodeId, cascadeDelete: true)
                .Index(t => t.CityId)
                .Index(t => t.StateId)
                .Index(t => t.ZipCodeId);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CityId);
            
            CreateTable(
                "dbo.States",
                c => new
                    {
                        StateId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.StateId);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        UserProfileId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.UserProfileId);
            
            CreateTable(
                "dbo.TrashCollections",
                c => new
                    {
                        TrashCollectionId = c.Int(nullable: false, identity: true),
                        PickUpDay = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        VacationStartDate = c.DateTime(nullable: false),
                        VacationEndDate = c.DateTime(nullable: false),
                        AddressId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TrashCollectionId)
                .ForeignKey("dbo.Addresses", t => t.AddressId, cascadeDelete: true)
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.ZipCodes",
                c => new
                    {
                        ZipCodeId = c.Int(nullable: false, identity: true),
                        Number = c.String(),
                        FourDigit = c.String(),
                    })
                .PrimaryKey(t => t.ZipCodeId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserProfileId = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId, cascadeDelete: true)
                .Index(t => t.UserProfileId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserProfileAddresses",
                c => new
                    {
                        UserProfile_UserProfileId = c.Int(nullable: false),
                        Address_AddressId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserProfile_UserProfileId, t.Address_AddressId })
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_UserProfileId, cascadeDelete: true)
                .ForeignKey("dbo.Addresses", t => t.Address_AddressId, cascadeDelete: true)
                .Index(t => t.UserProfile_UserProfileId)
                .Index(t => t.Address_AddressId);
            
            CreateTable(
                "dbo.TrashCollectionUserProfiles",
                c => new
                    {
                        TrashCollection_TrashCollectionId = c.Int(nullable: false),
                        UserProfile_UserProfileId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrashCollection_TrashCollectionId, t.UserProfile_UserProfileId })
                .ForeignKey("dbo.TrashCollections", t => t.TrashCollection_TrashCollectionId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_UserProfileId, cascadeDelete: true)
                .Index(t => t.TrashCollection_TrashCollectionId)
                .Index(t => t.UserProfile_UserProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Addresses", "ZipCodeId", "dbo.ZipCodes");
            DropForeignKey("dbo.TrashCollectionUserProfiles", "UserProfile_UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.TrashCollectionUserProfiles", "TrashCollection_TrashCollectionId", "dbo.TrashCollections");
            DropForeignKey("dbo.TrashCollections", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.UserProfileAddresses", "Address_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.UserProfileAddresses", "UserProfile_UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.Addresses", "StateId", "dbo.States");
            DropForeignKey("dbo.Addresses", "CityId", "dbo.Cities");
            DropIndex("dbo.TrashCollectionUserProfiles", new[] { "UserProfile_UserProfileId" });
            DropIndex("dbo.TrashCollectionUserProfiles", new[] { "TrashCollection_TrashCollectionId" });
            DropIndex("dbo.UserProfileAddresses", new[] { "Address_AddressId" });
            DropIndex("dbo.UserProfileAddresses", new[] { "UserProfile_UserProfileId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "UserProfileId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.TrashCollections", new[] { "AddressId" });
            DropIndex("dbo.Addresses", new[] { "ZipCodeId" });
            DropIndex("dbo.Addresses", new[] { "StateId" });
            DropIndex("dbo.Addresses", new[] { "CityId" });
            DropTable("dbo.TrashCollectionUserProfiles");
            DropTable("dbo.UserProfileAddresses");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ZipCodes");
            DropTable("dbo.TrashCollections");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.States");
            DropTable("dbo.Cities");
            DropTable("dbo.Addresses");
        }
    }
}
