using System;
using System.Collections.Generic;
using SnackToSixPack.Classes;
using System.ComponentModel.DataAnnotations;

public class WorkoutPlan
{
    [Key]
    public string PlanName { get; set; }
    public string Goal { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<Workout> Workouts { get; set; } = new List<Workout>();
    
    public int UserId { get; set; }
}
