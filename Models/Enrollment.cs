﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace UpSkillz.Models;
public class Enrollment
{

    [Key]
    public int EnrollmentId { get; set; }

    [Required]
    public Course Course { get; set; } = new Course();

    [Required]
    public User Student { get; set; } = new User();

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime EnrollmentDate { get; set; } = DateTime.Now;

    [Required]
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
}

public enum EnrollmentStatus
{
    Active = 1,
    Completed = 2
}