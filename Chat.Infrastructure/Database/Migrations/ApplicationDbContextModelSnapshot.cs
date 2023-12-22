﻿// <auto-generated />
using System;
using Chat.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Chat.Infrastructure.Database.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Chat.Application.Entities.ChatEntities.Chat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("ChatType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("LastMessageId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("LastMessageId");

                    b.ToTable("Chats");

                    b.HasDiscriminator<int>("ChatType");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Chat.Application.Entities.ChatEntities.ChatMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ChatId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("ChatMessageId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte>("MessageType")
                        .HasColumnType("smallint");

                    b.Property<string>("NormalizedContent")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("PinnedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("PinnedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ReadAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("ReadCounter")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("ReceivedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("SourceMessageId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("SourceUserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("PinnedById");

                    b.HasIndex("SourceMessageId");

                    b.HasIndex("SourceUserId");

                    b.HasIndex("UserId");

                    b.HasIndex("ChatId", "ChatMessageId");

                    b.ToTable("ChatMessages", (string)null);
                });

            modelBuilder.Entity("Chat.Application.Entities.ChatEntities.UserChatJoin", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ChatId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("LastReadMessageSentAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("UserId", "ChatId");

                    b.HasIndex("ChatId");

                    b.ToTable("UserChatJoins", (string)null);
                });

            modelBuilder.Entity("Chat.Application.Entities.IdentityEntities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIdx");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("Chat.Application.Entities.IdentityEntities.RoleClaim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims", (string)null);
                });

            modelBuilder.Entity("Chat.Application.Entities.IdentityEntities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedUsername")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIdx");

                    b.HasIndex("NormalizedUsername")
                        .IsUnique()
                        .HasDatabaseName("UsernameIdx");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("Chat.Application.Entities.IdentityEntities.UserClaim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims", (string)null);
                });

            modelBuilder.Entity("Chat.Application.Entities.IdentityEntities.UserRole", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles", (string)null);
                });

            modelBuilder.Entity("Chat.Application.Entities.UserProfileEntities.UserProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("AvatarPhotoId")
                        .HasColumnType("text");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte>("Color")
                        .HasColumnType("smallint");

                    b.Property<int>("Country")
                        .HasColumnType("integer");

                    b.Property<int>("Gender")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserProfiles", (string)null);
                });

            modelBuilder.Entity("Chat.Application.Entities.ChatEntities.GroupChat", b =>
                {
                    b.HasBaseType("Chat.Application.Entities.ChatEntities.Chat");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("Chat.Application.Entities.ChatEntities.UserChat", b =>
                {
                    b.HasBaseType("Chat.Application.Entities.ChatEntities.Chat");

                    b.Property<Guid>("UserFirstId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserSecondId")
                        .HasColumnType("uuid");

                    b.HasIndex("UserFirstId")
                        .IsUnique();

                    b.HasIndex("UserSecondId")
                        .IsUnique();

                    b.HasIndex("UserFirstId", "UserSecondId")
                        .IsUnique();

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("Chat.Application.Entities.ChatEntities.Chat", b =>
                {
                    b.HasOne("Chat.Application.Entities.ChatEntities.ChatMessage", "LastMessage")
                        .WithMany()
                        .HasForeignKey("LastMessageId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("LastMessage");
                });

            modelBuilder.Entity("Chat.Application.Entities.ChatEntities.ChatMessage", b =>
                {
                    b.HasOne("Chat.Application.Entities.ChatEntities.Chat", "Chat")
                        .WithMany("ChatMessages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chat.Application.Entities.IdentityEntities.User", "PinnedBy")
                        .WithMany()
                        .HasForeignKey("PinnedById");

                    b.HasOne("Chat.Application.Entities.ChatEntities.ChatMessage", "SourceMessage")
                        .WithMany()
                        .HasForeignKey("SourceMessageId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Chat.Application.Entities.IdentityEntities.User", "SourceUser")
                        .WithMany()
                        .HasForeignKey("SourceUserId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Chat.Application.Entities.IdentityEntities.User", "User")
                        .WithMany("ChatMessages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("PinnedBy");

                    b.Navigation("SourceMessage");

                    b.Navigation("SourceUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Chat.Application.Entities.ChatEntities.UserChatJoin", b =>
                {
                    b.HasOne("Chat.Application.Entities.ChatEntities.Chat", "Chat")
                        .WithMany("UserChatJoins")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chat.Application.Entities.IdentityEntities.User", "User")
                        .WithMany("UserChatJoins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Chat.Application.Entities.IdentityEntities.RoleClaim", b =>
                {
                    b.HasOne("Chat.Application.Entities.IdentityEntities.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Chat.Application.Entities.IdentityEntities.UserClaim", b =>
                {
                    b.HasOne("Chat.Application.Entities.IdentityEntities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Chat.Application.Entities.IdentityEntities.UserRole", b =>
                {
                    b.HasOne("Chat.Application.Entities.IdentityEntities.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Chat.Application.Entities.UserProfileEntities.UserProfile", b =>
                {
                    b.HasOne("Chat.Application.Entities.IdentityEntities.User", "User")
                        .WithOne("UserProfile")
                        .HasForeignKey("Chat.Application.Entities.UserProfileEntities.UserProfile", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Chat.Application.Entities.ChatEntities.UserChat", b =>
                {
                    b.HasOne("Chat.Application.Entities.IdentityEntities.User", "UserFirst")
                        .WithOne()
                        .HasForeignKey("Chat.Application.Entities.ChatEntities.UserChat", "UserFirstId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chat.Application.Entities.IdentityEntities.User", "UserSecond")
                        .WithOne()
                        .HasForeignKey("Chat.Application.Entities.ChatEntities.UserChat", "UserSecondId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserFirst");

                    b.Navigation("UserSecond");
                });

            modelBuilder.Entity("Chat.Application.Entities.ChatEntities.Chat", b =>
                {
                    b.Navigation("ChatMessages");

                    b.Navigation("UserChatJoins");
                });

            modelBuilder.Entity("Chat.Application.Entities.IdentityEntities.User", b =>
                {
                    b.Navigation("ChatMessages");

                    b.Navigation("UserChatJoins");

                    b.Navigation("UserProfile");
                });
#pragma warning restore 612, 618
        }
    }
}
