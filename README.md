# BulkInsert.Cascade EF6&EF core
Simple library for cascade bulk-insert using Entity Framework 6 and Entity Framework core supporting MS SQL

## Features
- Bulk-insert EF entities, info about entities is retrieved from EF
- retrieve Id from DB for identity PK using Hi/Lo algorithm
- propagation id to foreign key columns
- cascade bulk-insert with unlimited depth

## Known Issues and limitation
- Only MS SQL is supported
- Just single column primary is allowed in case you want to do cascade bulk-insert
- retrieving primary keys is working just for identity columns and just for C# types int, long and short. If primary Key is not identity then I expect that primary keys will be fill out  and I don’t do any detection of new/existing entities – I don’t think that it makes much sense to extend it
- in case of identity columns bulk-insert recognize new Entities and do inserts just new ones, but it propagates ids for all entities (new and old ones).
- cascade bulk-insert expects that for all for all foreign key which are used for cascade bulk-insert 2 properties in entity: navigation property (for storing inserted value) and id property (for propagation of foreign key) – this is IMHO good practice anyway
- many to many without entity for relation table is  not supported in cascade bulk-insert – simple workaround is to map in-between table to some entity
## EF6 - Known Issues and limitation
- if you use 3 level inheritance in entities : C inherit from B, B inherit from A and you try to bulk-insert C then just properties from C and from A are inserted – this is bug in EF.Metadata nuget and I’ll fix with priority. Work around is to put properties in base Entity (in our case entity A). it will be mapped same to database and bulk-insert will work fine.
## EF core - Known Issues 
- Geodata are not supported

## Example
```c#
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BulkInsert.Cascade.Samples
{
    static class Program
    {
        static async Task Main()
        {
            var stopwatch = Stopwatch.StartNew();
            using var ctx = new SchoolContext();
            Console.WriteLine($"Before Bulk Insert: Students - {await ctx.Students.CountAsync()}, Grades:{await ctx.Grades.CountAsync()}");
            
            var students = Enumerable.Range(0, 100)
                .Select(o => new Student
                {
                    DateOfBirth = new DateTime(1999, 1, 1),
                    Height = 156m,
                    StudentName = Guid.NewGuid().ToString(),
                    Grades = Enumerable.Range(0, 100).Select(x => new Grade
                    {
                        GradeName = "Good",
                        SubjectName = "Some",
                    }).ToList()
                }).ToList();
            await ctx.BulkInsertCascade(students, new Cascade<Student, Grade>(o => o.Grades));
            Console.WriteLine($"After Bulk Insert: Students - {await ctx.Students.CountAsync()}, Grades:{await ctx.Grades.CountAsync()}");
            Console.WriteLine($"It took {stopwatch.ElapsedMilliseconds} ms");
            Console.ReadKey();
        }
    }

    public class SchoolContext : DbContext
    {
        public SchoolContext() : base("Data Source=.;Initial Catalog=School;Integrated Security=True;MultipleActiveResultSets=true")
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Grade> Grades { get; set; }
    }


    public class Student
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public decimal Height { get; set; }
        public ICollection<Grade> Grades { get; set; }
    }

    public class Grade
    {
        public int Id { get; set; }
        public string GradeName { get; set; }
        public string SubjectName { get; set; }
        public Student Student { get; set; }
        public int StudentId { get; set; }
    }
}
```
## Output
```console
Before Bulk Insert: Students - 300, Grades:30000
After Bulk Insert: Students - 400, Grades:40000
It took 2046 ms
```
