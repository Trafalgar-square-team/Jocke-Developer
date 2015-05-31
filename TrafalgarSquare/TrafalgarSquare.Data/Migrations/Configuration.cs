namespace TrafalgarSquare.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TrafalgarSquareDbContext>
    {
        private TrafalgarSquareDbContext _context;
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(TrafalgarSquareDbContext context)
        {
            _context = context;
            // Create Administrator Role
            if (!_context.Users.Any())
            {
                CreateRoles();
                var users = CreateUsers();
                var categories = CreateCategories();
                var posts = CreatePosts(users, categories);
                AddComments(posts, users);
                AddFriendships(users);
            }
        }

        private void AddFriendships(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                foreach (var user2 in users)
                {
                    if (user == user2)
                    {
                        continue;
                    }

                    if (RandomBoolean())
                    {
                        user.Friends.Add(new UserFriends
                        {
                            Friend = user2,
                            IsAccepted = true,
                            SentFriendRequestDate = DateTime.Now.AddDays(-RandomNumber(100)),
                        });
                    }
                }
            }
            _context.SaveChanges();
        }

        private void AddComments(List<Post> posts, IEnumerable<User> users)
        {
            foreach (var post in posts)
            {
                foreach (var user in users)
                {
                    post.Comments.Add(new Comment
                    {
                        User = user,
                        CreatedOn = DateTime.Now.AddDays(-RandomNumber(10)),
                        Post = post,
                        Text = "Hello, this is a comment! "
                    });
                    if (RandomBoolean()) { 
                        break;
                    }
                }
            }
        }

        private List<Post> CreatePosts(IEnumerable<User> usersList, IEnumerable<Category> categoriesList)
        {
            var posts = new List<Post>();
            foreach (var user in usersList)
            {
                foreach (var category in categoriesList)
                {
                    for (var i = 0; i < 10; i++)
                    {
                        var newPost = new Post
                        {
                            Title = "Post's title " + i,
                            Text = "Somebody said something.",
                            Resource = new PostResources()
                            {
                                PictureUrl = "http://gravatar.com/avatar/658f2039885a85cc03cc31e20919bed6?s=512"
                            },
                            PostOwner = user,
                            CreatedDateTime = DateTime.Now.AddDays(-i),
                            Category = category
                        };

                        user.Posts.Add(newPost);
                        posts.Add(newPost);
                    }
                }
            }

            _context.SaveChanges();

            return posts;
        }

        private IEnumerable<User> CreateUsers()
        {
            var adminsList = new List<User>
            {
                new User //admin
                {
                    PasswordHash = "AA/axaHG/kGY+eFHHm3PsdMebx/28f3vwsMoxj0oeTkJEvOyCoZcjEsFQrFm5UzhMA==", //123456
                    SecurityStamp = "79089894-1067-4682-adcf-7508969b5836",
                    UserName = "admin",
                    Email = "admin@admin.com",
                    AvatarUrl = "https://forum.codoh.com/images/avatars/avatar-blank.jpg",
                },
                new User //admin
                {
                    PasswordHash = "AKLDc4TLa2zb4FaIfRgJHRr74PqvpTjcKj1rHP6+gr4yGmB9tiKUEC478vXSyM365A==",
                    SecurityStamp = "c05cdbb3-bdf0-49cd-9100-b7b98dff60ee",
                    UserName = "Icakis",
                    Email = "Icakis_87@abv.bg",
                    Gender = Gender.Male,
                    Birthday = new DateTime(1987, 6, 16),
                    AvatarUrl = "https://forum.codoh.com/images/avatars/avatar-blank.jpg"
                }
            };
           var usersList = new List<User>
           {
                new User
                {
                    PasswordHash = "AA/axaHG/kGY+eFHHm3PsdMebx/28f3vwsMoxj0oeTkJEvOyCoZcjEsFQrFm5UzhMA==", //123456
                    SecurityStamp = "79089894-1067-4682-adcf-7508969b5836",
                    UserName = "Penka",
                    Email = "penka23@abv.bg",
                    AvatarUrl = "https://forum.codoh.com/images/avatars/avatar-blank.jpg"
                },
                new User
                {
                    PasswordHash = "AA/axaHG/kGY+eFHHm3PsdMebx/28f3vwsMoxj0oeTkJEvOyCoZcjEsFQrFm5UzhMA==", //123456
                    SecurityStamp = "c4933bb9-3645-44d0-bd41-1453ffbf072e",
                    UserName = "Stavri",
                    Email = "ss@abv.bg",
                    AvatarUrl = "https://forum.codoh.com/images/avatars/avatar-blank.jpg"
                },
                new User
                {
                    PasswordHash = "AA/axaHG/kGY+eFHHm3PsdMebx/28f3vwsMoxj0oeTkJEvOyCoZcjEsFQrFm5UzhMA==", //123456
                    SecurityStamp = "c4933bb9-3645-44d0-bd41-1453ffbf072e",
                    UserName = "Asen",
                    Email = "asen@gmail.com",
                    AvatarUrl = "https://forum.codoh.com/images/avatars/avatar-blank.jpg"
                },
                new User
                {
                    PasswordHash = "AA/axaHG/kGY+eFHHm3PsdMebx/28f3vwsMoxj0oeTkJEvOyCoZcjEsFQrFm5UzhMA==", //123456
                    SecurityStamp = "c4933bb9-3645-44d0-bd41-1453ffbf072e",
                    UserName = "Lili",
                    Email = "lili86@abv.bg",
                    AvatarUrl = "https://forum.codoh.com/images/avatars/avatar-blank.jpg"
                },
                new User
                {
                    PasswordHash = "AA/axaHG/kGY+eFHHm3PsdMebx/28f3vwsMoxj0oeTkJEvOyCoZcjEsFQrFm5UzhMA==", //123456
                    SecurityStamp = "c4933bb9-3645-44d0-bd41-1453ffbf072e",
                    UserName = "Maria",
                    Email = "mimi@abv.bg",
                    AvatarUrl = "https://forum.codoh.com/images/avatars/avatar-blank.jpg"
                },
                new User
                {
                    PasswordHash = "AA/axaHG/kGY+eFHHm3PsdMebx/28f3vwsMoxj0oeTkJEvOyCoZcjEsFQrFm5UzhMA==", //123456
                    SecurityStamp = "c4933bb9-3645-44d0-bd41-1453ffbf072e",
                    UserName = "Tosho",
                    Email = "tosho@gmail.com",
                    AvatarUrl = "https://forum.codoh.com/images/avatars/avatar-blank.jpg"
                }
            };

            foreach (var user in adminsList)
            {
                _context.Users.Add(user);
            }
            foreach (var user in usersList)
            {
                _context.Users.Add(user);
            }
            _context.SaveChanges();

            var adminRole = _context.Roles.FirstOrDefault(x => x.Name == "Administrator");
            foreach (var user in adminsList)
            {
                var newUserRole = new IdentityUserRole { RoleId = adminRole.Id, UserId = user.Id };
                user.Roles.Add(newUserRole);
            }
            _context.SaveChanges();

            return adminsList.Union(usersList);
        }

        private IEnumerable<Category> CreateCategories()
        {
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Trainers' Quotes",
                    MachineName = "quotes"
                },
                new Category
                {
                    Name = "Funny Pictures",
                    MachineName = "pictures"
                },
                new Category
                {
                    Name = "Jokes",
                    MachineName = "jokes"
                },
                new Category
                {
                    Name = "Funny Codes",
                    MachineName = "codes"
                }
            };

            foreach (var category in categories)
            {
                _context.Categories.Add(category);
            }

            _context.SaveChanges();

            return categories;
        }

        private void CreateRoles()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
            var roleCreateResult = roleManager.Create(new IdentityRole("Administrator"));
            if (!roleCreateResult.Succeeded)
            {
                throw new Exception(string.Join(", ", roleCreateResult.Errors));
            }
        }

        private static bool RandomBoolean()
        {
            var gen = new Random();
            return gen.Next(1) == 0;
        }

        private static int RandomNumber(int maxNum)
        {
            var gen = new Random();
            return gen.Next(maxNum);
        }
    }
}
