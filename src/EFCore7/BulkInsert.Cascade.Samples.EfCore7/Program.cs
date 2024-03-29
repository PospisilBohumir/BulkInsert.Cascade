﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BulkInsert.Cascade.EfCore7;
using Microsoft.EntityFrameworkCore;

namespace BulkInsert.Cascade.Samples.EfCore7
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
        public SchoolContext() : base(new DbContextOptionsBuilder<SchoolContext>()
            .UseSqlServer("Data Source=.;Initial Catalog=School;Integrated Security=True;MultipleActiveResultSets=true;Encrypt=False").Options)

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