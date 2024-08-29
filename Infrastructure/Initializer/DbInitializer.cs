using Application.Services.Interfaces;
using Domain;
using Domain.Entities.Account;
using Domain.Entities.Business;
using Domain.Entities.Static;
using Domain.SubEntities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly ISSOService _ssoService;
        private readonly Random _rnd;

        public DbInitializer(ApplicationDbContext context, IWebHostEnvironment hostEnv, ISSOService ssoService)
        {
            _context = context;
            _hostEnv = hostEnv;
            _ssoService = ssoService;
            _rnd = new Random();
        }

        public async Task Execute()
        {
            await _context.Database.EnsureDeletedAsync();


            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }

            if (!_context.ProgramYears.Any())
            {
                _context.ProgramYears.AddRange(ProgramYears);
                _context.SaveChanges();
            }

            if (!_context.IndicatorCategories.Any())
            {
                _context.IndicatorCategories.AddRange(IndicatorCategories);
                _context.SaveChanges();
            }

            if (!_context.IndicatorTypes.Any())
            {
                _context.IndicatorTypes.AddRange(IndicatorTypes);
                _context.SaveChanges();
            }

            if (!_context.Roles.Any())
            {
                _context.Roles.AddRange(Roles);
                _context.SaveChanges();
            }

            var comps = await _ssoService.GetCompaniesAsync();
            comps.Insert(0, AdminCompany);

            if (!_context.Company.Any())
            {
                int counter = 0;
                foreach (var comp in comps)
                {
                    counter++;
                    comp.ParentId = counter == 1 ? null : AdminCompany.Id;
                    if (comp.Id == Guid.Parse("aa12b4d2-652c-407a-a569-9edcd1e2c467"))
                    {
                        comp.Childs = ChildCompany(comp.Id);
                    }
                    _context.Company.Add(comp);

                }

                _context.SaveChanges();
            }

            comps = _context.Company.ToList();

            foreach (var comp in comps)
            {
                AddCompanyIndicator(comp.Id);

                var peopleId = AddPeople(comp.Id);

                AddHardware(comp.Id);

                AddSystems(comp.Id);

                var programsId = AddPrograms(comp.Id);

                AddBigGoals(comp.Id, programsId, peopleId);

                AddUsers(comp.Id);

                foreach (var ujr in UserJoinRequests)
                {
                    ujr.CompanyId = comp.Id;
                    ujr.Id = Guid.NewGuid();
                    _context.UsersJoinRequests.Add(ujr);
                }

                _context.SaveChanges();
            }

            if (!_context.Permissions.Any())
            {
                _context.Permissions.Add(Permissions);
                _context.SaveChanges();
            }

            if (!_context.RolePermissions.Any())
            {
                var permissions = _context.Permissions.ToList();
                var agentPermissions = permissions.Select(b => new RolePermission { Id = Guid.NewGuid(), PermissionId = b.Id, RoleId = SD.AgentId });
                _context.RolePermissions.AddRange(agentPermissions.DistinctBy(b => b.Id));
                await _context.SaveChangesAsync();
            }
        }



        #region Business
        private List<Program> Programs =>
            new List<Program>
            {
                new Program
                {
                    Id = Guid.Parse("629AEEF2-F5FF-408A-A68D-A2DCB74FF75C"),
                    Description = lorem,
                    StartedAt = DateTime.Now.AddDays(new Random().Next(-500,0)),
                    EndAt = DateTime.Now.AddDays(new Random().Next(100 , 600)),
                    Title = "راهبری 1404"
                },
                new Program
                {
                    Id = Guid.Parse("729AEEF2-F5FF-408A-A68D-A2DCB74FF75C"),
                    Description = lorem,
                    StartedAt = DateTime.Now.AddDays(new Random().Next(-500,0)),
                    EndAt = DateTime.Now.AddDays(new Random().Next(100 , 600)),
                    Title = "راهبردی 1403"
                },
                new Program
                {
                    Id = Guid.Parse("829AEEF2-F5FF-408A-A68D-A2DCB74FF75C"),
                    Description = lorem,
                    StartedAt = DateTime.Now.AddDays(new Random().Next(-500,0)),
                    EndAt = DateTime.Now.AddDays(new Random().Next(100 , 600)),
                    Title = "راهبردی 1402",
                    IsActive = true
                },
                new Program
                {
                    Id = Guid.Parse("929AEEF2-F5FF-408A-A68D-A2DCB74FF75C"),
                    Description = lorem,
                    StartedAt = DateTime.Now.AddDays(new Random().Next(-500,0)),
                    EndAt = DateTime.Now.AddDays(new Random().Next(100 , 600)),
                    Title = "راهبردی 1401"
                },
                new Program
                {
                    Id = Guid.Parse("529AEEF2-F5FF-408A-A68D-A2DCB74FF75C"),
                    Description = lorem,
                    StartedAt = DateTime.Now.AddDays(new Random().Next(-500,0)),
                    EndAt = DateTime.Now.AddDays(new Random().Next(100 , 600)),
                    Title = "راهبردی 1400"
                },
            };

        private List<Person> People =>
            new List<Person>
            {
                new Person{
                    Id = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8") ,
                    Name = "امیررضا" ,
                    Family = "محمدی" ,
                    Education = "لیسانس" ,
                    JobTitle = "برنامه نویس" ,
                    Expertises = new List<Expertise>
                        {
                            new Expertise{Title = "فرانت"},
                            new Expertise{Title = "سرور"},
                            new Expertise{Title = "مهندسی نرم افزار"},
                        }
                },
                new Person{
                    Id = Guid.Parse("440FF1EF-B4DD-4C6B-B943-A20EA411E9D8") ,
                    Name = "پریسا" ,
                    Family = "مرادی" ,
                    Education = "لیسانس" ,
                    JobTitle = "برنامه نویس" ,
                    Expertises = new List<Expertise>
                        {
                            new Expertise{Title = "فرانت"},
                        }
                },
                new Person{
                    Id = Guid.Parse("440FF1EF-A4DD-4C6B-B943-A20EA411E9D8"),
                    Name = "پیمان" ,
                    Family = "حیدری" ,
                    Education = "لیسانس" ,
                    JobTitle = "برنامه نویس" ,
                    Expertises = new List<Expertise>
                        {
                            new Expertise{Title = "طراحی رابط کاربری"},
                        }
                }
            };

        private List<BigGoal> BigGoals =>
            new List<BigGoal>
            {
                new BigGoal{
                    Id = Guid.NewGuid(),
                    Deadline = DateTime.Now.AddYears(1) ,
                    StartedAt= DateTime.Now ,
                    Title = "مجازی سازی دانشگاه" ,
                    Description = "مجازی سازی دانشگاه ها برای ایجاد برقراری ارتباط از راه دور و تدریس در هر شرایطی بدون نیاز به حضور در دانشگاه",
                },
                new BigGoal{
                    Id =  Guid.NewGuid(),
                    Deadline = DateTime.Now.AddYears(1) ,
                    StartedAt= DateTime.Now ,
                    Title = "هوشمند سازی کلاس ها" ,
                    Description = "مجازی سازی دانشگاه ها برای ایجاد برقراری ارتباط از راه دور و تدریس در هر شرایطی بدون نیاز به حضور در دانشگاه",
                 },
                new BigGoal{
                    Id =  Guid.Parse("4C6B36FE-4C0F-4D81-8F72-E9DBF80FC9DC"),
                    Deadline = DateTime.Now.AddYears(1) ,
                    StartedAt= DateTime.Now ,
                    Title = "افزایش سنوات دانشجویان" ,
                    Description = "مجازی سازی دانشگاه ها برای ایجاد برقراری ارتباط از راه دور و تدریس در هر شرایطی بدون نیاز به حضور در دانشگاه",
                },
            };

        private List<OperationalObjective> OperationalObjectives(Guid bigGoalId) =>
            new List<OperationalObjective>
            {
                new OperationalObjective{
                    Id = Guid.Parse("A98F453F-2B00-4C24-AAE8-45455E0FDE6B"),
                    Deadline = DateTime.Now.AddYears(1),
                    Budget = 1000000000,
                    Description = lorem,
                    BigGoalId = bigGoalId,
                    GuaranteedFulfillmentAt = DateTime.Now.AddMonths(10),
                    Title = "افزایش تعداد سرور ها",
                },
                new OperationalObjective{
                    Id = Guid.Parse("A98F453F-2B00-4C24-AAE8-45455E0FDE6A"),
                    Deadline = DateTime.Now.AddYears(1),
                    Budget = 157000000,
                    Description = lorem,
                    BigGoalId = bigGoalId,
                    GuaranteedFulfillmentAt = DateTime.Now.AddMonths(10),
                    Title = "ایجاد سامانه اموزشی",
                },
                new OperationalObjective{
                    Id = Guid.Parse("A98F453F-2B00-4C24-AAE8-45455E0FDE69"),
                    Deadline = DateTime.Now.AddYears(1),
                    Budget = 157000000,
                    Description = lorem,
                    BigGoalId = bigGoalId,
                    GuaranteedFulfillmentAt = DateTime.Now.AddMonths(10),
                    Title = "حذف سامانه های قدیمی",
                },
            };

        private List<Transition> Transitions =>
            new List<Transition>
            {
                new Transition
                {
                    Id = Guid.NewGuid(),
                    Contractor = "مهدی نیازی" ,
                    Deadline= DateTime.Now.AddYears(1),
                    LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    StartedAt = DateTime.Now,
                    Title = "بهینه سازی سرور های موجود",
                    OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                    Childs= new List<Transition>
                    {
                        new Transition
                        {
                            Id = Guid.NewGuid(),
                            Contractor = "میثم شریفی" ,
                            Deadline= DateTime.Now.AddYears(1),
                            LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                            StartedAt = DateTime.Now,
                            Title = "خرید پردازنده جدید",
                            OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                            Childs= new List<Transition>
                             {
                                 new Transition
                                 {
                                     Id = Guid.NewGuid(),
                                     Deadline= DateTime.Now.AddYears(1),
                                     LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                                     StartedAt = DateTime.Now,
                                     Title = "خرید پردازنده intel",
                                     OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                                 },
                                new Transition
                                 {
                                     Id = Guid.NewGuid(),
                                     Deadline= DateTime.Now.AddYears(1),
                                     LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                                     StartedAt = DateTime.Now,
                                     Title = "خرید پردازنده ryzen",
                                     OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                                 },
                             }
                        },
                        new Transition
                        {
                            Id = Guid.NewGuid(),
                            Contractor = "میثم شریفی" ,
                            Deadline= DateTime.Now.AddYears(1),
                            LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                            StartedAt = DateTime.Now,
                            Title = "بهبود و ارتقا رم",
                            OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                        },
                    }
                },

                new Transition
                {
                    Id = Guid.NewGuid(),
                    Contractor = "مهدی نیازی" ,
                    Deadline= DateTime.Now.AddYears(1),
                    LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    StartedAt = DateTime.Now,
                    Title = "افرایش سامانه ها",
                    OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                    Childs= new List<Transition>
                    {
                        new Transition
                        {
                            Id = Guid.NewGuid(),
                            Contractor = "میثم شریفی" ,
                            Deadline= DateTime.Now.AddYears(1),
                            LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                            StartedAt = DateTime.Now,
                            Title = "خرید پردازنده جدید",
                            OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                            Childs= new List<Transition>
                             {
                                 new Transition
                                 {
                                     Id = Guid.NewGuid(),
                                     Deadline= DateTime.Now.AddYears(1),
                                     LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                                     StartedAt = DateTime.Now,
                                     Title = "خرید پردازنده intel",
                                     OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                                 },
                                new Transition
                                 {
                                     Id = Guid.NewGuid(),
                                     Deadline= DateTime.Now.AddYears(1),
                                     LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                                     StartedAt = DateTime.Now,
                                     Title = "خرید پردازنده ryzen",
                                     OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                                 },
                             }
                        },
                        new Transition
                        {
                            Id = Guid.NewGuid(),
                            Contractor = "میثم شریفی" ,
                            Deadline= DateTime.Now.AddYears(1),
                            LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                            StartedAt = DateTime.Now,
                            Title = "بهبود و ارتقا رم",
                            OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                            Childs = new List<Transition>
                            {
                                new Transition
                                 {
                                     Id = Guid.NewGuid(),
                                     Deadline= DateTime.Now.AddYears(1),
                                     LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                                     StartedAt = DateTime.Now,
                                     Title = "خرید پردازنده intel",
                                     OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                                 },
                                new Transition
                                 {
                                     Id = Guid.NewGuid(),
                                     Deadline= DateTime.Now.AddYears(1),
                                     LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                                     StartedAt = DateTime.Now,
                                     Title = "خرید پردازنده ryzen",
                                     OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                                 },
                            }
                        },
                    }
                },
                new Transition
                {
                    Id = Guid.NewGuid(),
                    Contractor = "محمد قاری" ,
                    Deadline= DateTime.Now.AddYears(1),
                    LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    StartedAt = DateTime.Now,
                    Title = "ایجاد پلتفرم ازمون",
                    OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                },
            };

        private List<HardwareEquipment> HardwareEquipments =>
            new List<HardwareEquipment>
            {
                new HardwareEquipment
                {
                    Id =  Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "سرور",
                    BrandName = "hp",
                    Count = 3,
                    SupportType = "وارانتی پیشنهادی",
                    Description = lorem
                },
                new HardwareEquipment
                {
                    Id = Guid.Parse("540FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "مانیتور",
                    BrandName = "apple",
                    Count = 3,
                    SupportType = "وارانتی متناسب با یک هدف خاص",
                    Description = lorem
                },
                new HardwareEquipment
                {
                    Id = Guid.Parse("640FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "کیبورد",
                    BrandName = "dell",
                    Count = 20,
                    SupportType = "وارانتی متناسب با یک هدف خاص",
                    Description = lorem
                },
                new HardwareEquipment
                {
                    Id = Guid.Parse("740FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "ماوس",
                    BrandName = "dell",
                    Count = 20,
                    SupportType = "وارانتی متناسب با یک هدف خاص",
                    Description = lorem
                },
            };

        private List<Domain.Entities.Business.System> Systems =>
            new List<Domain.Entities.Business.System>
            {
                new Domain.Entities.Business.System
                {
                    Id = Guid.Parse("441FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "سیستم 1",
                    Database = "SQL Server",
                    Development = lorem,
                    BuildInCompany = "ویتکو",
                    Framework = lorem,
                    OS = "ویندوز",
                    ProgrammingLanguage = "C#",
                    Description= lorem,
                    SupportType = lorem
                },
                new Domain.Entities.Business.System
                {
                    Id = Guid.Parse("442FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "سیستم 2",
                    Database = "Mongo",
                    Development = lorem,
                    BuildInCompany = "ویتکو",
                    Framework = lorem,
                    OS = "لینوکس",
                    ProgrammingLanguage = "JS",
                    Description= lorem,
                    SupportType = lorem
                },
                new Domain.Entities.Business.System
                {
                    Id = Guid.Parse("443FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "سیستم 3",
                    Database = "Elastic Search",
                    Development = lorem,
                    BuildInCompany = "ویتکو",
                    Framework = lorem,
                    OS = "ویندوز",
                    ProgrammingLanguage = "Python",
                    Description= lorem,
                    SupportType = lorem
                },
            };

        private IndicatorProgress IndicatorProgress(Guid id, Guid indicatorId) =>
            new IndicatorProgress
            {
                Id = id,
                IndicatorId = indicatorId,
                ProgressTime = DateTime.Now.AddDays(new Random().Next(-300, 300)),
            };

        private List<SWOT> SWOTs =>
            new List<SWOT>
            {
                new SWOT
                {
                    Content = "افزایش میزان بازدهی کلاس ها",
                    Type = SWOTType.Strengths
                },
                new SWOT
                {
                    Content = "امکان ارائه کلاس های بیشتر",
                    Type = SWOTType.Strengths
                },
                new SWOT
                {
                    Content = "کاهش منابع مصرفی دانشگاه و کارمندان",
                    Type = SWOTType.Strengths
                },
                new SWOT
                {
                    Content = "کاهش مهارت های اجتماعی دانشجویان",
                    Type = SWOTType.Weaknesses
                },
                new SWOT
                {
                    Content = "ناتوانی نسبی در اجرای کلاس های علوم ورزشی",
                    Type = SWOTType.Weaknesses
                },
                new SWOT
                {
                    Content = "کاهش مسئولیت پذیری و افزایش تقلب در امتحانات",
                    Type = SWOTType.Threats
                },
                new SWOT
                {
                    Content = "افزایش رشد و نواوری دانشگاه",
                    Type = SWOTType.Opportunities
                },
                new SWOT
                {
                    Content = "همگام کردن مسائل دانشگاه به روش نوین و سیستماتیک",
                    Type = SWOTType.Opportunities
                },
            };

        private List<Strategy> Strategies =>
            new List<Strategy>
            {
                new Strategy
                {
                    Content = "استفاده حداکثری از ظرفیت سامانه"
                },
                new Strategy
                {
                    Content = "استفاده از ICT"
                },
                new Strategy
                {
                    Content = "کاهش مخارج دانشگاه"
                },
            };
        #endregion

        #region Account
        private List<Company> Companies()
        {
            var companies = _ssoService.GetCompaniesAsync().GetAwaiter().GetResult();
            return companies;
        }

        private List<Company> ChildCompany(Guid parentId) =>
            new List<Company>
                        {
                            new Company
                            {
                                Id = Guid.Parse("FD88FE22-5AB8-48E9-B649-9A9F5FE93180"),
                                ParentId = parentId,
                                Title = "واحد علم و فناوری",
                                Childs = new List<Company>
                                {
                                    new Company
                                    {
                                        Id = Guid.Parse("03070627-8EDB-410C-AB95-7F29E5FA2676"),
                                        ParentId =  Guid.Parse("FD88FE22-5AB8-48E9-B649-9A9F5FE93180"),
                                        Title = "واحد آپا",
                                        Childs = new List<Company>
                                        {
                                            new Company{Id = Guid.NewGuid() , Title ="واحد آپا مرکزی", ParentId = Guid.Parse("03070627-8EDB-410C-AB95-7F29E5FA2676")},
                                            new Company{Id = Guid.NewGuid() , Title ="واحد آپا دانشکده کامپیوتر", ParentId = Guid.Parse("03070627-8EDB-410C-AB95-7F29E5FA2676")},
                                            new Company{Id = Guid.NewGuid() , Title ="واحد آپا دانشکده کشاورزی", ParentId = Guid.Parse("03070627-8EDB-410C-AB95-7F29E5FA2676")},

                                        }
                                    },
                                    new Company
                                    {
                                        Id = Guid.NewGuid(),
                                        ParentId =  Guid.Parse("FD88FE22-5AB8-48E9-B649-9A9F5FE93180"),
                                        Title = "مرکز هوش مصنوعی",
                                    },
                                }
                            },
                            new Company
                            {
                                Id = Guid.NewGuid(),
                                ParentId = parentId,
                                Title = "واحد شبکه"
                            },
                            new Company
                            {
                                Id = Guid.NewGuid(),
                                ParentId = parentId,
                                Title = "واحد سلامت"
                            },
                        };

        private Guid AdminCompanyId = Guid.Parse("7992DB78-0F89-4CDB-B13D-0E2E500DEB06");

        private Company AdminCompany => new Company
        {
            Id = AdminCompanyId,
            Title = "ادمین سیستم"
        };

        private List<Role> Roles =>
            new List<Role>
            {
                  new Role
                {
                    Id = SD.AgentId,
                    Title = "نماینده سازمان",
                    Description = "نماینده مشخص شده از یک سازمان"
                },
                new Role
                {
                    Id = Guid.Parse("3D768081-EAF8-4BC2-A14F-601C7AAB9338"),
                    Title = "کارشناش شبکه",
                    Description = "کارشناش شبکه"
                },

                new Role
                {
                    Id = Guid.Parse("0D4618FF-7576-47AC-85B8-ABD285939492"),
                    Title = "اپراتور",
                    Description = "اپراتور"
                },
                new Role
                {
                    Id = Guid.Parse("18CD3FC8-8314-4F6C-AE0F-1EA923FA647E"),
                    Title = "کارشنای مرکز داده",
                    Description = "کارشنای مرکز داده"
                },
            };

        private Permission Permissions =>
            new PermissionContainer(
                SD.TopPermissionId,
                "سطوح دسترسی",
                PermissionType.General,
                new List<Permission>
                {
                    new PermissionContainer(
                        "مسئولیت" ,
                        PermissionType.General,
                        new List<Permission>
                        {

                            new PermissionContainer("S.W.O.T" , PermissionType.System ,
                                new List<Permission> {
                                    new PermissionItem("مشاهده S.W.O.T", PermissionType.System , PermissionsSD.QuerySWOT),
                                    new PermissionItem("ویرایش S.W.O.T", PermissionType.System , PermissionsSD.CommandSWOT),
                                }),

                            new PermissionContainer("راهبرد" , PermissionType.System ,
                                new List<Permission> {
                                    new PermissionItem("مشاهده راهبرد", PermissionType.System , PermissionsSD.QueryStrategy),
                                    new PermissionItem("ویرایش راهبرد", PermissionType.System , PermissionsSD.CommandStrategy),
                                }),

                            new PermissionContainer("دورنما" , PermissionType.System ,
                                new List<Permission> {
                                    new PermissionItem("مشاهده دورنما", PermissionType.System , PermissionsSD.SeePerspective),
                                    new PermissionItem("ویرایش دورنما", PermissionType.System , PermissionsSD.UpsertPerspective),
                                }),

                            new PermissionContainer("هدف کلان" , PermissionType.Company ,
                                new List<Permission>
                            {
                                new PermissionItem("مشاهده هدف کلان" , PermissionType.Company , PermissionsSD.QueryBigGoal),
                                new PermissionItem("ویرایش هدف کلان" , PermissionType.Company , PermissionsSD.CommandBigGoal),
                            }),

                            new PermissionContainer("هدف عملیاتی" , PermissionType.Company ,
                                new List<Permission>
                            {
                                new PermissionItem("مشاهده هدف عملیاتی"  , PermissionType.Company,  PermissionsSD.QueryOperationalObjective),
                                new PermissionItem("ویرایش هدف عملیاتی"  , PermissionType.Company,  PermissionsSD.CommandOperationalObjective),
                            }),

                            new PermissionContainer("گذار" , PermissionType.Company ,
                                new List<Permission>
                                {
                                    new PermissionItem("مشاهده پروژه و اقدام" , PermissionType.Company ,  PermissionsSD.QueryTransition),
                                    new PermissionItem("ویرایش پروژه و اقدام" , PermissionType.Company ,  PermissionsSD.CommandTransition),
                                }),
                        }),
                    new PermissionContainer(
                        SD.ManagmentPermissionId ,
                        "مدیریت" ,
                        PermissionType.System ,
                        new List<Permission>
                        {

                            new PermissionItem("بررسی اطلاعات دانشگاه ها" , PermissionType.System , PermissionsSD.FilterCompany),
                            new PermissionContainer("دارایی" , PermissionType.Company ,
                                new List<Permission>{
                                new PermissionContainer("فرد" , PermissionType.Company ,
                                    new List<Permission>
                                    {
                                        new PermissionItem("مشاهده فرد" , PermissionType.Company ,  PermissionsSD.QueryPerson),
                                        new PermissionItem("ویرایش فرد" , PermissionType.Company ,  PermissionsSD.CommandPerson),
                                    }),
                                new PermissionContainer("تجهیز سخت افزاری" , PermissionType.Company ,
                                    new List<Permission>
                                    {
                                        new PermissionItem("مشاهده تجهیز سخت افزاری" , PermissionType.Company ,  PermissionsSD.QueryHardwareEquipment),
                                        new PermissionItem("ویرایش تجهیز سخت افزاری" , PermissionType.Company ,  PermissionsSD.CommandHardwareEquipment),
                                    }),
                                new PermissionContainer("سامانه" , PermissionType.Company ,
                                    new List<Permission>
                                    {
                                        new PermissionItem("مشاهده سامانه" , PermissionType.Company ,  PermissionsSD.QuerySystem),
                                        new PermissionItem("ویرایش سامانه" , PermissionType.Company ,  PermissionsSD.CommandSystem),
                                    }),
                            }),
                            new PermissionContainer(
                                 "روند" ,
                                 PermissionType.System,
                                 new List<Permission>
                                 {
                                     new PermissionContainer("برنامه" , PermissionType.System ,
                                        new List<Permission> {
                                           new PermissionItem("مشاهده برنامه", PermissionType.System , PermissionsSD.QueryProgram),
                                           new PermissionItem("ویرایش برنامه", PermissionType.System , PermissionsSD.CommandProgram),
                                        }),
                                     new PermissionContainer("طبقه بندی شاخص" , PermissionType.System ,
                                        new List<Permission> {
                                            new PermissionItem("مشاهده طبقه بندی شاخص", PermissionType.System , PermissionsSD.QueryIndicatorCategory),
                                            new PermissionItem("ویرایش طبقه بندی شاخص", PermissionType.System , PermissionsSD.CommandIndicatorCategory),
                                        }),
                                    new PermissionContainer("واحد شاخص" , PermissionType.System ,
                                        new List<Permission> {
                                            new PermissionItem("مشاهده واحد شاخص", PermissionType.System , PermissionsSD.QueryIndicatorType),
                                            new PermissionItem("ویرایش واحد شاخص", PermissionType.System , PermissionsSD.CommandIndicatorType),
                                        }),
                                 }),
                             new PermissionContainer(
                                "حساب" ,
                                PermissionType.General ,
                                new List<Permission>
                                {
                                    new PermissionContainer("نقش", PermissionType.System , new List<Permission>
                                    {
                                        new PermissionItem("مشاهده نقش" , PermissionType.System ,PermissionsSD.QueryRole),
                                        new PermissionItem("ویرایش نقش" , PermissionType.System ,PermissionsSD.CommandRole),
                                    }),
                                    new PermissionContainer("کاربران", PermissionType.System , new List<Permission>
                                    {
                                        new PermissionItem("مشاهده کاربران" , PermissionType.System ,PermissionsSD.UsersList),
                                        new PermissionItem("مدیریت درخواست ورود کاربر", PermissionType.System , PermissionsSD.UsersRequests),
                                        new PermissionItem("حذف کاربر", PermissionType.System , PermissionsSD.RemoveUser),
                                        new PermissionItem("تعیین دسترسی", PermissionType.System , PermissionsSD.ManageUserRole),
                                    }),
                                    new PermissionContainer("سازمان ها" , PermissionType.System ,
                                        new List<Permission> {
                                            new PermissionItem("مشاهده ساختار سازمانی" , PermissionType.System , PermissionsSD.QueryCompany),
                                            new PermissionItem("ویرایش سازمان" , PermissionType.System , PermissionsSD.CommandCompany),
                                    }),
                             }),

                        }),
                }
            );

        private List<User> Users =>
            new List<User> {
                new User
                {
                    Id = Guid.NewGuid(),
                    NationalId = SD.DefaultNationalId,
                    Name = "امیررضا",
                    Family = "محمدی",
                    IsActive = true,
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    NationalId = "3360408330",
                    Name = "نادر",
                    Family = "یاری",
                    IsActive = true
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    NationalId = "3360408331",
                    Name = "رضا",
                    Family = "اسدی",
                    IsActive = true
                }
            };

        private List<UserJoinRequest> UserJoinRequests =>
            new List<UserJoinRequest>
            {
                new UserJoinRequest
                {
                    NationalId = "3360405050",
                    FullName = "نادر سلیمانی",
                    PhoneNumber = "09211572323",
                },
                new UserJoinRequest
                {
                    NationalId = "3360404050",
                    FullName = "محمد باقری",
                    PhoneNumber = "09371552693",
                },
            };

        #endregion

        #region Static

        private List<ProgramYear> ProgramYears =>
            new List<ProgramYear>
            {
                new ProgramYear{Id = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30E"),Year = "1401-1402"},
                new ProgramYear{Id = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30F"),Year = "1400-1401"},
                new ProgramYear{Id = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30A"),Year = "1399-1400"},
            };

        private List<IndicatorCategory> IndicatorCategories =>
            new List<IndicatorCategory>
            {
                new IndicatorCategory
                {
                    Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA98CB"),
                    Title = "فناوری اطلاعات",
                    Childs = new List<IndicatorCategory>
                    {
                        new IndicatorCategory
                        {
                            Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA98CA"),
                            Title = "سرور"
                        },
                        new IndicatorCategory
                        {
                            Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA98CE"),
                            Title = "شبکه و ارتباطات",
                            Childs = new List<IndicatorCategory>
                            {
                                new IndicatorCategory
                                {
                                    Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA97CB"),
                                    Title = "طبقه بندی 1"
                                },
                                new IndicatorCategory
                                {
                                    Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA96CB"),
                                    Title = "طبقه بندی 2"
                                }
                            }
                        },
                        new IndicatorCategory
                        {
                            Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA95CB"),
                            Title = "اطلاع رسانی",
                            Childs = new List<IndicatorCategory>
                            {
                                new IndicatorCategory
                                {
                                    Id =  Guid.Parse("D1089483-938C-401B-A003-BEFF50EA94CB"),
                                    Title = "طبقه بندی 1"
                                },
                                new IndicatorCategory
                                {
                                    Id =  Guid.Parse("D1089483-938C-401B-A003-BEFF50EA93CB"),
                                    Title = "طبقه بندی 2"
                                },
                                new IndicatorCategory
                                {
                                    Id =  Guid.Parse("D1089483-938C-401B-A003-BEFF50EA92CB"),
                                    Title = "طبقه بندی 3"
                                }
                            }
                        }
                    }
                },
                new IndicatorCategory
                {
                    Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA91CB"),
                    Title = "توسعه و پیشرفت",
                    Childs = new List<IndicatorCategory>
                    {
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA81CB"),
                             Title = "طبقه بندی 1"
                         },
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA71CB"),
                             Title = "طبقه بندی 2"
                         },
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA61CB"),
                             Title = "طبقه بندی 3"
                         },
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA51CB"),
                             Title = "طبقه بندی 3"
                         }
                    }
                },
                new IndicatorCategory
                {
                    Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA41CB"),
                    Title = "بهداشت و سلامت",
                    Childs = new List<IndicatorCategory>
                    {
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("C1089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 1",
                             Childs = new List<IndicatorCategory>
                    {
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("B1089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 1"
                         },
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("A1089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 2"
                         },
                    }
                         },
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("91089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 2",
                             Childs = new List<IndicatorCategory>
                    {
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("81089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 1"
                         },
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("71089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 2"
                         },
                    }
                         },
                         new IndicatorCategory
                         {
                             Id =Guid.Parse("61089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 3"
                         },
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("51089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 3"
                         }
                    }
                },
            };

        private List<IndicatorType> IndicatorTypes =>
            new List<IndicatorType>
            {
                new IndicatorType
                {
                    Id = Guid.Parse("406FA9EC-1FB9-48DF-B6D1-90CEBB3F9F62"),
                    Title = "تعداد"
                },
                new IndicatorType
                {
                    Id =  Guid.Parse("406FA9EC-1FB9-48DF-B6D1-90CEBB3F9F61"),
                    Title = "تومان"
                },
                new IndicatorType
                {
                    Id =  Guid.Parse("406FA9EC-1FB9-48DF-B6D1-90CEBB3F9F60"),
                    Title = "ریال"
                },
            };
        #endregion

        private Indicator CustomIndicator(Guid id) =>
                new Indicator
                {
                    Id = id,
                    FromDate = DateTime.Now.AddDays(new Random().Next(-700, 0)),
                    ToDate = DateTime.Now.AddDays(new Random().Next(0, 700)),
                    CategoryId = IndicatorCategories[new Random().Next(0, IndicatorCategories.Count - 1)].Id,
                    GoalValue = new Random().Next(500, 10000),
                    InitValue = new Random().Next(0, 499),
                    Period = (IndicatorPeriod)(new Random().Next(0, 6) * 10),
                    TypeId = IndicatorTypes[new Random().Next(0, IndicatorTypes.Count - 1)].Id,
                };

        private void AddCompanyIndicator(Guid companyId)
        {
            for (int i = 0; i < _rnd.Next(1, 6); i++)
            {
                var indicator = CustomIndicator(Guid.NewGuid());
                var companyIndicator = new CompanyIndicator { CompanyId = companyId, IndicatorId = indicator.Id };

                for (int j = 0; j < _rnd.Next(0, 7); j++)
                {
                    var progressIndicator = IndicatorProgress(Guid.NewGuid(), indicator.Id);
                    progressIndicator.Value = _rnd.NextInt64(indicator.InitValue, indicator.GoalValue);
                    _context.IndicatorProgresses.Add(progressIndicator);
                }

                _context.Indicators.Add(indicator);
                _context.CompanyIndicators.Add(companyIndicator);
            }
        }

        private List<Guid> AddPeople(Guid companyId)
        {
            List<Guid> peopleId = new List<Guid>();

            foreach (var person in People)
            {
                person.CompanyId = companyId;
                person.Id = Guid.NewGuid();
                peopleId.Add(person.Id);
                _context.People.Add(person);
            }

            return peopleId;
        }

        private void AddHardware(Guid companyId)
        {
            foreach (var heq in HardwareEquipments)
            {
                heq.CompanyId = companyId;
                heq.Id = Guid.NewGuid();
                _context.HardwareEquipment.Add(heq);
            }
        }

        private void AddSystems(Guid companyId)
        {
            foreach (var system in Systems)
            {
                system.CompanyId = companyId;
                system.Id = Guid.NewGuid();
                _context.Systems.Add(system);
            }
        }

        private List<Guid> AddPrograms(Guid companyId)
        {
            List<Guid> programsId = new List<Guid>();

            foreach (var program in Programs)
            {
                program.Id = Guid.NewGuid();
                programsId.Add(program.Id);
                program.CompanyId = companyId;

                _context.Program.Add(program);

                foreach (var item in SWOTs)
                {
                    item.Id = Guid.NewGuid();
                    item.ProgramId = program.Id;
                    _context.SWOT.Add(item);
                }

                foreach (var item in Strategies)
                {
                    item.Id = Guid.NewGuid();
                    item.ProgramId = program.Id;
                    _context.Strategy.Add(item);
                }

            }

            return programsId;
        }

        private void AddBigGoals(Guid companyId, List<Guid> programsId, List<Guid> peopleId)
        {
            foreach (var bigGoal in BigGoals)
            {
                bigGoal.Id = Guid.NewGuid();

                for (int i = 0; i < _rnd.Next(1, 6); i++)
                {
                    var indicator = CustomIndicator(Guid.NewGuid());
                    var bid = new BigGoalIndicator { BigGoalId = bigGoal.Id, IndicatorId = indicator.Id };

                    for (int j = 0; j < _rnd.Next(0, 7); j++)
                    {
                        var progressIndicator = IndicatorProgress(Guid.NewGuid(), indicator.Id);
                        progressIndicator.IndicatorId = indicator.Id;
                        progressIndicator.Value = _rnd.NextInt64(indicator.InitValue, indicator.GoalValue);
                        _context.IndicatorProgresses.Add(progressIndicator);
                    }

                    _context.Indicators.Add(indicator);
                    _context.BigGoalIndicators.Add(bid);
                }

                int startIndex = _rnd.Next(1, programsId.Count);

                for (int i = startIndex; i < programsId.Count; i++)
                {
                    var programBigGoal = new ProgramBigGoal(programsId[i], bigGoal.Id);
                    _context.ProgramBigGoal.Add(programBigGoal);
                }

                _context.BigGoals.Add(bigGoal);

                var operationalObjectives = OperationalObjectives(bigGoal.Id);

                foreach (var opo in operationalObjectives)
                {
                    opo.Id = Guid.NewGuid();
                    _context.OperationalObjectives.Add(opo);

                    for (int i = 0; i < _rnd.Next(1, 6); i++)
                    {
                        var ind = CustomIndicator(Guid.NewGuid());

                        var opoInd = new OperationalObjectiveIndicator
                        {
                            IndicatorId = ind.Id,
                            OperationalObjectiveId = opo.Id
                        };

                        for (int j = 0; j < _rnd.Next(0, 7); j++)
                        {
                            var progressIndicator = IndicatorProgress(Guid.NewGuid(), ind.Id);
                            progressIndicator.Value = _rnd.NextInt64(ind.InitValue, ind.GoalValue);
                            _context.IndicatorProgresses.Add(progressIndicator);
                        }

                        _context.Indicators.Add(ind);
                        _context.OperationalObjectiveIndicators.Add(opoInd);
                    }

                    foreach (var transition in Transitions)
                    {
                        AddTransition(transition, opo.Id, peopleId, null, _rnd.Next(0, 2) == 1 ? TransitionType.Project : TransitionType.Action);
                    }

                }
            }
        }

        private void AddUsers(Guid companyId)
        {
            var counter = 0;

            if (companyId == AdminCompanyId)
            {
                var usersList = new List<User>
                {
                    new User
                    {
                    Id = Guid.NewGuid(),
                    NationalId = "3838624092",
                    Name = "فرهاد",
                    Family = "مردوخی",
                    IsActive = true,
                    },
                    new User
                     {
                        Id = Guid.NewGuid(),
                        NationalId = "3360408330",
                        Name = "امیررضا",
                        Family = "محمدی",
                        IsActive = true,
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        NationalId = "3370108968",
                        Name = "منصور",
                        Family = "بیجاد",
                        IsActive = true,
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        NationalId = "3242378644",
                        Name = "عارف",
                        Family = "منوچهری",
                        IsActive = true,
                   },
                };

                foreach (var user in usersList)
                {
                    var act =
                          new Act
                          {
                              Id = Guid.NewGuid(),
                              CompanyId = companyId,
                              UserId = user.Id,
                              RoleId = SD.AgentId
                          };

                    _context.Act.Add(act);
                    _context.Users.Add(user);
                }

            }


            foreach (var user in Users)
            {
                user.Id = Guid.NewGuid();
                user.NationalId = counter == 0 && companyId == AdminCompanyId ? SD.DefaultNationalId : _rnd.NextInt64(1111111111, 9999999999).ToString();

                var act =
                  new Act
                  {
                      Id = Guid.NewGuid(),
                      CompanyId = companyId,
                      UserId = user.Id,
                      RoleId = counter == 0 ? SD.AgentId : Roles[_rnd.Next(1, Roles.Count - 1)].Id
                  };

                _context.Act.Add(act);
                _context.Users.Add(user);

                counter++;
            }
        }

        private void AddTransition(Transition transition, Guid opId, List<Guid> peopleId, Guid? parentId, TransitionType type)
        {
            transition.Id = Guid.NewGuid();
            transition.ParentId = parentId;
            transition.OperationalObjectiveId = opId;
            transition.LeaderId = peopleId[_rnd.Next(peopleId.Count - 1)];
            transition.Type = type;

            transition.Financials = People.Take(_rnd.Next(0, People.Count - 1)).Select(b => new Financial { Title = b.Name + " " + b.Family }).ToList();
            transition.Financials.AddRange(HardwareEquipments.Take(_rnd.Next(0, HardwareEquipments.Count - 1)).Select(b => new Financial { Title = b.Title }).ToList());
            transition.Financials.AddRange(Systems.Take(_rnd.Next(0, Systems.Count - 1)).Select(b => new Financial { Title = b.Title }).ToList());


            for (int i = 0; i < _rnd.Next(1, 6); i++)
            {
                var ind = CustomIndicator(Guid.NewGuid());

                var projectInd = new TransitionIndicator { IndicatorId = ind.Id, TransitionId = transition.Id };

                for (int j = 0; j < _rnd.Next(0, 7); j++)
                {
                    var progressIndicator = IndicatorProgress(Guid.NewGuid(), ind.Id);
                    progressIndicator.Value = _rnd.NextInt64(ind.InitValue, ind.GoalValue);

                    _context.IndicatorProgresses.Add(progressIndicator);
                }

                _context.Indicators.Add(ind);
                _context.TransitionIndicators.Add(projectInd);
            }

            _context.Transitions.Add(transition);

            foreach (var item in transition.Childs)
                AddTransition(item, opId, peopleId, transition.Id, _rnd.Next(0, 2) == 1 ? TransitionType.Project : TransitionType.Action);
        }


        private String lorem = "لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است. چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد. کتابهای زیادی در شصت و سه درصد گذشته، حال و آینده شناخت فراوان جامعه و متخصصان را می طلبد تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی و فرهنگ پیشرو در زبان فارسی ایجاد کرد. در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها و شرایط سخت تایپ به پایان رسد وزمان مورد نیاز شامل حروفچینی دستاوردهای اصلی و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.";
    }
}
