using System.ComponentModel.DataAnnotations;

namespace SnackToSixPack.Models;

public class PlanExercise
{
    [Key]
    public int ExerciseId { get; set; }
    public int PlanId { get; set; }
}